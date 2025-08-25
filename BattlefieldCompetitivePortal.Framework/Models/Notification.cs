using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int NotificationType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public enum NotificationType
    {
        TeamJoinRequest = 0,
        TeamJoinApproved = 1,
        TeamInvite = 2,
        TournamentJoinRequest = 3,
        TournamentJoinApproved = 4,
        ScrimJoinRequest = 5,
        ScrimJoinApproved = 6,
        MatchReminder = 7,
        General = 8
    }
}
