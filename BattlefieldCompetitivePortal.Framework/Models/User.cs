using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public int? TeamId { get; set; }
        public PlayerRole? PlayerRole { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public enum UserRole
    {
        Admin = 1,
        Captain = 2,
        Player = 3,
    }

    public enum PlayerRole
    {
        Starter = 1,
        Sub = 2,
    }
}
