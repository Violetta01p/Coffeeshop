using CoffeeShop.Application.Menu.Dtos;
using CoffeeShop.Domain.Menu.Entities;
using CoffeeShop.Domain.Menu.Enums;

namespace CoffeeShop.Application.Menu.Mappers;

/// <summary>
/// Інтерфейс мапера між сутностями контексту Menu та DTO (Transient).
/// Забезпечує сувору ізоляцію між Domain та зовнішніми шарами.
/// </summary>
public interface IMenuMapper
{
    MenuItemSummaryDto MapToSummary(MenuItem item);
    IReadOnlyList<MenuItemSummaryDto> MapToSummaryList(IEnumerable<MenuItem> items);
    MenuCategory MapToCategory(string category);
}
