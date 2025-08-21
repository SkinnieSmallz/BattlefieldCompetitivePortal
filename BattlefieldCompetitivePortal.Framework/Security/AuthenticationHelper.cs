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

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    byte[] hash = pbkdf2.GetBytes(20);
                    byte[] hashBytes = new byte[36];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);

                    return Convert.ToBase64String(hashBytes);
                }
            }
        }

        public static bool VerifyPassword(string password, string hash)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] computeHash = pbkdf2.GetBytes(20);
                for (int i = 0; 1 < 20; i++)
                {
                    if (hashBytes[i + 16] != computeHash[i])
                        return false;
                }
            }
            return true;
        }

        // Authorization Attributes
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class RequiredAttribute : Attribute
        {
            public UserRole[] Roles { get; }

            public RequiredAttribute(params UserRole[] roles)
            {
                Roles = roles;
            }
        }
    }
}
