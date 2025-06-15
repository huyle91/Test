using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class TreatmentServiceRepository : ITreatmentServiceRepository
    {
        private readonly ApplicationDbContext _context;
        public TreatmentServiceRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<TreatmentService>> GetAllAsync() =>
            await _context.TreatmentServices.ToListAsync();

        public async Task<TreatmentService?> GetByIdAsync(int id) =>
            await _context.TreatmentServices.FindAsync(id);

        public async Task AddAsync(TreatmentService entity)
        {
            _context.TreatmentServices.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(TreatmentService entity)
        {
            _context.TreatmentServices.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var service = await GetByIdAsync(id);
            if (service == null) return false;
            _context.TreatmentServices.Remove(service);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsNameExistsAsync(string name)
        {
            return await _context.TreatmentServices.AnyAsync(s => s.Name == name);
        }
    }
}
