using InfertilityTreatment.Entity.DTOs.Users;

namespace InfertilityTreatment.Entity.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserProfileDto User { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
