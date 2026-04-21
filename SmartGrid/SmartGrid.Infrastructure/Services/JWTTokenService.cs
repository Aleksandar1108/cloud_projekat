using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SmartGrid.Application.Interfaces;
using SmartGrid.Domain.Models;
using System.Security.Claims;
using System.Text;

namespace SmartGrid.Infrastructure.Services
{
    public class JWTTokenService : IJwtTokenService
    {
        private readonly string _key;

        public JWTTokenService(IConfiguration config)
        {
            _key = config["Jwt:Key"]
                ?? throw new ArgumentNullException("Jwt:Key missing");
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("id", user.Id.Value.ToString()),
                new Claim("username", user.Email.Value),
                new Claim("role", user.Role.ToString())
            };

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = "SmartGrid",
                Audience = "SmartGrid",
                SigningCredentials = creds
            };

            var handler = new JsonWebTokenHandler();

            return handler.CreateToken(descriptor);
        }
    }
}