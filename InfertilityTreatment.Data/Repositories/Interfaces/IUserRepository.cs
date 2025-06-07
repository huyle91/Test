using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> FindByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserWithProfileAsync(int userId);
        Task<User?> GetUserWithRefreshTokensAsync(int userId);
    }
}