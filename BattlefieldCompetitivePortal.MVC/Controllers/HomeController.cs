using System.Diagnostics;
using BattlefieldCompetitivePortal.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using BattlefieldCompetitivePortal.Framework.Services;

namespace BattlefieldCompetitivePortal.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;

        public HomeController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get real statistics from your database
                var stats = await GetDashboardStats();

                // Pass data to the view
                ViewBag.ActivePlayers = stats.ActivePlayers;
                ViewBag.TotalTeams = stats.TotalTeams;
                ViewBag.CompletedTournaments = stats.CompletedTournaments;
                ViewBag.TotalPrizeMoney = stats.TotalPrizeMoney;

                // Optional: Pass user info if authenticated
                if (User.Identity.IsAuthenticated)
                {
                    var userId = GetCurrentUserId();
                    var currentUser = await _userService.GetUserById(userId);
                    ViewBag.CurrentUser = currentUser;
                }

                return View();
            }
            catch (Exception ex)
            {
                // Log the error (you should add proper logging)
                // For now, return view with default values
                ViewBag.ActivePlayers = 0;
                ViewBag.TotalTeams = 0;
                ViewBag.CompletedTournaments = 0;
                ViewBag.TotalPrizeMoney = 0;

                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        // Helper method to get current user ID from claims
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        // Get dashboard statistics
        private async Task<DashboardStats> GetDashboardStats()
        {
            // You'll need to create these methods in your services
            // For now, returning mock data - replace with actual database calls

            return new DashboardStats
            {
                ActivePlayers = await GetActivePlayersCount(),
                TotalTeams = await GetTotalTeamsCount(),
                CompletedTournaments = await GetCompletedTournamentsCount(),
                TotalPrizeMoney = await GetTotalPrizeMoney()
            };
        }

        private async Task<int> GetActivePlayersCount()
        {
            try
            {
                // TODO: Add this method to your UserService
                // return await _userService.GetActiveUsersCount();

                // Mock implementation for now
                await Task.Delay(1); // Simulate async call
                return 2847;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<int> GetTotalTeamsCount()
        {
            try
            {
                // TODO: Add TeamService and implement this
                // return await _teamService.GetTotalTeamsCount();

                await Task.Delay(1);
                return 156;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<int> GetCompletedTournamentsCount()
        {
            try
            {
                // TODO: Add TournamentService and implement this
                // return await _tournamentService.GetCompletedCount();

                await Task.Delay(1);
                return 89;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<decimal> GetTotalPrizeMoney()
        {
            try
            {
                // TODO: Add TournamentService and implement this
                // return await _tournamentService.GetTotalPrizeMoney();

                await Task.Delay(1);
                return 45000m;
            }
            catch
            {
                return 0;
            }
        }

        // DTO for dashboard statistics
        public class DashboardStats
        {
            public int ActivePlayers { get; set; }
            public int TotalTeams { get; set; }
            public int CompletedTournaments { get; set; }
            public decimal TotalPrizeMoney { get; set; }
        }
    }
}
