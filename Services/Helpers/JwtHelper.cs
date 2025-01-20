using Microsoft.IdentityModel.Tokens;
using Models.DTOs;
using Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace Services.Helpers
{
    public class JwtHelper
    {
        private readonly JwtConfig _jwtConfig;

        public JwtHelper(IOptions<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        public string GenerateToken(UserDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.Email))
                throw new ArgumentNullException(nameof(userDto.Email), "Email cannot be null.");

            if (string.IsNullOrEmpty(userDto.Role))
                throw new ArgumentNullException(nameof(userDto.Role), "The role cannot be null.");

            if (string.IsNullOrEmpty(_jwtConfig.SecretKey))
                throw new ArgumentNullException(nameof(_jwtConfig.SecretKey), "The JWT key cannot be null.");

            if (string.IsNullOrEmpty(_jwtConfig.Issuer))
                throw new ArgumentNullException(nameof(_jwtConfig.Issuer), "The JWT issuer cannot be null.");

            if (string.IsNullOrEmpty(_jwtConfig.Audience))
                throw new ArgumentNullException(nameof(_jwtConfig.Audience), "JWT audience cannot be null.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userDto.Email),
            new Claim(JwtRegisteredClaimNames.Email, userDto.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, userDto.Role)
        };

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
