using CafeApi.DTOs;
using FluentValidation;

namespace CafeApi.Validators;

public class UpdateMenuItemValidator : AbstractValidator<UpdateMenuItemDto>
{
    public UpdateMenuItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .MaximumLength(140).WithMessage("Name max 140 characters")
            .When(x => x.Name is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .When(x => x.Price is not null);
    }
}