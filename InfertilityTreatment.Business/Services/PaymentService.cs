using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Stub implementations
        public async Task<PaymentResponseDto> CreateVNPayPaymentAsync(CreatePaymentDto dto)
        {
            // TODO: Implement in BE021
            throw new NotImplementedException("Will be implemented in BE021");
        }

        public async Task<PaymentResponseDto> CreateMomoPaymentAsync(CreatePaymentDto dto)
        {
            // TODO: Implement in BE021
            throw new NotImplementedException("Will be implemented in BE021");
        }

        public async Task<bool> HandleVNPayCallbackAsync(VNPayCallbackDto dto)
        {
            // TODO: Implement in BE021
            throw new NotImplementedException("Will be implemented in BE021");
        }

        public async Task<bool> HandleMomoCallbackAsync(MomoCallbackDto dto)
        {
            // TODO: Implement in BE021
            throw new NotImplementedException("Will be implemented in BE021");
        }

        public async Task<List<PaymentDto>> GetPaymentHistoryAsync(int customerId)
        {
            // TODO: Implement in BE021
            throw new NotImplementedException("Will be implemented in BE021");
        }
    }
}
