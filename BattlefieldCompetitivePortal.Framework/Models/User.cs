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
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public int? TeamId { get; set; }
        public PlayerRole? PlayerRole { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
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
