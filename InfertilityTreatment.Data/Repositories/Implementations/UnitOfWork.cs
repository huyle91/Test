using Microsoft.EntityFrameworkCore.Storage;
using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Data.Repositories.Implementations;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Repository instances
        private IUserRepository? _users;
        private ICustomerRepository? _customers;
        private ITreatmentCycleRepository? _treatmentCycles;
        private IRefreshTokenRepository? _refreshTokens;
        private IAppointmentRepository? _appointments;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lazy loading repositories
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
        public ITreatmentCycleRepository TreatmentCycles => _treatmentCycles ??= new TreatmentCycleRepository(_context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
        public IAppointmentRepository Appointments => _appointments ??= new AppointmentRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
