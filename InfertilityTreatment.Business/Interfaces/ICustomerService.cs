using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDetailDto> GetCustomerProfileAsync(int customerId);
        Task<string> UpdateCustomerProfileAsync(int userId, CustomerProfileDto updateProfileDto);
        Task<string> UpdateMedicalHistoryAsync(int customerId, string medicalHistory);
        Task<PaginatedResultDto<CustomerProfileDto>> GetCustomersAsync(CustomerFilterDto filter);
    }
}
