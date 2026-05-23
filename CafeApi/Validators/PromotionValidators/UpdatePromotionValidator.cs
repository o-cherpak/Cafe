using CafeApi.DTOs;
using FluentValidation;

namespace CafeApi.Validators.PromotionValidators;

public class UpdatePromotionValidator : AbstractValidator<UpdatePromotionDto>
{
    public UpdatePromotionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Promotion name cannot be empty.")
            .MaximumLength(200).WithMessage("Name max 200 characters.")
            .When(x => x.Name != null);

        RuleFor(x => x.Description)
            .MaximumLength(400).WithMessage("Description max 400 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("Discount must be greater than zero.")
            .When(x => x.DiscountValue != null);

        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("IsActive must be a boolean.")
            .When(x => x.IsActive != null);
    }
}