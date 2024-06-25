using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Homework.Application.Identity;
using Homework.Application.Identity.DataTransferObjects;
using Homework.Application.Services;
using Homework.Common;
using Homework.Common.Dto;
using Homework.Common.Enums;
using Homework.Domain.Models.Identity;

namespace Homework.Application.Queries.Users
{
    public class LoginQuery : IRequest<BaseResult<LoginQueryResult>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginQueryHandler : IRequestHandler<LoginQuery, BaseResult<LoginQueryResult>>
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IValidator<LoginQuery> _loginQueryValidator;
        private readonly ILogger<LoginQueryHandler> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAdministrationService _userRolesService;

        public LoginQueryHandler(ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IValidator<LoginQuery> loginQueryValidator,
            ILogger<LoginQueryHandler> logger,
            SignInManager<ApplicationUser> signInManager, 
            IAdministrationService userRolesService
            )
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _configuration = configuration;
            _loginQueryValidator = loginQueryValidator;
            _logger = logger;
            _signInManager = signInManager;
            _userRolesService = userRolesService;
        }

        public async Task<BaseResult<LoginQueryResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var loginResult = new LoginQueryResult();

            ValidationResult validationResult = await _loginQueryValidator.ValidateAsync(request);


            if (!validationResult.IsValid)
                return new BaseResult<LoginQueryResult>(new ErrorCode[] { ErrorCode.LoginValidationError });

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogError(Constants.USER_NOT_FOUND);

                return new BaseResult<LoginQueryResult>(new ErrorCode[] { ErrorCode.UserNotFound });
            }

            if (user.Status == false)
            {
                _logger.LogError(Constants.USER_LOGIN_ERROR);

                return new BaseResult<LoginQueryResult>(new ErrorCode[] { ErrorCode.UserIsNotActive });
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user.Email!, request.Password, false, true);

            if (signInResult.IsLockedOut)
            {
                _logger.LogError(Constants.USER_LOGIN_ERROR);

                return new BaseResult<LoginQueryResult>(new ErrorCode[] { ErrorCode.UserIsLocked });
            }
            else if (!signInResult.Succeeded)
            {
                _logger.LogError(Constants.USER_LOGIN_ERROR);

                return new BaseResult<LoginQueryResult>(new ErrorCode[] { ErrorCode.LoginValidationError });
            }
            
            user.AccessFailedCount = 0;
            await _userManager.UpdateAsync(user);

            var tokenLifeTime = TimeSpan.FromMinutes(int.Parse(_configuration.GetSection(Constants.TOKEN_LIFE_TIME).Value!));
            var refreshTokenLifeTime = TimeSpan.FromMinutes(int.Parse(_configuration.GetSection(Constants.REFRESH_TOKEN_LIFE_TIME).Value!));
            
            var tokens = _tokenService.GenerateJwtToken(new TokenGenerationRequest()
            {
                Id = user.Id,
                Email = request.Email,
                Password = request.Password,
                TokenLifeTime = tokenLifeTime,
                RefreshTokenLifeTime = refreshTokenLifeTime,
                FirstName = user.FirstName, 
                LastName = user.LastName, 
                Position = user.Position!, 
                NickName = user.NickName
            });

            loginResult.JwtRefreshToken = tokens.JwtRefreshToken;
            loginResult.JwtToken = tokens.JwtToken;
            loginResult.NeedPasswordReset = user.NeedPasswordReset;

            return new BaseResult<LoginQueryResult>()
            {
                Data = loginResult
            };
        }
    }
}
