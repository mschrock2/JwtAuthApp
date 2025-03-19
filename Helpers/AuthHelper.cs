using JwtAuthApp.Controllers;
using JwtAuthApp.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtAuthApp.Helpers
{
    public interface IAuthHelper
    {
        string GetSalt();
        string GetHashedString(string value, string salt);
        string GenerateJWTToken(string username, string scopes);
        string? ValidatePassword(string password);
    }

    public class AuthHelper : IAuthHelper
    {
        private static readonly char[] requiredCharacters = "!@#$%^&*()[]{};:,./<>?".ToCharArray();
        private JwtSettings jwtSettings;

        public AuthHelper(JwtSettings jwtSettings)
        {
            this.jwtSettings = jwtSettings;
        }

        public string GetSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            return Convert.ToBase64String(salt);
        }

        public string GetHashedString(string value, string salt)
        {
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: value,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        public string GenerateJWTToken(string username, string scopes)
        {
            // Configure JWT authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim("Scopes", "User:crud")
            };
            var jwtToken = new JwtSecurityToken(
                audience: jwtSettings.Audience,
                issuer: jwtSettings.Issuer,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(jwtSettings.SecretKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public string? ValidatePassword(string password)
        {
            if (password.Length < 6)
            {
                return "Password must have at least 6 characters.";
            }
            if (!password.Any(char.IsUpper))
            {
                return "Password must have at least one uppercase character.";
            }
            if (!password.Any(char.IsLower))
            {
                return "Password must have at least one lowercase character.";
            }
            if (!password.Any(char.IsDigit))
            {
                return "Password must have at least one number.";
            }
            if (!password.Any(ch => requiredCharacters.Contains(ch)))
            {
                return "Password must have at least one special character (!@#$%^&*()[]{};:,./<>?).";
            }
            return null;
        }

    }

}
