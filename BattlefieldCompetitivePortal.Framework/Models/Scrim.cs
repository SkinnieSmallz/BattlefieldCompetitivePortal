using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Models
{
    public class Scrim
    {
        public int ScrimId { get; set; }
        public int Team1Id { get; set; }
        public int? Team2Id { get; set; }
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public string RequestedByName { get; set; }
        public DateTime ScheduledDate { get; set; }
        public ScrimStatus Status { get; set; }
        public int? WinnerTeamId { get; set; }
        public int RequestedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public enum ScrimStatus
    {
        Pending = 1,
        Approved = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5,
    }
}
