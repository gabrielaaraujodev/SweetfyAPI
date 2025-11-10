using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SweetfyAPI.TokenService
{
    public class TokenService : ITokenService
    {
        public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config)
        {
            var key = _config.GetSection("JWT").GetValue<string>("SecretKey") ??
                throw new InvalidOperationException("JWT:SecretKey is not configured.");

            var privateKey = Encoding.UTF8.GetBytes(key);

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey),
                SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_config.GetSection("JWT").GetValue<double>("TokenValidityInMinutes")),
                Audience = _config.GetSection("JWT").GetValue<string>("ValidAudience"),
                Issuer = _config.GetSection("JWT").GetValue<string>("ValidIssuer"),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            return token; 
        }

        public string GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[128];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(secureRandomBytes);

            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            return refreshToken;
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, IConfiguration _config)
        {
            var secretKey = _config.GetSection("JWT").GetValue<string>("SecretKey")
                            ?? throw new InvalidOperationException("JWT:SecretKey is not configured.");

            var jwtAudience = _config.GetSection("JWT").GetValue<string>("ValidAudience")
                           ?? throw new InvalidOperationException("JWT:ValidAudience is not configured.");

            var jwtIssuer = _config.GetSection("JWT").GetValue<string>("ValidIssuer")
                         ?? throw new InvalidOperationException("JWT:ValidIssuer is not configured.");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (SecurityTokenException)
            {
                return null;
            }
        }
    }
}