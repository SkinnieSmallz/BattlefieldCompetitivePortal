using BattlefieldCompetitivePortal.Framework.Models;
using BattlefieldCompetitivePortal.Framework.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BattlefieldCompetitivePortal.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userService.ValidateUser(model.Username, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                // Create claims for cookie authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.UserId.ToString())
                };

                if (user.TeamId.HasValue)
                    claims.Add(new Claim("TeamId", user.TeamId.Value.ToString()));

                if (user.PlayerRole.HasValue)
                    claims.Add(new Claim("PlayerRole", user.PlayerRole.Value.ToString()));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(model.RememberMe ? 30 : 1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Login failed. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = model.Password, // Will be hashed in service
                    Role = model.Role,
                    TeamId = model.TeamId,
                    PlayerRole = model.PlayerRole,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                var success = await _userService.CreateUser(user);
                if (success)
                {
                    TempData["SuccessMessage"] = "User created successfully";
                    return RedirectToAction("Details", new { id = user.UserId });
                }

                ModelState.AddModelError("", "Failed to create user");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating user: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                // If no ID provided, show current user's profile
                var userId = id ?? GetCurrentUserId();

                // Only allow users to view their own profile unless they're admin
                if (!User.IsInRole("Admin") && userId != GetCurrentUserId())
                    return Forbid();

                var user = await _userService.GetUserById(userId);
                if (user == null)
                    return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error retrieving user details";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            return await Details(GetCurrentUserId());
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }
    }

    // ViewModels for MVC
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public int? TeamId { get; set; }

        public PlayerRole? PlayerRole { get; set; }
    }
}