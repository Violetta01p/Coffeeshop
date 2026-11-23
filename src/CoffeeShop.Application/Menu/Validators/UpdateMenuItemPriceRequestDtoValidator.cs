using CoffeeShop.Application.Menu.Dtos;
using FluentValidation;

namespace CoffeeShop.Application.Menu.Validators;

/// <summary>
/// Синтаксична валідація зміни ціни позиції меню (UpdateMenuItemPriceRequestDto).
/// </summary>
public class UpdateMenuItemPriceRequestDtoValidator : AbstractValidator<UpdateMenuItemPriceRequestDto>
{
    public UpdateMenuItemPriceRequestDtoValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Ціна позиції меню повинна бути більшою за 0.")
            .LessThanOrEqualTo(10_000)
            .WithMessage("Ціна позиції меню не може перевищувати 10 000 грн.");
    }
}
