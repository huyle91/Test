namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Repository Properties
        IUserRepository Users { get; }
        ICustomerRepository Customers { get; }
        ITreatmentCycleRepository TreatmentCycles { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IAppointmentRepository Appointments { get; } // Added
        IDoctorRepository Doctors { get; }
        IDoctorScheduleRepository DoctorSchedules { get; }

        // Transaction Methods
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}