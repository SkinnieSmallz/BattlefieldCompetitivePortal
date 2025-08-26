using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BattlefieldCompetitivePortal.Framework.Models;
using BattlefieldCompetitivePortal.Framework.Services;

namespace BattlefieldCompetitivePortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _userService.ValidateUser(request.Username, request.Password);
                if (user == null)
                    return Unauthorized(new { message = "Invalid username or password" });

                // Generate JWT token here if using JWT
                // var token = GenerateJwtToken(user);

                return Ok(new AuthenticationResponse
                {
                    User = user,
                    // Token = token,
                    Success = true,
                    Message = "Authentication successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Authentication failed", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user", error = ex.Message });
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Name = request.Name,
                    Surname = request.Surname,
                    ContactNumber = request.ContactNumber,
                    PasswordHash = request.Password, // Will be hashed in service
                    Role = UserRole.Player,
                    TeamId = null,
                    PlayerRole = null,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                var success = await _userService.CreateUser(user);
                if (success)
                    return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);

                return BadRequest(new { message = "Failed to create user" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user", error = ex.Message });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                // Get user ID from JWT token claims or session
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var user = await _userService.GetUserById(userId);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving profile", error = ex.Message });
            }
        }
    }

    // DTOs for API requests/responses
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ContactNumber { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public int? TeamId { get; set; }
        public PlayerRole? PlayerRole { get; set; }
    }

    public class AuthenticationResponse
    {
        public User User { get; set; }
        public string Token { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
