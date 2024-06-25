using Microsoft.Extensions.Configuration;
using Homework.Application.Identity;
using Homework.Application.Identity.DataTransferObjects;
using Homework.Common;
using Homework.Common.Dto;

namespace Homework.Application.Queries.Tokens
{
    public class GetJwtRefreshTokenQuery : IRequest<BaseResult<GetJwtRefreshTokenQueryResult>>
    {
        public string? JwtRefreshToken { get; set; }
    }

    public class GetJwtRefreshTokenQueryHandler : IRequestHandler<GetJwtRefreshTokenQuery, BaseResult<GetJwtRefreshTokenQueryResult>>
    {
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public GetJwtRefreshTokenQueryHandler(ITokenService tokenService, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<BaseResult<GetJwtRefreshTokenQueryResult>> Handle(GetJwtRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var tokenLifeTime = TimeSpan.FromMinutes(int.Parse(_configuration.GetSection(Constants.TOKEN_LIFE_TIME).Value!));
            var refreshTokenLifeTime = TimeSpan.FromMinutes(int.Parse(_configuration.GetSection(Constants.REFRESH_TOKEN_LIFE_TIME).Value!));

            var result = _tokenService.GenerateJwtRefreshToken(new RefreshTokenRequest
            {
                JwtRefreshToken = request.JwtRefreshToken,
                RefreshTokenLifeTime = refreshTokenLifeTime,
                TokenLifeTime = tokenLifeTime
            });

            return new BaseResult<GetJwtRefreshTokenQueryResult>(){ Data = new GetJwtRefreshTokenQueryResult() { JwtToken = result.JwtToken } };
        }
    }
}
