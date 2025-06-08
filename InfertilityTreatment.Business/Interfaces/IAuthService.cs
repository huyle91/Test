using InfertilityTreatment.Entity.DTOs.Auth;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task LogoutAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
    }
}
