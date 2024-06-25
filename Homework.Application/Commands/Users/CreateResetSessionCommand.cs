using Homework.Application.Services;
using Homework.Common.Dto;

namespace Homework.Application.Commands.Users
{
    public class CreateResetSessionCommand : IRequest<BaseResult<CreateResetSessionCommandResult>>
    {
        public string Email { get; set; }
    }

    public class CreateResetSessionCommandHandler : IRequestHandler<CreateResetSessionCommand, BaseResult<CreateResetSessionCommandResult>>
    {
        private readonly IAdministrationService _administrationService;
        private readonly IEmailService _emailService;

        public CreateResetSessionCommandHandler(IAdministrationService administrationService, IEmailService emailService)
        {
            _administrationService = administrationService;
            _emailService = emailService;
        }

        public async Task<BaseResult<CreateResetSessionCommandResult>> Handle(CreateResetSessionCommand request, CancellationToken cancellationToken)
        {
            var sessionId = Guid.NewGuid();
            var pass = await _administrationService.CreateResetSession(sessionId, request.Email);

            _emailService.SendResetPasswordEmail(request.Email, sessionId, pass);

            return new BaseResult<CreateResetSessionCommandResult> { Data = new CreateResetSessionCommandResult() { } };
        }
    }
}