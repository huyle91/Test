using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Auth;
using InfertilityTreatment.Entity.DTOs.Common;
using FluentValidation;
using System.Security.Claims;

namespace InfertilityTreatment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IValidator<LoginRequestDto> loginValidator,
            IValidator<RegisterRequestDto> registerValidator,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _logger = logger;
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT tokens and user profile</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                // Validate request
                var validationResult = await _loginValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponseDto<LoginResponseDto>.CreateError("Validation failed", errors));
                }

                // Attempt login
                var result = await _authService.LoginAsync(request);
                
                _logger.LogInformation("User {Email} logged in successfully", request.Email);
                
                return Ok(ApiResponseDto<LoginResponseDto>.CreateSuccess(result, "Login successful"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Login failed for {Email}: {Error}", request.Email, ex.Message);
                return Unauthorized(ApiResponseDto<LoginResponseDto>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for {Email}", request.Email);
                return StatusCode(500, ApiResponseDto<LoginResponseDto>.CreateError("An error occurred during login"));
            }
        }

        /// <summary>
        /// Register new user account
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <returns>Registration result</returns>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponseDto<RegisterResponseDto>>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                // Validate request
                var validationResult = await _registerValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponseDto<RegisterResponseDto>.CreateError("Validation failed", errors));
                }

                // Attempt registration
                var result = await _authService.RegisterAsync(request);
                
                if (result.Success)
                {
                    _logger.LogInformation("User {Email} registered successfully", request.Email);
                    return Ok(ApiResponseDto<RegisterResponseDto>.CreateSuccess(result, "Registration successful"));
                }
                else
                {
                    _logger.LogWarning("Registration failed for {Email}: {Error}", request.Email, result.Message);
                    return BadRequest(ApiResponseDto<RegisterResponseDto>.CreateError(result.Message));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error for {Email}", request.Email);
                return StatusCode(500, ApiResponseDto<RegisterResponseDto>.CreateError("An error occurred during registration"));
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>New JWT tokens</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return BadRequest(ApiResponseDto<LoginResponseDto>.CreateError("Refresh token is required"));
                }

                var result = await _authService.RefreshTokenAsync(request);
                
                _logger.LogInformation("Token refreshed successfully");
                
                return Ok(ApiResponseDto<LoginResponseDto>.CreateSuccess(result, "Token refreshed successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Token refresh failed: {Error}", ex.Message);
                return Unauthorized(ApiResponseDto<LoginResponseDto>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh error");
                return StatusCode(500, ApiResponseDto<LoginResponseDto>.CreateError("An error occurred during token refresh"));
            }
        }

        /// <summary>
        /// Logout and revoke all refresh tokens for current user
        /// </summary>
        /// <returns>Logout result</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<object>>> Logout()
        {
            try
            {
                // Get user ID from JWT claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return BadRequest(ApiResponseDto<object>.CreateError("Invalid user token"));
                }

                await _authService.LogoutAsync(userId);
                
                _logger.LogInformation("User {UserId} logged out successfully", userId);
                
                return Ok(ApiResponseDto<object>.CreateSuccess(new { message = "Logout successful" }, "Logout successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                return StatusCode(500, ApiResponseDto<object>.CreateError("An error occurred during logout"));
            }
        }

        /// <summary>
        /// Get current user info from JWT token
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<ApiResponseDto<object>> GetCurrentUser()
        {
            try
            {
                var userInfo = new
                {
                    Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Name = User.FindFirst(ClaimTypes.Name)?.Value,
                    Email = User.FindFirst(ClaimTypes.Email)?.Value,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value
                };

                return Ok(ApiResponseDto<object>.CreateSuccess(userInfo, "User information retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get current user error");
                return StatusCode(500, ApiResponseDto<object>.CreateError("An error occurred while retrieving user information"));
            }
        }
    }
}
