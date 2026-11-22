# ☕ CoffeeShop — Чиста архітектура та Domain-Driven Design (DDD)

Практичне рішення на **ASP.NET Core Web API (.NET 8)**, яке демонструє застосування принципів **Clean Architecture** та **Domain-Driven Design** на прикладі предметної області «Кав'ярня».

---

## 📋 Предметна область

Система моделює роботу кав'ярні та охоплює два бізнес-процеси:
- **управління меню** — створення позицій (напої, десерти), встановлення цін та контроль доступності;
- **оформлення замовлень** — створення замовлення гостем, підрахунок суми та керування статусами приготування.

---

## 🏗️ Архітектура та напрямок залежностей

Рішення побудоване за принципом **Clean Architecture (Onion Architecture)** — зовнішні шари залежать від внутрішніх, але **ніколи навпаки**.

```
┌─────────────────────────────────────────────────────────────┐
│                     WebApi (ASP.NET Core)                    │
│  Контролери · Middleware · Swagger · DI-конфігурація         │
│         залежить від: Application, Infrastructure           │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│                      Infrastructure                         │
│  InMemory-репозиторії · MenuCatalogGateway · DI Extension   │
│              залежить від: Application                      │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│                       Application                           │
│  DTOs · MenuService · OrderService (Use Cases)              │
│               залежить від: Domain                          │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│                         Domain                              │
│  Entities · Value Objects · Enums · Domain Exceptions       │
│  Інтерфейси репозиторіїв · IMenuCatalogGateway              │
│              🔒 нуль зовнішніх залежностей                  │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔀 Bounded Contexts

### 1. Menu (Меню)
- **Aggregate Root:** `MenuItem` — назва, категорія (`HotDrink`, `ColdDrink`, `Dessert`, `Snack`), ціна, доступність.
- **Репозиторій:** `IMenuItemRepository`.
- **Use Cases (MenuService):** отримання всіх позицій, додавання нової, зміна ціни, зміна доступності.

### 2. Orders (Замовлення)
- **Aggregate Root:** `Order` — ім'я клієнта, статус (`Pending` → `Preparing` → `Ready` → `Completed` / `Cancelled`), список позицій.
- **Entity:** `OrderItem` — посилання на `MenuItemId`, зафіксована назва та ціна на момент замовлення, кількість.
- **Репозиторій:** `IOrderRepository`.
- **Use Cases (OrderService):** створення замовлення, перегляд, зміна статусу, скасування.

### Взаємодія між контекстами

Контекст **Orders** не має прямої залежності від внутрішньої реалізації **Menu**. Взаємодія відбувається через **контракт-інтерфейс** `IMenuCatalogGateway`, визначений у шарі Domain (контекст Orders):

```csharp
public interface IMenuCatalogGateway
{
    Task<CatalogItemInfo?> GetItemInfoAsync(Guid menuItemId, CancellationToken ct = default);
}
```

Реалізація `MenuCatalogGateway` знаходиться в шарі Infrastructure та делегує виклик до `IMenuItemRepository`.

---

## 🧱 DDD-елементи

| Елемент | Приклади |
|---|---|
| **Entity / Aggregate Root** | `MenuItem`, `Order`, `OrderItem` |
| **Value Object** | `Money` (грошова сума), `CustomerName` (ім'я гостя) |
| **Domain Exception** | `DomainException` — порушення бізнес-інваріантів |
| **Repository Interface** | `IMenuItemRepository`, `IOrderRepository` |
| **Gateway Contract** | `IMenuCatalogGateway` |

### Бізнес-інваріанти
- Ціна позиції меню повинна бути > 0.
- Замовлення не може бути порожнім.
- Кількість позиції в замовленні повинна бути > 0.
- Зміна статусу замовлення дозволена лише за визначеними переходами.
- Скасувати вже видане (Completed) замовлення неможливо.

---

## 🚀 Запуск

```bash
cd src/CoffeeShop.WebApi
dotnet run
```

Swagger UI буде доступний за адресою: **http://localhost:5000/swagger**

---

## 🧪 Приклади запитів для тестування API

### Menu

**Отримати все меню:**
```http
GET /api/menu
```

**Отримати позицію за Id:**
```http
GET /api/menu/a1111111-1111-1111-1111-111111111111
```

**Додати нову позицію:**
```http
POST /api/menu
Content-Type: application/json

{
  "name": "Американо",
  "category": 0,
  "price": 60.00
}
```

**Змінити доступність:**
```http
PATCH /api/menu/a1111111-1111-1111-1111-111111111111/availability
Content-Type: application/json

{
  "isAvailable": false
}
```

**Змінити ціну:**
```http
PATCH /api/menu/a1111111-1111-1111-1111-111111111111/price
Content-Type: application/json

{
  "price": 65.00
}
```

### Orders

**Створити замовлення:**
```http
POST /api/orders
Content-Type: application/json

{
  "customerName": "Олена",
  "items": [
    { "menuItemId": "a2222222-2222-2222-2222-222222222222", "quantity": 2 },
    { "menuItemId": "a5555555-5555-5555-5555-555555555555", "quantity": 1 }
  ]
}
```

**Отримати всі замовлення:**
```http
GET /api/orders
```

**Змінити статус (prepare → ready → complete):**
```http
PATCH /api/orders/{orderId}/status
Content-Type: application/json

{ "action": "prepare" }
```

**Скасувати замовлення:**
```http
PATCH /api/orders/{orderId}/status
Content-Type: application/json

{ "action": "cancel" }
```

---

## 📁 Структура проєкту

```
CoffeeShop.sln
src/
├── CoffeeShop.Domain/              🔒 Нуль зовнішніх залежностей
│   ├── Common/
│   │   ├── Exceptions/DomainException.cs
│   │   └── ValueObjects/
│   │       ├── Money.cs
│   │       └── CustomerName.cs
│   ├── Menu/
│   │   ├── Entities/MenuItem.cs
│   │   ├── Enums/MenuCategory.cs
│   │   └── Repositories/IMenuItemRepository.cs
│   └── Orders/
│       ├── Contracts/IMenuCatalogGateway.cs
│       ├── Entities/
│       │   ├── Order.cs
│       │   └── OrderItem.cs
│       ├── Enums/OrderStatus.cs
│       └── Repositories/IOrderRepository.cs
│
├── CoffeeShop.Application/         ➡️ залежить від Domain
│   ├── Menu/
│   │   ├── Dtos/MenuDtos.cs
│   │   └── Services/MenuService.cs
│   └── Orders/
│       ├── Dtos/OrderDtos.cs
│       └── Services/OrderService.cs
│
├── CoffeeShop.Infrastructure/      ➡️ залежить від Application
│   ├── Menu/InMemoryMenuItemRepository.cs
│   ├── Orders/InMemoryOrderRepository.cs
│   ├── Gateways/MenuCatalogGateway.cs
│   └── DependencyInjection.cs
│
└── CoffeeShop.WebApi/              ➡️ залежить від Application + Infrastructure
    ├── Controllers/
    │   ├── MenuController.cs
    │   └── OrdersController.cs
    ├── Middleware/ExceptionHandlingMiddleware.cs
    └── Program.cs
```

---

## 📝 Технології

- .NET 8 / ASP.NET Core Web API
- Swagger / OpenAPI (Swashbuckle)
- InMemory-сховище (ConcurrentDictionary) — проєкт запускається без налаштування БД
