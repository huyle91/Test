using InfertilityTreatment.Entity.DTOs.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IPaymentService
    {
        // Prepare for BE021: Payment Gateway Integration
        Task<PaymentResponseDto> CreateVNPayPaymentAsync(CreatePaymentDto dto);
        Task<PaymentResponseDto> CreateMomoPaymentAsync(CreatePaymentDto dto);
        Task<bool> HandleVNPayCallbackAsync(VNPayCallbackDto dto);
        Task<bool> HandleMomoCallbackAsync(MomoCallbackDto dto);
        Task<List<PaymentDto>> GetPaymentHistoryAsync(int customerId);
    }
}
