using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
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

        public async Task<Customer> GetWithMedicalHistoryAsync(int customerId)
        {
            var customer = await GetByIdWithIncludeAsync(customerId, c => c.User, c => c.TreatmentCycles);

            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }

            return customer;
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
        public async Task<PaginatedResultDto<Customer?>> GetCustomers(CustomerFilterDto filter)
        {
            var query = _context.Customers.AsQueryable();

            var totalCount = await query.CountAsync();

            var pagedUsers = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResultDto<Customer?>(pagedUsers, totalCount, filter.PageNumber, filter.PageSize);
        }
    }
}
