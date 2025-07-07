using InfertilityTreatment.Business.Exceptions;
using InfertilityTreatment.Business.Helpers;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Payments;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace InfertilityTreatment.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaymentGatewayConfig _paymentConfig;

        public PaymentService(IUnitOfWork unitOfWork, IOptions<PaymentGatewayConfig> paymentConfig)
        {
            _unitOfWork = unitOfWork;
            _paymentConfig = paymentConfig.Value;
        }

        public async Task<PaymentResponseDto> CreateVNPayPaymentAsync(CreatePaymentDto dto)
        {
            try
            {
                // Get TreatmentPackage to retrieve amount
                var treatmentPackage = await _unitOfWork.TreatmentPackages.GetByIdAsync(dto.TreatmentPackageId);
                if (treatmentPackage == null)
                {
                    throw new KeyNotFoundException($"TreatmentPackage with ID {dto.TreatmentPackageId} not found");
                }

                var userExist = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
                if (userExist == null)
                {
                    throw new KeyNotFoundException($"customer with ID {dto.CustomerId} not found");
                }

                // Generate unique payment ID
                var paymentId = Guid.NewGuid().ToString();

                var descrip = $"Payment-{paymentId}";
                // Create payment record with amount from TreatmentPackage
                var payment = new Payment
                {
                    PaymentId = paymentId,
                    CustomerId = dto.CustomerId,
                    TreatmentPackageId = dto.TreatmentPackageId,
                    Amount = treatmentPackage.Price, // Get amount from TreatmentPackage.Price
                    PaymentMethod = "VNPay",
                    Status = PaymentStatus.Pending,
                    Description = descrip,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync(); // <-- Lưu để có `payment.Id`

                // Create payment log (sau khi đã có payment.Id)
                var paymentLog = new PaymentLog
                {
                    PaymentId = payment.Id, 
                    Action = "Created",
                    Status = PaymentStatus.Pending.ToString(),
                    RequestData = JsonSerializer.Serialize(dto),
                    Notes = "Payment created for VNPay processing",
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentLogRepository.AddAsync(paymentLog);
                await _unitOfWork.SaveChangesAsync(); 

                // Generate VNPay payment URL
                var paymentUrl = GenerateVNPayPaymentUrl(dto, paymentId, treatmentPackage.Price, descrip);

                return new PaymentResponseDto
                {
                    PaymentId = paymentId,
                    PaymentUrl = paymentUrl,
                    Status = "Pending",
                    Message = "Payment URL generated successfully",
                    Amount = treatmentPackage.Price, // Return actual amount from package
                    CreatedAt = DateTime.UtcNow,
                    TransactionId = paymentId
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating VNPay payment: {ex.Message}", ex);
            }
        }

        public async Task<bool> HandleVNPayCallbackAsync(VNPayCallbackDto dto)
        {
            try
            {
                // Validate essential fields first
                if (string.IsNullOrEmpty(dto.vnp_TxnRef))
                {
                    await LogPaymentCallback("UNKNOWN", "Missing vnp_TxnRef", dto);
                    return false;
                }

                if (string.IsNullOrEmpty(dto.vnp_ResponseCode))
                {
                    await LogPaymentCallback(dto.vnp_TxnRef, "Missing vnp_ResponseCode", dto);
                    return false;
                }

                // Validate signature
                if (!await ValidateVNPaySignature(dto))
                {
                    await LogPaymentCallback(dto.vnp_TxnRef, "Invalid signature", dto);
                    return false;
                }

                // Find payment by PaymentId (stored as vnp_TxnRef)
                var payment = await _unitOfWork.PaymentRepository.GetByPaymentIdAsync(dto.vnp_TxnRef);
                if (payment == null)
                {
                    await LogPaymentCallback(dto.vnp_TxnRef, "Payment not found", dto);
                    return false;
                }

                // Update payment status based on VNPay response
                var isSuccess = dto.vnp_ResponseCode == "00";
                var statusPayment = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed; 
                if (!isSuccess)
                {
                    statusPayment = dto.vnp_ResponseCode == "24" ? PaymentStatus.Cancelled : PaymentStatus.Failed;
                }
                payment.Status = statusPayment;

                payment.TransactionId = dto.vnp_TransactionNo;
                payment.ProcessedAt = VNPayHelper.ParsePayDate(dto.vnp_PayDate) ?? DateTime.UtcNow;
                payment.PaymentGatewayResponse = JsonSerializer.Serialize(dto);

                await _unitOfWork.PaymentRepository.UpdateAsync(payment);

                // Create payment log
                var paymentLog = new PaymentLog
                {
                    PaymentId = payment.Id,
                    Action = "Callback",
                    Status = payment.Status.ToString(),
                    ResponseData = JsonSerializer.Serialize(dto),
                    Notes = isSuccess ? "Payment completed successfully" : $"Payment failed: {dto.vnp_ResponseCode}",
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentLogRepository.AddAsync(paymentLog);
                await _unitOfWork.SaveChangesAsync();

                return isSuccess;
            }
            catch (Exception ex)
            {
                await LogPaymentCallback(dto.vnp_TxnRef, $"Error processing callback: {ex.Message}", dto);
                return false;
            }
        }

        public async Task<PaginatedResultDto<PaymentHistoryDto>> GetPaymentHistoryAsync(int customerId, PaginationQueryDTO pagination)
        {
            var payments = await _unitOfWork.PaymentRepository.GetPaymentHistoryAsync(customerId, pagination.PageNumber, pagination.PageSize);
            
            var paymentDtos = payments.Items.Select(p => new PaymentHistoryDto
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                TransactionId = p.TransactionId,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                ProcessedAt = p.ProcessedAt
            }).ToList();

            return new PaginatedResultDto<PaymentHistoryDto>(paymentDtos, payments.TotalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<PaymentStatusDto> GetPaymentStatusAsync(string paymentId)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByPaymentIdAsync(paymentId);
            if (payment == null)
            {
                throw new NotFoundException($"Payment with ID {paymentId} not found");
            }

            return new PaymentStatusDto
            {
                PaymentId = payment.PaymentId,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                Amount = payment.Amount,
                CreatedAt = payment.CreatedAt,
                ProcessedAt = payment.ProcessedAt,
                Notes = payment.Notes
            };
        }

        public async Task<RefundResponseDto> ProcessRefundAsync(RefundRequestDto dto)
        {
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByPaymentIdAsync(dto.PaymentId);
                if (payment == null)
                {
                    return new RefundResponseDto
                    {
                        IsSuccess = false,
                        Message = "Payment not found",
                        RefundAmount = 0,
                        ProcessedAt = DateTime.UtcNow
                    };
                }

                if (payment.Status != PaymentStatus.Completed)
                {
                    return new RefundResponseDto
                    {
                        IsSuccess = false,
                        Message = "Only completed payments can be refunded",
                        RefundAmount = 0,
                        ProcessedAt = DateTime.UtcNow
                    };
                }

                // Update payment status
                payment.Status = PaymentStatus.Refunded;
                payment.Notes = $"Refunded by {dto.AdminUserId}. Reason: {dto.Reason}";
                await _unitOfWork.PaymentRepository.UpdateAsync(payment);

                // Create refund log
                var refundLog = new PaymentLog
                {
                    PaymentId = payment.Id,
                    Action = "Refund",
                    Status = PaymentStatus.Refunded.ToString(),
                    RequestData = JsonSerializer.Serialize(dto),
                    Notes = $"Refund processed by admin {dto.AdminUserId}",
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentLogRepository.AddAsync(refundLog);
                await _unitOfWork.SaveChangesAsync();

                return new RefundResponseDto
                {
                    IsSuccess = true,
                    Message = "Refund processed successfully",
                    RefundTransactionId = Guid.NewGuid().ToString(),
                    RefundAmount = dto.RefundAmount,
                    ProcessedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new RefundResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error processing refund: {ex.Message}",
                    RefundAmount = 0,
                    ProcessedAt = DateTime.UtcNow
                };
            }
        }

        public Task<bool> ValidateVNPaySignature(VNPayCallbackDto dto)
        {
            try
            {
                var parameters = dto.ToValidationDict();
                
                // Log signature validation details for debugging
                var filteredParams = parameters
                    .Where(x => x.Key != "vnp_SecureHash" && x.Key != "vnp_SecureHashType" && !string.IsNullOrEmpty(x.Value))
                    .OrderBy(x => x.Key)
                    .ToList();

                var queryString = string.Join("&", filteredParams.Select(x => $"{x.Key}={x.Value}")); 

                var computedHash = VNPayHelper.ComputeHmacSha512(_paymentConfig.VNPay.HashSecret, queryString);
                
                Console.WriteLine($"VNPay Signature Validation:");
                Console.WriteLine($"Query String: {queryString}");
                Console.WriteLine($"Hash Secret: {_paymentConfig.VNPay.HashSecret}");
                Console.WriteLine($"Computed Hash: {computedHash}");
                Console.WriteLine($"Received Hash: {dto.vnp_SecureHash}");
                Console.WriteLine($"Match: {dto.vnp_SecureHash?.Equals(computedHash, StringComparison.OrdinalIgnoreCase)}");
                
                return Task.FromResult(VNPayHelper.ValidateSignature(parameters, _paymentConfig.VNPay.HashSecret, dto.vnp_SecureHash));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating VNPay signature: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public string GenerateVNPayPaymentUrl(CreatePaymentDto dto, string paymentId, decimal amount, string descrip)
        {
            var parameters = new Dictionary<string, string>
            {
                {"vnp_Version", "2.1.0"},
                {"vnp_Command", "pay"},
                {"vnp_TmnCode", _paymentConfig.VNPay.TmnCode},
                {"vnp_Amount", VNPayHelper.FormatAmount(amount)},
                {"vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
                {"vnp_CurrCode", "VND"},
                {"vnp_IpAddr", "127.0.0.1"},
                {"vnp_Locale", "vn"},
                {"vnp_OrderInfo", descrip},
                {"vnp_OrderType", "other"},
                {"vnp_ReturnUrl", dto.ReturnUrl ?? _paymentConfig.VNPay.ReturnUrl},
                {"vnp_TxnRef", paymentId}
            };

            return VNPayHelper.CreateRequestUrl(_paymentConfig.VNPay.PaymentUrl, parameters, _paymentConfig.VNPay.HashSecret);
        }

        private async Task LogPaymentCallback(string paymentId, string message, VNPayCallbackDto dto)
        {
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByPaymentIdAsync(paymentId);
                if (payment != null)
                {
                    var errorLog = new PaymentLog
                    {
                        PaymentId = payment.Id,
                        Action = "CallbackError",
                        Status = "Error",
                        ResponseData = JsonSerializer.Serialize(dto),
                        Notes = message,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.PaymentLogRepository.AddAsync(errorLog);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the callback flow
                Console.WriteLine($"Error logging callback: {ex.Message}");
            }
        }
    }
}
