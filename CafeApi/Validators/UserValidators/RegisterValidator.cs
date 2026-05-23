using CafeApi.DTOs;
using FluentValidation;

namespace CafeApi.Validators.UserValidators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(140).WithMessage("Name max 140 characters.")
            .When(x => x.Name != null);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email")
            .MaximumLength(200).WithMessage("Email max 200 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid user role.");
    }
}