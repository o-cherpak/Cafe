using CafeApi.DTOs;
using FluentValidation;

namespace CafeApi.Validators;

public class CreateMenuItemValidator : AbstractValidator<CreateMenuItemDto>
{
    public CreateMenuItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(140).WithMessage("Max 140 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Must be greater then 0");

        RuleFor(x => x.Category).IsInEnum();

        RuleFor(x => x.Description)
            .MaximumLength(400).WithMessage("Description max length is 400 chars");
    }
}
