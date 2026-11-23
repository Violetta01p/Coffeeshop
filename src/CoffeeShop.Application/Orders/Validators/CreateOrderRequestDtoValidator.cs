using CoffeeShop.Application.Orders.Dtos;
using FluentValidation;

namespace CoffeeShop.Application.Orders.Validators;

/// <summary>
/// Синтаксична та семантична (бізнес-правила) валідація запиту на створення замовлення.
/// </summary>
public class CreateOrderRequestDtoValidator : AbstractValidator<CreateOrderRequestDto>
{
    public CreateOrderRequestDtoValidator()
    {
        // ── Синтаксична валідація ──
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Ім'я клієнта є обов'язковим і не може бути порожнім.")
            .Length(2, 64)
            .WithMessage("Ім'я клієнта повинно містити від 2 до 64 символів.");

        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("Список позицій замовлення не може бути null.")
            .NotEmpty()
            .WithMessage("Замовлення повинно містити щонайменше одну позицію.");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemRequestDtoValidator());

        // ── Семантична валідація (бізнес-обмеження) ──
        RuleFor(x => x.Items)
            .Must(HaveUniqueMenuItems)
            .WithMessage("Позиції в замовленні не повинні дублюватися; збільште кількість замість додавання дубліката.")
            .When(x => x.Items != null && x.Items.Count > 0);

        RuleFor(x => x.Items)
            .Must(items => items.Sum(i => i.Quantity) <= 50)
            .WithMessage("Загальна кількість позицій в одному замовленні не може перевищувати 50.")
            .When(x => x.Items != null && x.Items.Count > 0);
    }

    private static bool HaveUniqueMenuItems(List<OrderItemRequestDto> items)
    {
        if (items == null || items.Count <= 1)
            return true;

        var seen = new HashSet<Guid>();
        foreach (var item in items)
        {
            if (!seen.Add(item.MenuItemId))
                return false;
        }

        return true;
    }
}
