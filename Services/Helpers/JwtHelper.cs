using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Config;
using Models.Entities;

namespace Services.Helpers
{
    public class JwtHelper
    {
        private readonly JwtConfig _jwtConfig;

        public JwtHelper(IOptions<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
            ValidateConfig();
        }

        private void ValidateConfig()
        {
            if (string.IsNullOrEmpty(_jwtConfig.SecretKey))
                throw new ArgumentNullException(nameof(_jwtConfig.SecretKey), ErrorMessages.UnconfiguredSecretKey);

            if (string.IsNullOrEmpty(_jwtConfig.Issuer))
                throw new ArgumentNullException(nameof(_jwtConfig.Issuer), ErrorMessages.UnconfiguredIssuer);
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("role", user.Role.ToString()),
        new Claim("alias", user.Alias)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}