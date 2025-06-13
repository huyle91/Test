using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Auth;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InfertilityTreatment.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponseDto<UserProfileDto>>> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userService.GetProfileAsync(int.Parse(userId));
                return Ok(ApiResponseDto<UserProfileDto>.CreateSuccess(user, "Profile retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseDto<string>.CreateError("An error occurred while retrieving the profile."));
            }   
        }
        [HttpPut("profile")]
        public async Task<ActionResult<string>> UpdateProfile([FromBody]UpdateProfileDto updateProfileDto)
        {
            try {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var updatedProfile = await _userService.UpdateProfileAsync(int.Parse(userId), updateProfileDto);
                return Ok(ApiResponseDto<string>.CreateSuccess(null, updatedProfile));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseDto<string>.CreateError("An error occurred while updating the profile."));
            }

        }
        [HttpPut("change-password")]
        public async Task<ActionResult<string>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _userService.ChangePasswordAsync(int.Parse(userId), changePasswordDto);
                return Ok(ApiResponseDto<string>.CreateSuccess(null, result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseDto<string>.CreateError("An error occurred while changing the password."));
            }
        }
    }
}
