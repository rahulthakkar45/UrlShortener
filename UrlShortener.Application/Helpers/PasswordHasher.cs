using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UrlShortener.Domain.Entitites;

namespace UrlShortener.Application.Helpers
{
    public static class PasswordHasher
    {
        private static readonly PasswordHasher<User> hasher = new();
        public static string Hash(User user, string password)
        {
            return hasher.HashPassword(user, password);
        }
        public static bool VerifyPassword(User user, string hashPassword, string password) 
        {
            var result = hasher.VerifyHashedPassword(user, hashPassword, password);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
