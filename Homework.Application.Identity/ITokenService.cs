using Homework.Application.Identity.DataTransferObjects;

namespace Homework.Application.Identity
{
    public interface ITokenService
    {
        RefreshTokenResponse GenerateJwtRefreshToken(RefreshTokenRequest request);
        TokenGenerationResponse GenerateJwtToken(TokenGenerationRequest request);
    }
}
