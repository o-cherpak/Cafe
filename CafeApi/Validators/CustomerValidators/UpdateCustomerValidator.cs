using CafeApi.DTOs;
using FluentValidation;

namespace CafeApi.Validators.CustomerValidators;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .MaximumLength(140).WithMessage("Name max 140 characters")
            .When(x => x.Name is not null);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Invalid email")
            .MaximumLength(200).WithMessage("Email max 200 characters")
            .When(x => x.Email is not null);
    }
}