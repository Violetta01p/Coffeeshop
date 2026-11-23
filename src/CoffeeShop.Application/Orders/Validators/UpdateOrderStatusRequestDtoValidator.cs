using CoffeeShop.Application.Orders.Dtos;
using FluentValidation;

namespace CoffeeShop.Application.Orders.Validators;

/// <summary>
/// Валідація запиту на зміну статусу замовлення (UpdateOrderStatusRequestDto).
/// </summary>
public class UpdateOrderStatusRequestDtoValidator : AbstractValidator<UpdateOrderStatusRequestDto>
{
    private static readonly HashSet<string> AllowedActions = new(StringComparer.OrdinalIgnoreCase)
    {
        "prepare",
        "ready",
        "complete",
        "cancel",
        "preparing",
        "completed",
        "cancelled"
    };

    public UpdateOrderStatusRequestDtoValidator()
    {
        RuleFor(x => x.TargetAction)
            .NotEmpty()
            .WithMessage("Дія або статус для оновлення замовлення є обов'язковими.")
            .Must(action => AllowedActions.Contains(action.Trim()))
            .WithMessage($"Невідома дія або статус. Допустимі значення: {string.Join(", ", AllowedActions)}.");
    }
}
