
using Models.DTOs;
using FluentValidation;
using Shared;

namespace Models.Validators
{
    public class PlayerRegisterDtoValidator : AbstractValidator<PlayerRegisterDto>
    {
        public PlayerRegisterDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Alias).NotEmpty().WithMessage("Alias is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                                 .EmailAddress().WithMessage(ErrorMessages.InvalidEmailFormat);
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                                    .MinimumLength(8).WithMessage(ErrorMessages.PasswordTooShort);
            RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required.");
        }
    }
}
