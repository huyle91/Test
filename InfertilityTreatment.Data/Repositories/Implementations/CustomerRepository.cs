using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {

        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetWithUserAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == customerId && c.IsActive);
        }

        public async Task<Customer?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);
        }

        public async Task<Customer?> GetWithMedicalHistoryAsync(int customerId)
        {
            //return await _dbSet
            //    .Include(c => c.User)
            //    .Include(c => c.TreatmentCycles)
            //    .FirstOrDefaultAsync(c => c.Id == customerId && c.IsActive);
            return await GetByIdWithIncludeAsync(customerId, c => c.User, c => c.TreatmentCycles);
        }

        public async Task<Customer?> UpdateCustomerProfileAsync(int customerId, CustomerProfileDto customerProfileDto)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId && c.IsActive);

            if (customer == null)
            {
                return null;
            }
            customer.Address = customerProfileDto.Address;
            customer.EmergencyContactName = customerProfileDto.EmergencyContactName;
            customer.EmergencyContactPhone = customerProfileDto.EmergencyContactPhone;
            customer.MedicalHistory = customerProfileDto.MedicalHistory;
            customer.MaritalStatus = customerProfileDto.MaritalStatus;
            customer.Occupation = customerProfileDto.Occupation;

            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer?> UpdateMedicalHistoryAsync(int userId, string medicalHistory)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == userId && c.IsActive);

            if (customer == null)
            {
                return null;
            }
            customer.MedicalHistory = medicalHistory;
            await _context.SaveChangesAsync();

            return customer;
        }
    }
}
