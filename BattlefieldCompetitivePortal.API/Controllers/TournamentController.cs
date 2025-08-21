using BattlefieldCompetitivePortal.Framework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BattlefieldCompetitivePortal.Framework.Security.AuthenticationHelper;

namespace BattlefieldCompetitivePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TournamentController : ControllerBase
    {
        private readonly TournamentService _tournamentService;

        public TournamentController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet]
        public IActionResult GetTournaments()
        {
            try
            {
                var tournaments = _tournamentService.GetAllTournaments();
                return Ok(tournaments);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public IActionResult CreateTournament([FromBody] Tournament tournament)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();
                tournament.CreatedBy = userId;

                var created = _tournamentService.CreateTournament(tournament);
                return CreatedAtAction(nameof(GetTournaments),
                    new { id = created.TournamentId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create tournament" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId");
            return int.Parse(userIdClaim.Value);
        }
    }
}
