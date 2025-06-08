using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId);
        Task RevokeTokenAsync(string token);
        Task RevokeAllUserTokensAsync(int userId);
    }
}
