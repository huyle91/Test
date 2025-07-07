using InfertilityTreatment.API.Attributes;
using InfertilityTreatment.Business.Exceptions;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfertilityTreatment.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new VNPay payment
        /// </summary>
        /// <param name="dto">Payment creation data</param>
        /// <returns>Payment response with payment URL</returns>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _paymentService.CreateVNPayPaymentAsync(dto);

                
                return Ok(new { success = true, data = result });
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = "Internal server error", data= ex.Message });
            }
        }

        /// <summary>
        /// Handle VNPay callback/IPN
        /// </summary>
        /// <param name="dto">VNPay callback data</param>
        /// <returns>Success status</returns>
        [HttpGet("vnpay/callback")]
        [AllowAnonymous]
        [SkipModelValidation] // Skip model validation for VNPay callback
        public async Task<IActionResult> VNPayCallback([FromQuery] VNPayCallbackDto dto)
        {
            try
            {
                _logger.LogInformation("Received VNPay callback for transaction {TxnRef}", dto.vnp_TxnRef);
                
                // Skip model validation for VNPay callback since VNPay controls what data is sent
                // We'll validate the required fields in the service layer
                
                var result = await _paymentService.HandleVNPayCallbackAsync(dto);
                
                if (result)
                {
                    _logger.LogInformation("VNPay callback processed successfully for transaction {TxnRef}", dto.vnp_TxnRef);
                    return Ok(new { RspCode = "00", Message = "Success" });
                }
                else
                {
                    _logger.LogWarning("VNPay callback failed for transaction {TxnRef}", dto.vnp_TxnRef);
                    return Ok(new { RspCode = "99", Message = "Failed"});
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay callback for transaction {TxnRef}", dto.vnp_TxnRef);
                 return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                }); ;
            }
        }

        /// <summary>
        /// Get payment history for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated payment history</returns>
        [HttpGet("history/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentHistory(int customerId, [FromQuery] PaginationQueryDTO pagination)
        {
            try
            {
                pagination.PageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
                pagination.PageSize = pagination.PageSize <= 0 ? 100 : pagination.PageSize;
                if (pagination.PageSize > 100) pagination.PageSize = 100;
                if (pagination.PageNumber < 1) pagination.PageNumber = 1;
                var result = await _paymentService.GetPaymentHistoryAsync(customerId, pagination);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment history for customer {CustomerId}", customerId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get payment status by payment ID
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>Payment status information</returns>
        [HttpGet("status/{paymentId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentStatus(string paymentId)
        {
            try
            {
                var result = await _paymentService.GetPaymentStatusAsync(paymentId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status for payment {PaymentId}", paymentId);
                
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(new { success = false, message = "Payment not found" });
                }
                
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Process refund (Admin only)
        /// </summary>
        /// <param name="dto">Refund request data</param>
        /// <returns>Refund result</returns>
        [HttpPost("refund")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessRefund([FromBody] RefundRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _paymentService.ProcessRefundAsync(dto);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Refund processed successfully for payment {PaymentId} by admin {AdminUserId}", 
                        dto.PaymentId, dto.AdminUserId);
                    return Ok(new { success = true, data = result });
                }
                else
                {
                    _logger.LogWarning("Refund failed for payment {PaymentId}: {Message}", dto.PaymentId, result.Message);
                    return BadRequest(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment {PaymentId}", dto.PaymentId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Validate VNPay return URL (for frontend)
        /// </summary>
        /// <param name="dto">VNPay callback data from return URL</param>
        /// <returns>Payment validation result</returns>
        [HttpGet("vnpay/return")]
        [AllowAnonymous]
        [SkipModelValidation] // Skip model validation for VNPay return
        public async Task<IActionResult> VNPayReturn([FromQuery] VNPayCallbackDto dto)
        {
            try
            {
                var isValidSignature = await _paymentService.ValidateVNPaySignature(dto);
                var paymentStatus = await _paymentService.GetPaymentStatusAsync(dto.vnp_TxnRef);

                return Ok(new 
                { 
                    success = true, 
                    isValidSignature = isValidSignature,
                    paymentStatus = paymentStatus,
                    vnpResponseCode = dto.vnp_ResponseCode,
                    isPaymentSuccess = dto.vnp_ResponseCode == "00" && dto.vnp_TransactionStatus == "00"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating VNPay return for transaction {TxnRef}", dto.vnp_TxnRef);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}
