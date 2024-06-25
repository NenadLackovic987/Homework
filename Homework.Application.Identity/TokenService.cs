using Microsoft.IdentityModel.Tokens;
using Homework.Application.Identity.DataTransferObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Homework.Application.Identity
{
    public class TokenService : ITokenService
    {
        private const string TokenKey = "zbSCTJpZmWhfMF7pPdw4gqJ722JqajnK";

        public RefreshTokenResponse GenerateJwtRefreshToken(RefreshTokenRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenKey);

            // Validate the refresh token
            var principal = tokenHandler.ValidateToken(request.JwtRefreshToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            if (principal.Identity is not ClaimsIdentity claimsIdentity
                || !claimsIdentity.IsAuthenticated
                || !claimsIdentity.HasClaim(c => c.Type == JwtRegisteredClaimNames.Jti))
            {
                // Invalid or expired refresh token
                throw new SecurityTokenException("Invalid refresh token");
            }

            // Generate a new access token and take previous claims
            var email = claimsIdentity.Claims.First(x => x.Type ==  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, email!),
                new(JwtRegisteredClaimNames.Email, email!)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(request.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var newAccessToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwtNewAccessToken = tokenHandler.WriteToken(newAccessToken);

            var response = new RefreshTokenResponse
            {
                JwtToken = jwtNewAccessToken
            };

            return response;
        }

        public TokenGenerationResponse GenerateJwtToken(TokenGenerationRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenKey);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, request.Email!),
                new(JwtRegisteredClaimNames.Email, request.Email!),
                new(JwtRegisteredClaimNames.Name, request.FirstName!),
                new(JwtRegisteredClaimNames.FamilyName, request.LastName!),
                new(JwtRegisteredClaimNames.UniqueName, request.Id!),
                new("Position", request.Position!),
                new("NickName", request.NickName!)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(request.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            // Claims for refresh token
            var refreshTokenClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, request.Email!),
                new(JwtRegisteredClaimNames.Email, request.Email!),
                new(JwtRegisteredClaimNames.Name, request.FirstName!),
                new(JwtRegisteredClaimNames.FamilyName, request.LastName!),
                new(JwtRegisteredClaimNames.UniqueName, request.Id!),
                new("Position", request.Position!),
                new("NickName", request.NickName!)
            };

            var refreshTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(refreshTokenClaims),
                Expires = DateTime.UtcNow.Add(request.RefreshTokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var refreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);
            var jwtRefreshToken = tokenHandler.WriteToken(refreshToken);


            var response = new TokenGenerationResponse() { JwtToken = jwt, JwtRefreshToken = jwtRefreshToken };

            return response;
        }
    }
}
