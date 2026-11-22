using CoffeeShop.Application.Menu.Dtos;
using CoffeeShop.Domain.Menu.Entities;

namespace CoffeeShop.Application.Menu.Mappers;

/// <summary>
/// Stateless-мапер сутностей меню (Transient).
/// </summary>
public class MenuMapper : IMenuMapper
{
    public MenuItemResponse ToResponse(MenuItem item) =>
        new(item.Id,
            item.Name,
            item.Category.ToString(),
            item.Price.Amount,
            item.IsAvailable);

    public IReadOnlyList<MenuItemResponse> ToResponseList(IEnumerable<MenuItem> items) =>
        items.Select(ToResponse).ToList();
}
