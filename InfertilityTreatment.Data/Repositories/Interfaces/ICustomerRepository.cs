using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<Customer?> GetWithUserAsync(int customerId);
        Task<Customer?> GetByUserIdAsync(int userId);
        Task<Customer?> UpdateCustomerProfileAsync(int customerId, CustomerProfileDto customerProfileDto);
        Task<Customer> GetWithMedicalHistoryAsync(int customerId);
        Task<Customer?> UpdateMedicalHistoryAsync(int userId,string medicalHistory);
        Task<PaginatedResultDto<Customer?>> GetCustomers(CustomerFilterDto filter);

        Task<IEnumerable<Customer>> GetAllWithUserAsync();
    }
}