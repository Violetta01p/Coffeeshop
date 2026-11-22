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
│       ├── Events/
│       │   └── OrderEvents.cs      ⚡ Доменні події (OrderCreated, OrderStatusChanged)
│       └── Repositories/IOrderRepository.cs
│
├── CoffeeShop.Application/         ➡️ залежить від Domain
│   ├── Events/
│   │   ├── IEventBus.cs            ⚡ Контракт шини подій
│   │   └── IEventHandler.cs        ⚡ Контракт підписника/обробника
│   ├── Menu/
│   │   ├── Dtos/MenuDtos.cs
│   │   └── Services/MenuService.cs
│   └── Orders/
│       ├── Dtos/OrderDtos.cs
│       ├── Dtos/OrderAnalyticsDto.cs
│       └── Services/
│           ├── OrderService.cs     ⚡ Публікує події через IEventBus
│           └── IOrderAnalyticsService.cs
│
├── CoffeeShop.Infrastructure/      ➡️ залежить від Application
│   ├── Events/
│   │   └── InProcessEventBus.cs    ⚡ In-Process шина на базі IServiceProvider
│   ├── Menu/InMemoryMenuItemRepository.cs
│   ├── Orders/
│   │   ├── InMemoryOrderRepository.cs
│   │   ├── Analytics/InMemoryOrderAnalyticsStore.cs
│   │   └── Handlers/
│   │       ├── OrderCreatedNotificationHandler.cs
│   │       ├── OrderCreatedAnalyticsHandler.cs
│   │       └── OrderStatusChangedNotificationHandler.cs
│   ├── Gateways/MenuCatalogGateway.cs
│   └── DependencyInjection.cs
│
└── CoffeeShop.WebApi/              ➡️ залежить від Application + Infrastructure
    ├── Controllers/
    │   ├── MenuController.cs
    │   ├── OrdersController.cs
    │   └── AnalyticsController.cs  ⚡ GET /api/analytics
    ├── Middleware/ExceptionHandlingMiddleware.cs
    └── Program.cs
```

---

## ⚡ Практична робота №2: Подієво-орієнтований підхід (Event-Driven Design)

У межах ПЗ №2 реалізовано внутрішньопроцесну подієво-орієнтовану модель (In-Process Event-Driven Architecture), яка забезпечує **слабку зв'язаність (Loose Coupling)** та дотримання принципів Clean Architecture.

### 1. Архітектурні компоненти
- **Контракти подій (Domain):**
  - `OrderCreatedEvent` — публікується при створенні нового замовлення (містить `OrderId`, `CustomerName`, `TotalAmount`, `ItemsCount`, `CreatedAtUtc`).
  - `OrderStatusChangedEvent` — публікується при зміні статусу замовлення (`OrderId`, `CustomerName`, `PreviousStatus`, `NewStatus`, `ChangedAtUtc`).
- **Шина подій (Application / Infrastructure):**
  - `IEventBus` (Application) — інтерфейс шини для відправки подій `PublishAsync<TEvent>(evt, ct)`.
  - `IEventHandler<in TEvent>` (Application) — контракт обробників.
  - `InProcessEventBus` (Infrastructure) — резолвить усіх зареєстрованих підписників через `IServiceProvider.GetServices<IEventHandler<TEvent>>()` та асинхронно викликає їх.
- **Продюсер:**
  - `OrderService` знає **лише** про `IEventBus`. Жодної прямої залежності від сервісів сповіщень чи аналітики.
- **Споживачі (Consumers / Handlers):**
  1. `OrderCreatedNotificationHandler` — імітує сповіщення баристи / кухні про надходження замовлення.
  2. `OrderCreatedAnalyticsHandler` — автоматично агрегує виручку та кількість замовлень у `InMemoryOrderAnalyticsStore`.
  3. `OrderStatusChangedNotificationHandler` — надсилає гостю контекстне сповіщення при переходах статусів (`Preparing`, `Ready`, `Completed`, `Cancelled`).

### 2. Ендпоінт демонстрації роботи обробників
- `GET /api/analytics` — повертає зведену статистику, накопичену аналітичним обробником подій у реальному часі.

---

## 📝 Технології

- .NET 8 / ASP.NET Core Web API
- Swagger / OpenAPI (Swashbuckle)
- InMemory-сховище (ConcurrentDictionary) — проєкт запускається без налаштування БД
- In-Process Event Bus (Event-Driven Architecture)
