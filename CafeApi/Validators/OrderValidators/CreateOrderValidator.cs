using CafeApi.DTOs;
using FluentValidation;

namespace CafeApi.Validators.OrderValidators;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.")
            .GreaterThan(0).WithMessage("CustomerId must be greater than zero.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.")
            .NotNull().WithMessage("Items list cannot be null.");
        
        RuleForEach(x => x.Items).SetValidator(new OrderItemValidator());
    }
}