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
   public class TreatmentPackageRepository : ITreatmentPackageRepository
    {
        private readonly ApplicationDbContext _context;
        public TreatmentPackageRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<TreatmentPackage>> GetAllAsync() =>
            await _context.TreatmentPackages.ToListAsync();

        public async Task<TreatmentPackage?> GetByIdAsync(int id) =>
            await _context.TreatmentPackages.FindAsync(id);

        public async Task AddAsync(TreatmentPackage entity)
        {
            _context.TreatmentPackages.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(TreatmentPackage entity)
        {
            _context.TreatmentPackages.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pkg = await GetByIdAsync(id);
            if (pkg == null) return false;
            _context.TreatmentPackages.Remove(pkg);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> IsPackageNameExistsAsync(string packageName)
        {
            return await _context.TreatmentPackages.AnyAsync(p => p.PackageName == packageName);
        }
    }
}
