using CoffeeShop.Application.Menu.Dtos;
using CoffeeShop.Domain.Menu.Entities;

namespace CoffeeShop.Application.Menu.Mappers;

/// <summary>
/// Інтерфейс мапера сутностей Menu у DTO (Transient).
/// </summary>
public interface IMenuMapper
{
    MenuItemResponse ToResponse(MenuItem item);
    IReadOnlyList<MenuItemResponse> ToResponseList(IEnumerable<MenuItem> items);
}
