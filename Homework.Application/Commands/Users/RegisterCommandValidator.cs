using FluentValidation;

namespace Homework.Application.Commands.Users
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(40).WithMessage("Your password length must not exceed 40.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\-\!\?\*\.]+").WithMessage("Your password must contain at least one (-!? *.).");
        }
    }
}

