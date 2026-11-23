using CoffeeShop.Application.Menu.Dtos;
using CoffeeShop.Domain.Menu.Enums;
using FluentValidation;

namespace CoffeeShop.Application.Menu.Validators;

/// <summary>
/// Синтаксична та семантична валідація створення нової позиції меню (CreateMenuItemRequestDto).
/// </summary>
public class CreateMenuItemRequestDtoValidator : AbstractValidator<CreateMenuItemRequestDto>
{
    public CreateMenuItemRequestDtoValidator()
    {
        // ── Синтаксична валідація ──
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Назва позиції меню є обов'язковою.")
            .Length(2, 100)
            .WithMessage("Назва позиції меню повинна містити від 2 до 100 символів.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Ціна позиції меню повинна бути більшою за 0.")
            .LessThanOrEqualTo(10_000)
            .WithMessage("Ціна позиції меню не може перевищувати 10 000 грн.");

        // ── Семантична валідація ──
        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Категорія позиції меню є обов'язковою.")
            .Must(BeValidCategory)
            .WithMessage("Невідома категорія меню. Допустимі значення: HotDrink, ColdDrink, Dessert, Snack.");
    }

    private static bool BeValidCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return false;

        return Enum.TryParse<MenuCategory>(category.Trim(), ignoreCase: true, out var result) &&
               Enum.IsDefined(typeof(MenuCategory), result);
    }
}
