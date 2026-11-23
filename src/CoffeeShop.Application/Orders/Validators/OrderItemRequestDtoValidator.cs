using CoffeeShop.Application.Orders.Dtos;
using FluentValidation;

namespace CoffeeShop.Application.Orders.Validators;

/// <summary>
/// Синтаксична валідація окремої позиції замовлення (OrderItemRequestDto).
/// </summary>
public class OrderItemRequestDtoValidator : AbstractValidator<OrderItemRequestDto>
{
    public OrderItemRequestDtoValidator()
    {
        RuleFor(x => x.MenuItemId)
            .NotEmpty()
            .WithMessage("Ідентифікатор позиції меню (MenuItemId) не може бути порожнім.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Кількість товару повинна бути більшою за 0.")
            .LessThanOrEqualTo(20)
            .WithMessage("Кількість однієї позиції не може перевищувати 20 одиниць.");
    }
}
