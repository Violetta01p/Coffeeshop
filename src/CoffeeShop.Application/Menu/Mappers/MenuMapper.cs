using CoffeeShop.Application.Menu.Dtos;
using CoffeeShop.Domain.Common.Exceptions;
using CoffeeShop.Domain.Menu.Entities;
using CoffeeShop.Domain.Menu.Enums;

namespace CoffeeShop.Application.Menu.Mappers;

/// <summary>
/// Stateless-мапер сутностей меню (Transient).
/// Реалізує безпечне перетворення доменних сутностей у вихідні DTO
/// та валідований мапінг рядкових категорій у доменні enums.
/// </summary>
public class MenuMapper : IMenuMapper
{
    public MenuItemSummaryDto MapToSummary(MenuItem item) =>
        new(item.Id,
            item.Name,
            item.Category.ToString(),
            item.Price.Amount,
            item.IsAvailable);

    public IReadOnlyList<MenuItemSummaryDto> MapToSummaryList(IEnumerable<MenuItem> items) =>
        items.Select(MapToSummary).ToList();

    public MenuCategory MapToCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new DomainException("Категорія позиції меню не може бути порожньою.");
        }

        if (Enum.TryParse<MenuCategory>(category.Trim(), ignoreCase: true, out var result) &&
            Enum.IsDefined(typeof(MenuCategory), result))
        {
            return result;
        }

        var validCategories = string.Join(", ", Enum.GetNames<MenuCategory>());
        throw new DomainException(
            $"Некоректна категорія меню «{category}». Допустимі значення: {validCategories}.");
    }
}
