using BattlefieldCompetitivePortal.Framework.Models;
using BattlefieldCompetitivePortal.Framework.Services;
//using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;

namespace BattlefieldCompetitivePortal.MVC.Controllers
{
    [Authorize]
    public class TournamentController : BaseController
    {
        private readonly TournamentService _tournamentService;

        public ActionResult Index()
        {
            var tournaments = _tournamentService.GetAllTournaments();
            ViewBag.CanCreateTournament = CurrentUser.Role == UserRole.Admin;
            ViewBag.CanRegister = CurrentUser.Role == UserRole.Captain &&
                                 CurrentUser.TeamId.HasValue;

            return View(tournaments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tournament tournament)
        {
            var roleCheck = RequireRole(UserRole.Admin);
            if (roleCheck != null) return roleCheck;

            if (!ModelState.IsValid)
                return View(tournament);

            tournament.CreatedBy = CurrentUser.UserId;
            _tournamentService.CreateTournament(tournament);

            TempData["Success"] = "Tournament created successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(int tournamentId)
        {
            if (CurrentUser.Role != UserRole.Captain || !CurrentUser.TeamId.HasValue)
                return Json(new { success = false, message = "Only team captains can register" });

            try
            {
                _tournamentService.RegisterTeam(tournamentId, CurrentUser.TeamId.Value);
                return Json(new { success = true, message = "Team registered successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
