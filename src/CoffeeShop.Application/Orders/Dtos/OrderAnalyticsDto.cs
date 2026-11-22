namespace CoffeeShop.Application.Orders.Dtos;

/// <summary>
/// DTO зі зведеною статистикою, накопиченою Event-обробниками.
/// </summary>
public sealed record OrderAnalyticsDto(
    int TotalOrdersCount,
    decimal TotalRevenue,
    DateTime? LastOrderCreatedAtUtc,
    int StatusChangesCount);
