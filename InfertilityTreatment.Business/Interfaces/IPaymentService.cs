using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IPaymentService
    {
        // VNPay Integration
        Task<PaymentResponseDto> CreateVNPayPaymentAsync(CreatePaymentDto dto);
        Task<bool> HandleVNPayCallbackAsync(VNPayCallbackDto dto);
        
        // Payment Management
        Task<PaginatedResultDto<PaymentHistoryDto>> GetPaymentHistoryAsync(int customerId, PaginationQueryDTO pagination);
        Task<PaymentStatusDto> GetPaymentStatusAsync(string paymentId);
        Task<RefundResponseDto> ProcessRefundAsync(RefundRequestDto dto);
        
        // Utility Methods
        Task<bool> ValidateVNPaySignature(VNPayCallbackDto dto);
        string GenerateVNPayPaymentUrl(CreatePaymentDto dto, string paymentId, decimal amount, string descrip);
    }
}
