using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new user with Manager or Doctor role
        /// </summary>
        /// <param name="request">User creation request</param>
        /// <returns>Created user information</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<UserResponse>>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponseDto<UserResponse>.CreateError("Validation failed", errors));
                }

                // Validate role - only Manager and Doctor allowed
                if (request.Role != UserRole.Manager && request.Role != UserRole.Doctor)
                {
                    return BadRequest(ApiResponseDto<UserResponse>.CreateError("Role must be either Manager or Doctor"));
                }

                var userResponse = await _userService.CreateUserAsync(request);
                
                return Ok(ApiResponseDto<UserResponse>.CreateSuccess(userResponse, "User created successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided for user creation");
                return BadRequest(ApiResponseDto<UserResponse>.CreateError(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict occurred during user creation");
                return Conflict(ApiResponseDto<UserResponse>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating user");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponseDto<UserResponse>.CreateError("An error occurred while creating the user"));
            }
        }
    }
}