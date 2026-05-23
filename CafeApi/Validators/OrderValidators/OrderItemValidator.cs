using CafeApi.DTOs;
using FluentValidation;

namespace CafeApi.Validators.OrderValidators;

public class OrderItemValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.MenuItemId)
            .NotEmpty().WithMessage("MenuItemId is required.")
            .GreaterThan(0).WithMessage("MenuItemId must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}