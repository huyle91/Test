using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfertilityTreatment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult GetPublic()
        {
            return Ok(new { message = "This is a public endpoint", timestamp = DateTime.UtcNow });
        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult GetProtected()
        {
            return Ok(new
            {
                message = "This is a protected endpoint",
                user = User.Identity?.Name,
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("admin")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAdmin()
        {
            return Ok(new
            {
                message = "This is an admin-only endpoint",
                user = User.Identity?.Name,
                timestamp = DateTime.UtcNow
            });
        }
    }
}