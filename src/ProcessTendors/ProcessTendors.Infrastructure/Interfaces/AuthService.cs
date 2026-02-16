using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProcessTendors.Application.Common.Interfaces.Service;
using ProcessTendors.Application.Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProcessTendors.Infrastructure.Interfaces
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Tuple<string, int>> GenerateToken(UserContextModel model)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, model.UserId.ToString()),
            new Claim(ClaimTypes.Name, model.Name),
            new Claim(ClaimTypes.Role, model.Role),
        });

            var token = new JwtSecurityToken(_configuration["JWT:Issuer"],
              _configuration["JWT:Audience"],
              claims: claimsIdentity.Claims,
              expires: DateTime.UtcNow.AddSeconds(Convert.ToInt32(_configuration["JWT:expirationTimeInSeconds"])),
              signingCredentials: credentials);

            return Tuple.Create(new JwtSecurityTokenHandler().WriteToken(token), Convert.ToInt32(_configuration["JWT:expirationTimeInSeconds"]));
        }

        public async Task<string> GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
