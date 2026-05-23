using CafeApi.DTOs;
using FluentValidation;

namespace CafeApi.Validators.CustomerValidators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(140).WithMessage("Name max 140 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email")
            .MaximumLength(200).WithMessage("Email max 200 characters");
    }
}