using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Models
{
    public class Tournament
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public int MaxTeams { get; set; }
        public TournamentStatus Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public enum TournamentStatus
    {
        Registration = 1,
        Pending = 2,
        Ongoing = 3,
        Completed = 4,
        Cancelled = 5,
    }
}
