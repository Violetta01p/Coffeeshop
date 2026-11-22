using CoffeeShop.Application.Orders.Dtos;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Інтерфейс виключно для команд створення та зміни стану замовлень (ISP / SRP).
/// Приймає вхідні DTO (CreateOrderRequestDto, UpdateOrderStatusRequestDto)
/// та повертає деталізований DTO замовлення (OrderDetailsDto).
/// </summary>
public interface IOrderCommandService
{
    Task<OrderDetailsDto> CreateAsync(CreateOrderRequestDto request, CancellationToken ct = default);
    Task<OrderDetailsDto> ChangeStatusAsync(Guid id, UpdateOrderStatusRequestDto request, CancellationToken ct = default);
}
