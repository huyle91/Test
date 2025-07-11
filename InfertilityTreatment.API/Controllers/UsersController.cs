using Microsoft.AspNetCore.Mvc;
using Test.Models.DTOs;
using Test.Services;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Data = ModelState,
                        Message = "Validation failed"
                    });
                }

                var userResponse = await _userService.CreateUserAsync(request);

                return Ok(new ApiResponse<UserResponse>
                {
                    Success = true,
                    Data = userResponse,
                    Message = "User created successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse<object>
                {
                    Success = false,
                    Data = null,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Data = null,
                    Message = "An error occurred while creating the user"
                });
            }
        }
    }
}