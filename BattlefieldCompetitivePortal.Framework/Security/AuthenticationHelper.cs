using BattlefieldCompetitivePortal.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Security
{
    public static class AuthenticationHelper
    {
        public static string HashPassword(string password)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);
                    byte[] hashBytes = new byte[16 + 32];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 32);

                    return Convert.ToBase64String(hashBytes);
                }
            }
        }

        public static bool VerifyPassword(string password, string hash)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] computedHash = pbkdf2.GetBytes(32);

                bool isMatch = true;
                for (int i = 0; i < 32; i++)
                {
                    isMatch &= (hashBytes[i + 16] == computedHash[i]);
                }
                    return isMatch;
            }
        }

        // Authorization Attributes
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class RequireRoleAttribute : Attribute
        {
            public UserRole[] Roles { get; }

            public RequireRoleAttribute(params UserRole[] roles)
            {
                Roles = roles ?? throw new ArgumentNullException(nameof(roles));
            }
        }
    }
}
