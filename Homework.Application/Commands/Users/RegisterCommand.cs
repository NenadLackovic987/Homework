using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Homework.Application.Identity;
using Homework.Application.Identity.DataTransferObjects;
using Homework.Common;
using Homework.Common.Dto;
using Homework.Common.Enums;
using Homework.Domain.Models.Identity;

namespace Homework.Application.Commands.Users
{
    public class RegisterCommand : IRequest<BaseResult<RegisterCommandResult>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? NickName { get; set; }
        public string Position { get; set; } = null!;
        public bool NeedPasswordReset { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, BaseResult<RegisterCommandResult>>
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private IValidator<RegisterCommand> _registerCommandValidator;

        public RegisterCommandHandler(ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IValidator<RegisterCommand> registerCommandValidator)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _configuration = configuration;
            _registerCommandValidator = registerCommandValidator;
        }

        public async Task<BaseResult<RegisterCommandResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var registerResult = new RegisterCommandResult();

            var validationResult = await _registerCommandValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
                return new BaseResult<RegisterCommandResult>(new ErrorCode[] { ErrorCode.UserIsNotActive });

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user != null)
            {
                return new BaseResult<RegisterCommandResult>(new ErrorCode[] { ErrorCode.UserIsNotActive });
            }

            var result = await _userManager.CreateAsync(new ApplicationUser()
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = new Guid().ToString(),
                NormalizedEmail = request.Email,
                NormalizedUserName = request.Email,
                TwoFactorEnabled = false,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Position = request.Position,
                NickName = request.NickName,
                NeedPasswordReset = request.NeedPasswordReset,
                Status = true,
                CreatedBy = request.Email,
                CreatedDateTime = DateTime.UtcNow,
            }, request.Password);

            if (result == null)
            {
                return new BaseResult<RegisterCommandResult>(new ErrorCode[] { ErrorCode.RegistrationError });
            }
            else if (!result.Succeeded)
            {
                return new BaseResult<RegisterCommandResult>(new ErrorCode[] { ErrorCode.RegistrationError });
            }
            else
            {
                var tokenLifeTime = TimeSpan.FromMinutes(int.Parse(_configuration.GetSection(Constants.TOKEN_LIFE_TIME).Value!));
                var refreshTokenLifeTime = TimeSpan.FromMinutes(int.Parse(_configuration.GetSection(Constants.REFRESH_TOKEN_LIFE_TIME).Value!));
                user = await _userManager.FindByEmailAsync(request.Email);
                var tokens = _tokenService.GenerateJwtToken(new TokenGenerationRequest()
                {
                    Id = user.Id,
                    Email = request.Email,
                    Password = request.Password,
                    TokenLifeTime = tokenLifeTime,
                    RefreshTokenLifeTime = refreshTokenLifeTime,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Position = request.Position,
                    NickName = request.NickName
                });

                registerResult.JwtRefreshToken = tokens.JwtRefreshToken;
                registerResult.JwtToken = tokens.JwtToken;

                return new BaseResult<RegisterCommandResult>()
                {
                    Data = registerResult
                };
            }
        }
    }
}

