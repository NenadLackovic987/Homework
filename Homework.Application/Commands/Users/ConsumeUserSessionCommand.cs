using Homework.Application.Services;
using Homework.Common.Dto;

namespace Homework.Application.Commands.Users
{
    public class ConsumeUserSessionCommand : IRequest<BaseResult<ConsumeUserSessionCommandResult>>
    {
        public Guid SessionId { get; set; }
    }

    public class ConsumeUserSessionCommandHandler : IRequestHandler<ConsumeUserSessionCommand, BaseResult<ConsumeUserSessionCommandResult>>
    {
        private readonly IAdministrationService _administrationService;

        public ConsumeUserSessionCommandHandler(IAdministrationService administrationService)
        {
            _administrationService = administrationService;
        }

        public async Task<BaseResult<ConsumeUserSessionCommandResult>> Handle(ConsumeUserSessionCommand request, CancellationToken cancellationToken)
        {
            var email = await _administrationService.ConsumeUserSession(request.SessionId);

            return new BaseResult<ConsumeUserSessionCommandResult> { Data = new ConsumeUserSessionCommandResult() { Email = email } };
        }
    }
}
