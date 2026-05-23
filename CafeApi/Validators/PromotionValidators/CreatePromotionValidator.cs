using CafeApi.DTOs;
using FluentValidation;

namespace CafeApi.Validators.PromotionValidators;

public class CreatePromotionValidator : AbstractValidator<CreatePromotionDto>
{
    public CreatePromotionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Promotion name is required.")
            .MaximumLength(200).WithMessage("Name max 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(400).WithMessage("Description max 400 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.BonusCost)
            .GreaterThanOrEqualTo(0).WithMessage("Bonus must be greater then 0.");

        RuleFor(x => x.DiscountType)
            .IsInEnum().WithMessage("Invalid type.");

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than zero.");
    }
}