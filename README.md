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
docker-compose.yml                  🐳 Сервіс Memcached 1.6-alpine для розподіленого кешу
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
│       ├── Pricing/                💎 OCP: Політики знижок та ціноутворення
│       │   ├── IDiscountPolicy.cs
│       │   ├── BulkOrderDiscountPolicy.cs
│       │   ├── LargeOrderDiscountPolicy.cs
│       │   ├── IOrderPricingService.cs
│       │   └── OrderPricingService.cs
│       └── Repositories/IOrderRepository.cs
│
├── CoffeeShop.Application/         ➡️ залежить від Domain
│   ├── Common/
│   │   ├── Caching/
│   │   │   └── ICacheService.cs    ⚡ Абстракція розподіленого кешу (Cache-Aside)
│   │   ├── Diagnostics/
│   │   │   └── ILifetimeServices.cs 🔌 Демонстраційні сервіси Transient/Scoped/Singleton
│   │   ├── Time/
│   │   │   └── IDateTimeProvider.cs 🔌 Абстракція часу (DIP/IoC замість DateTime.UtcNow)
│   │   └── IUnitOfWork.cs          💾 Контракт патерну Unit of Work
│   ├── Events/
│   │   ├── IEventBus.cs            ⚡ Контракт шини подій
│   │   └── IEventHandler.cs        ⚡ Контракт підписника/обробника
│   ├── Menu/
│   │   ├── Dtos/MenuDtos.cs
│   │   ├── Mappers/                🔌 Stateless-мапери DTO (Transient)
│   │   │   ├── IMenuMapper.cs
│   │   │   └── MenuMapper.cs
│   │   ├── Services/
│   │   │   ├── IMenuService.cs     💎 DIP: Інтерфейс для MenuController
│   │   │   └── MenuService.cs      💾 Використовує IUnitOfWork для збереження
│   │   └── Validators/             🛡️ FluentValidation: синтаксична та семантична валідація
│   │       ├── CreateMenuItemRequestDtoValidator.cs
│   │       └── UpdateMenuItemPriceRequestDtoValidator.cs
│   └── Orders/
│       ├── Dtos/OrderDtos.cs
│       ├── Dtos/OrderAnalyticsDto.cs
│       ├── Mappers/                🔌 Stateless-мапери DTO (Transient)
│       │   ├── IOrderMapper.cs
│       │   └── OrderMapper.cs
│       ├── Services/
│       │   ├── IOrderQueryService.cs       💎 SRP / ISP: Інтерфейс читання замовлень
│       │   ├── IOrderCommandService.cs     💎 SRP / ISP: Інтерфейс команд замовлень
│       │   ├── OrderQueryService.cs        💎 SRP: Читання замовлень
│       │   ├── OrderCommandService.cs      💾 Атомарне збереження IUnitOfWork перед публікацією подій
│       │   ├── IOrderAnalyticsReader.cs    💎 ISP: Тільки читання аналітики
│       │   ├── IOrderAnalyticsWriter.cs    💎 ISP: Тільки запис аналітики
│       │   └── IOrderAnalyticsService.cs
│       └── Validators/             🛡️ FluentValidation: синтаксична та семантична валідація
│           ├── OrderItemRequestDtoValidator.cs
│           ├── CreateOrderRequestDtoValidator.cs
│           └── UpdateOrderStatusRequestDtoValidator.cs
│   └── DependencyInjection.cs      🔌 Composition Root шару Application
│
├── CoffeeShop.Infrastructure/      ➡️ залежить від Application
│   ├── Caching/
│   │   └── MemcachedCacheService.cs ⚡ Розподілений кеш на базі Memcached (resilience + graceful fallback)
│   ├── Events/
│   │   └── InProcessEventBus.cs    ⚡ In-Process шина на базі IServiceProvider (Scoped)
│   ├── Persistence/                💾 Патерни Repository та Unit of Work
│   │   ├── InMemoryDataStore.cs    💾 Singleton: централізовані колекції меню і замовлень
│   │   ├── InMemoryUnitOfWork.cs   💾 Scoped: черга операцій та атомарний коміт
│   │   ├── InMemoryMenuItemRepository.cs 💾 Scoped: репозиторій позицій меню
│   │   └── InMemoryOrderRepository.cs    💾 Scoped: репозиторій замовлень
│   ├── Orders/
│   │   ├── Analytics/InMemoryOrderAnalyticsStore.cs   💎 Реалізує IOrderAnalyticsReader та Writer
│   │   └── Handlers/
│   │       ├── OrderCreatedNotificationHandler.cs
│   │       ├── OrderCreatedAnalyticsHandler.cs        💎 Залежить суто від IOrderAnalyticsWriter
│   │       └── OrderStatusChangedNotificationHandler.cs 💎 Залежить суто від IOrderAnalyticsWriter
│   ├── Gateways/MenuCatalogGateway.cs
│   ├── Services/
│   │   └── SystemDateTimeProvider.cs 🔌 Singleton-провайдер часу
│   └── DependencyInjection.cs      🔌 Composition Root шару Infrastructure
│
└── CoffeeShop.WebApi/              ➡️ залежить від Application + Infrastructure
    ├── Controllers/
    │   ├── MenuController.cs       💎 Залежить від IMenuService (DIP)
    │   ├── OrdersController.cs     💎 Залежить від IOrderCommandService, IOrderQueryService (DIP / ISP)
    │   ├── AnalyticsController.cs  💎 Залежить від IOrderAnalyticsReader (DIP / ISP)
    │   └── DiagnosticsController.cs 🔌 GET /api/diagnostics/lifetimes
    ├── Middleware/ExceptionHandlingMiddleware.cs
    └── Program.cs                  🔌 Підключення AddApplication() та AddInfrastructure()
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
  - `OrderCommandService` знає **лише** про `IEventBus`. Жодної прямої залежності від сервісів сповіщень чи аналітики.
- **Споживачі (Consumers / Handlers):**
  1. `OrderCreatedNotificationHandler` — імітує сповіщення баристи / кухні про надходження замовлення.
  2. `OrderCreatedAnalyticsHandler` — автоматично агрегує виручку та кількість замовлень у `InMemoryOrderAnalyticsStore`.
  3. `OrderStatusChangedNotificationHandler` — надсилає гостю контекстне сповіщення при переходах статусів (`Preparing`, `Ready`, `Completed`, `Cancelled`).

### 2. Ендпоінт демонстрації роботи обробників
- `GET /api/analytics` — повертає зведену статистику, накопичену аналітичним обробником подій у реальному часі.

---

## 💎 Практична робота №3: Принципи SOLID у проєктуванні компонентів ПЗ

У межах ПЗ №3 проведено глибокий рефакторинг кодової бази із дотриманням 4 ключових принципів SOLID:

### 1. DIP (Dependency Inversion Principle — Принцип інверсії залежностей)
* **До:** Контролери `MenuController` та `OrdersController` напряму залежали від конкретних реалізацій сервісів `MenuService` та `OrderService`. Модулі високого рівня були жорстко прив'язані до конкретних класів.
* **Після:**
  - Створено інтерфейси `IMenuService`, `IOrderCommandService` та `IOrderQueryService` у шарі `Application`.
  - Контролери залежать **виключно від інтерфейсів-абстракцій**.
  - Конкретні реалізації реєструються у DI-контейнері (`AddInfrastructure()`).

### 2. SRP (Single Responsibility Principle — Принцип єдиної відповідальності)
* **До:** Клас `OrderService` поєднував у собі запити на читання (`GetAll`, `GetById`), валідацію і створення замовлень, застосування знижок, зміну статусів та публікацію подій. Він мав забагато причин для змін.
* **Після:**
  - `OrderQueryService` — має одну зону відповідальності: читання та мапінг замовлень у DTO.
  - `OrderCommandService` — відповідає виключно за виконання бізнес-команд зі зміни стану, виклик розрахунку знижок та відправку подій у шину `IEventBus`.

### 3. ISP (Interface Segregation Principle — Принцип розділення інтерфейсу)
* **До:** Сервіс аналітики мав універсальний інтерфейс `IOrderAnalyticsService`, що поєднував методи читання (`GetStatistics`) та запису (`RecordOrder`, `RecordStatusChange`). В результаті клієнти залежали від непотрібних їм методів.
* **Після:**
  - Створено два вузьких інтерфейси:
    - `IOrderAnalyticsReader` — містить лише `GetStatistics()`, впроваджується **тільки** в `AnalyticsController`.
    - `IOrderAnalyticsWriter` — містить методи мутації, впроваджується **тільки** в обробники подій `OrderCreatedAnalyticsHandler` та `OrderStatusChangedNotificationHandler`.
  - Сховище `InMemoryOrderAnalyticsStore` реалізує обидва інтерфейси, але кожен клієнт бачить лише свій контракт.

### 4. OCP (Open/Closed Principle — Принцип відкритості / закритості)
* **До:** Будь-яка нова знижка або акційне правило вимагала б дописування розгалужених ланцюжків `if/else` всередині сутності чи прикладного сервісу.
* **Після:**
  - У шарі `Domain` створено абстракцію політики знижок `IDiscountPolicy` з методом `Money CalculateDiscount(Order order)`.
  - Реалізовано конкретні стратегії:
    - `BulkOrderDiscountPolicy` — 10% знижки при замовленні від 3 товарів;
    - `LargeOrderDiscountPolicy` — 5% знижки на чеки від 300 грн.
  - Доменний сервіс `OrderPricingService : IOrderPricingService` приймає `IEnumerable<IDiscountPolicy>` через DI та обирає найбільш вигідну для клієнта знижку.
  - **Нові знижки додаються створенням нового класу-політики** без редагування існуючих класів. Якщо знижки немає, сума замовлення залишається без змін (зворотна сумісність).

---

## 🔌 Практична робота №4: Dependency Injection та інверсія керування (IoC)

У межах ПЗ №4 реалізовано модульну конфігурацію DI-контейнера через патерн **Composition Root**, усунуто статичні та прямі залежності та впроваджено діагностику життєвих циклів:

### 1. Усунення прямих та статичних залежностей
* **Абстракція часу (`IDateTimeProvider`):** Прямі виклики `DateTime.UtcNow` замінено на ін'єкцію інтерфейсу `IDateTimeProvider` (реалізація `SystemDateTimeProvider` у `Infrastructure`). Це забезпечує 100% передбачуваність під час модульного тестування (можливість mock-увати час).
* **Stateless-мапери DTO (`Transient`):** Логіку створення об'єктів відповідей `OrderResponse` та `MenuItemResponse` винесено з сервісів у легковагові мапери `IMenuMapper` та `IOrderMapper`, які впроваджуються через конструктор.

### 2. Матриця життєвих циклів (Service Lifetimes)

| Сервіс / Контракт | Життєвий цикл | Рівень | Обґрунтування вибору |
| :--- | :--- | :--- | :--- |
| `IMenuMapper` → `MenuMapper` | **Transient** | Application | Легковаговий stateless-компонент без внутрішнього стану. Створюється на вимогу та утилізується одразу після використання. |
| `IOrderMapper` → `OrderMapper` | **Transient** | Application | Stateless-мапер замовлень, не зберігає стан. |
| `ITransientService` → `TransientService` | **Transient** | Application | Демонстраційний сервіс для діагностики життєвих циклів. |
| `IMenuService` → `MenuService` | **Scoped** | Application | Бізнес-сервіс варіантів використання меню. Створюється один на весь HTTP-запит. |
| `IOrderQueryService` → `OrderQueryService` | **Scoped** | Application | Сервіс читання замовлень у межах запиту. |
| `IOrderCommandService` → `OrderCommandService` | **Scoped** | Application | Сервіс команд замовлень, координує скоуплені залежності (EventBus, Gateway). |
| `IOrderPricingService` → `OrderPricingService` | **Scoped** | Application | Доменний сервіс розрахунку цін і знижок у межах поточного запиту. |
| `IMenuCatalogGateway` → `MenuCatalogGateway` | **Scoped** | Infrastructure | Міжконтекстний шлюз для безпечної взаємодії Orders → Menu в межах запиту. |
| `IEventBus` → `InProcessEventBus` | **Scoped** | Infrastructure | **Захист від Captive Dependencies**: зареєстрований як Scoped, щоб безпечно резолвити скоуплені обробники подій у межах запиту без витоків пам'яті. |
| `IEventHandler<T>` (усі обробники) | **Scoped** | Infrastructure | Обробники доменних подій, що працюють у скоупі поточного HTTP-запиту. |
| `IScopedService` → `ScopedService` | **Scoped** | Application | Демонстраційний маркер-сервіс (однаковий у межах одного запиту, новий для наступного). |
| `IDateTimeProvider` → `SystemDateTimeProvider` | **Singleton** | Infrastructure | Системний провайдер часу не містить змінного стану, достатньо одного екземпляра на весь час роботи програми. |
| `IDiscountPolicy` (Bulk, Large) | **Singleton** | Application | Незмінні stateless-стратегії розрахунку знижок. |
| `IMenuItemRepository`, `IOrderRepository` | **Singleton** | Infrastructure | In-Memory сховища на базі `ConcurrentDictionary` повинні зберігати дані меню та замовлень між усіма HTTP-запитами. |
| `InMemoryOrderAnalyticsStore` | **Singleton** | Infrastructure | Спільне потокобезпечне сховище аналітики для `IOrderAnalyticsReader` і `IOrderAnalyticsWriter`. |
| `ISingletonService` → `SingletonService` | **Singleton** | Application | Демонстраційний маркер-сервіс (один і той самий екземпляр між усіма запитами). |

### 3. Захист від Captive Dependencies
* Жоден `Singleton`-сервіс не залежить від `Scoped`-компонентів.
* `InProcessEventBus` зареєстровано як `Scoped`, тому він отримує залежності з поточного `IServiceScope`, не створюючи заблокованих чи витокових ресурсів.

### 4. Діагностичний ендпоінт перевірки життєвих циклів
* `GET /api/diagnostics/lifetimes` — повертає по дві інстанції кожного типу сервісу:
  * `transient`: два різні GUID в одному запиті (`areSameInRequest: false`);
  * `scoped`: однакові GUID в одному запиті (`areSameInRequest: true`), але змінюються при кожному наступному HTTP-запиті;
  * `singleton`: однакові GUID в одному запиті та ідентичні при будь-яких наступних запитах (`areSameInRequest: true`).

---

## 💾 Практична робота №5: Патерни Repository та Unit of Work

У межах ПЗ №5 реалізовано патерни **Repository** та **Unit of Work** для забезпечення атомарності змін у сховищі даних та узгодженості з подійно-орієнтованою архітектурою:

### 1. Роль та призначення `IUnitOfWork`
* **Відокремлення інтерфейсів:** Інтерфейс [`IUnitOfWork`](file:///Users/violetta/Documents/CoffeeShop/src/CoffeeShop.Application/Common/IUnitOfWork.cs) розміщено в шарі `Application` (Clean Architecture), що захищає прикладні сервіси від прив'язки до деталей конкретної БД.
* **Атомарність (All-or-Nothing):** Усі методи запису репозиторіїв (`AddAsync`, `UpdateAsync`, `DeleteAsync`) не мутують колекції негайно, а реєструють відкладені дії у черзі `InMemoryUnitOfWork.RegisterAction(Action)`.
* **Фіксування змін (`SaveChangesAsync`):** Лише при виклику `await _unitOfWork.SaveChangesAsync(ct)` всі накопичені в межах запиту операції атомарно виконуються над централізованим сховищем [`InMemoryDataStore`](file:///Users/violetta/Documents/CoffeeShop/src/CoffeeShop.Infrastructure/Persistence/InMemoryDataStore.cs). Якщо до фіксування трапляється помилка — жодна зміна не потрапляє у сховище.

### 2. Транзакційна узгодженість із шиною подій (Transactional Outbox-like Consistency)
* У [`OrderCommandService`](file:///Users/violetta/Documents/CoffeeShop/src/CoffeeShop.Application/Orders/Services/OrderCommandService.cs) доменні події (`OrderCreatedEvent`, `OrderStatusChangedEvent`) публікуються у шину `IEventBus` **виключно після успішного виконання `_unitOfWork.SaveChangesAsync(ct)`**.
* Це гарантує, що сповіщення на кухню, клієнту та обробники аналітики спрацюють лише тоді, коли замовлення гарантовано збережене у сховищі.

### 3. Архітектура компонентів та життєві цикли
* **`InMemoryDataStore` (Singleton):** Централізоване сховище колекцій `MenuItems` та `Orders`, спільне для всього застосунку.
* **`InMemoryUnitOfWork` (Scoped):** Створюється один на HTTP-запит, накопичує операції та очищає чергу після збереження.
* **`InMemoryMenuItemRepository` / `InMemoryOrderRepository` (Scoped):** Читають дані напряму з `InMemoryDataStore`, а запис делегують поточному `InMemoryUnitOfWork`.

---

## 🔄 Практична робота №6: Об'єкти передавання даних (DTO) та мапінг між шарами

У межах ПЗ №6 реалізовано строгу ізоляцію рівня представлення (`CoffeeShop.WebApi`) від доменного шару (`CoffeeShop.Domain`), запроваджено спеціалізовані DTO під різні бізнес-сценарії та розширено stateless-мапери у шарі `CoffeeShop.Application`.

### 1. Повна ізоляція WebApi від Domain
* У контролерах `CoffeeShop.WebApi` (`MenuController`, `OrdersController`, `AnalyticsController`, `DiagnosticsController`) **відсутній будь-який `using CoffeeShop.Domain...`**.
* Доменні сутності (`MenuItem`, `Order`, `OrderItem`), Value Objects (`Money`, `CustomerName`) та доменні enum (`MenuCategory`, `OrderStatus`) ніколи не виходять за межі шару `Domain` та прикладних сервісів `Application`.
* Контролери приймають і повертають виключно контракти DTO з шару `Application`.

### 2. Таблиця спеціалізованих DTO (Requests / Responses)

| Контекст | DTO | Напрямок | Ендпоінт / Сценарій | Призначення та ключові поля |
| :--- | :--- | :--- | :--- | :--- |
| **Menu** | `MenuItemSummaryDto` | **Вихідний (Response)** | `GET /api/Menu`<br>`GET /api/Menu/{id}` | Відображення позиції меню (`Id`, `Name`, `Category`, `Price`, `IsAvailable`). |
| **Menu** | `CreateMenuItemRequestDto` | **Вхідний (Request)** | `POST /api/Menu` | Створення нової позиції меню. `Category` передається рядком (`string`), що захищає WebApi від доменного enum `MenuCategory`. |
| **Menu** | `UpdateMenuItemPriceRequestDto` | **Вхідний (Request)** | `PATCH /api/Menu/{id}/price` | Оновлення вартості позиції (`Price`). |
| **Menu** | `UpdateMenuItemAvailabilityRequestDto` | **Вхідний (Request)** | `PATCH /api/Menu/{id}/availability` | Оновлення прапорця доступності (`IsAvailable`). |
| **Orders** | `OrderSummaryDto` | **Вихідний (Response)** | `GET /api/Orders` | Компактне відображення списку замовлень (`Id`, `CustomerName`, `Status`, `ItemsCount`, `TotalAmount`, `CreatedAtUtc`). **Не містить масиву позицій**, що суттєво оптимізує мережевий трафік. |
| **Orders** | `OrderDetailsDto` | **Вихідний (Response)** | `GET /api/Orders/{id}`<br>`POST /api/Orders`<br>`PATCH /api/Orders/{id}/status` | Повна детальна інформація про замовлення, включаючи масив позицій `Items` (`OrderItemDto`), суму знижки `Discount` та назву застосованої політики `AppliedDiscount`. |
| **Orders** | `OrderItemDto` | **Вихідний (Response)** | У складі `OrderDetailsDto` | Позиція замовленого товару (`Id`, `MenuItemId`, `ProductName`, `UnitPrice`, `Quantity`, `Subtotal`). |
| **Orders** | `CreateOrderRequestDto` | **Вхідний (Request)** | `POST /api/Orders` | Створення замовлення гостем (`CustomerName`, список `Items` типу `OrderItemRequestDto`). |
| **Orders** | `OrderItemRequestDto` | **Вхідний (Request)** | У складі `CreateOrderRequestDto` | Позиція для замовлення (`MenuItemId`, `Quantity`). |
| **Orders** | `UpdateOrderStatusRequestDto` | **Вхідний (Request)** | `PATCH /api/Orders/{id}/status` | Зміна життєвого циклу замовлення. Підтримує як назву дії (`prepare`, `ready`, `complete`, `cancel`), так і стан (`Status`). |
| **Orders** | `OrderAnalyticsDto` | **Вихідний (Response)** | `GET /api/Analytics` | Зведена бізнес-статистика (`TotalOrdersCount`, `TotalRevenue`, `LastOrderCreatedAtUtc`, `StatusChangesCount`). |

### 3. Робота маперів (`IMenuMapper`, `IOrderMapper`)
* **Життєвий цикл Transient:** Мапери зареєстровані у DI як `Transient` (`AddTransient<IMenuMapper, MenuMapper>()`, `AddTransient<IOrderMapper, OrderMapper>()`), оскільки вони є легковаговими чистими stateless-компонентами без стану.
* **Централізація перетворень:** У сервісах `MenuService`, `OrderQueryService`, `OrderCommandService` немає жодного ручного створення `new ...Dto(...)`. Увесь мапінг інкапсульовано в маперах:
  * `IMenuMapper.MapToSummary(MenuItem)` та `MapToSummaryList(IEnumerable<MenuItem>)`
  * `IMenuMapper.MapToCategory(string)` — виконує безпечний парсинг рядкової категорії у доменний enum `MenuCategory` з вичерпним повідомленням про помилку (400 Bad Request при невалідному значенні).
  * `IOrderMapper.MapToSummary(Order)` та `MapToSummaryList(IEnumerable<Order>)`
  * `IOrderMapper.MapToDetails(Order)`
  * `IOrderMapper.MapItemToDto(OrderItem)` та `MapItemsToDtoList(IEnumerable<OrderItem>)`
* **Zero Dependencies у домені:** `CoffeeShop.Domain` не містить жодних посилань на мапери чи DTO — залежність спрямована суворо з шару `Application` до `Domain`.

---

## 🛡️ Практична робота №8: Валідація даних у прикладному рівні засобами FluentValidation

У межах ПЗ №8 реалізовано автоматичну перевірку вхідних даних на прикладному рівні (`CoffeeShop.Application`) за допомогою бібліотеки **FluentValidation** із дотриманням принципів Clean Architecture:
- Доменний рівень `CoffeeShop.Domain` залишається повністю чистим (0 пакетів і посилань).
- Валідація DTO виконується автоматично до виклику дій контролерів та прикладних сервісів через конвеєр `FluentValidation.AspNetCore`.
- При некоректних даних клієнт отримує стандартну відповідь `400 Bad Request` у форматі `ValidationProblemDetails` із переліком усіх порушених правил для кожного поля.

### 1. Синтаксична та семантична валідація

У системі чітко розмежовано два рівні валідації:
1. **Синтаксична валідація (структура та базові обмеження полів):**
   - Перевірка обов'язковості заповнення (`NotEmpty`).
   - Межі довжини рядків (наприклад, ім'я клієнта від 2 до 64 символів, назва страви від 2 до 100 символів).
   - Числові діапазони (кількість товару від 1 до 20, ціна від 0.01 до 10 000 грн).
2. **Семантична валідація (бізнес-коректність вхідного запиту):**
   - **Унікальність позицій:** У межах одного замовлення неприпустиме дублювання одного й того самого `MenuItemId`.
   - **Сумарний ліміт позицій:** Загальна кількість одиниць товару в замовленні не може перевищувати 50.
   - **Допустимість категорії:** Рядкова категорія страви меню повинна успішно транслюватися у валідний `MenuCategory` (`HotDrink`, `ColdDrink`, `Dessert`, `Snack`).
   - **Допустимість дії над статусом:** Дія зміни статусу повинна входити до списку дозволених переходів (`prepare`, `ready`, `complete`, `cancel` тощо).

### 2. Реалізовані валідатори (CoffeeShop.Application)

| Валідатор | Цільовий DTO | Синтаксичні правила | Семантичні правила |
| :--- | :--- | :--- | :--- |
| `OrderItemRequestDtoValidator` | `OrderItemRequestDto` | • `MenuItemId` не порожній GUID.<br>• `Quantity` від 1 до 20. | — |
| `CreateOrderRequestDtoValidator` | `CreateOrderRequestDto` | • `CustomerName` обов'язкове, довжина 2..64 символи.<br>• `Items` не порожній список.<br>• Кожен елемент валідується через `OrderItemRequestDtoValidator`. | • Заборонено дублювання однакового `MenuItemId` у кошику.<br>• Загальна сума всіх `Quantity` <= 50. |
| `UpdateOrderStatusRequestDtoValidator` | `UpdateOrderStatusRequestDto` | • `TargetAction` обов'язкове, непорожнє. | • Значення `TargetAction` має бути одним із дозволених переходів: `prepare`, `ready`, `complete`, `cancel`, `preparing`, `completed`, `cancelled`. |
| `CreateMenuItemRequestDtoValidator` | `CreateMenuItemRequestDto` | • `Name` обов'язкове, довжина 2..100 символів.<br>• `Price` строго > 0 та <= 10 000 грн. | • `Category` повинна бути однією з валідних категорій меню (`HotDrink`, `ColdDrink`, `Dessert`, `Snack`). |
| `UpdateMenuItemPriceRequestDtoValidator` | `UpdateMenuItemPriceRequestDto` | • `Price` строго > 0 та <= 10 000 грн. | — |

### 3. Автоматична реєстрація та інтеграція в ASP.NET Core

1. **Реєстрація у збірці Application (`DependencyInjection.cs`):**
   ```csharp
   services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
   ```
   Всі валідатори автоматично скануються та реєструються у DI-контейнері як `Scoped`.

2. **Підключення авто-валідації у WebApi (`Program.cs`):**
   ```csharp
   builder.Services.AddControllers();
   builder.Services.AddFluentValidationAutoValidation();
   ```
   При надходженні HTTP-запиту валідація викликається автоматично перед виконанням коду дій у контролерах. Якщо модель не пройшла перевірку, контролер навіть не викликається, а повертається стандартизований `400 Bad Request` із структурою `ValidationProblemDetails`.

### 4. Приклад відповіді при помилці валідації (HTTP 400)

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "CustomerName": [
      "Ім'я клієнта обов'язкове для замовлення."
    ],
    "Items": [
      "Замовлення повинно містити хоча б один товар.",
      "Замовлення містить дублікати однієї позиції меню. Об'єднайте кількість в один рядок."
    ]
  }
}
```

---

## ⚡ Практична робота №9: Кешування в архітектурі програмних систем (Memcached)

У межах ПЗ №9 реалізовано шар розподіленого кешування на базі **Memcached** із застосуванням патерну **Cache-Aside**, відмовостійкістю (resilience) та адресною інвалідацією кешу:

### 1. Архітектурне розмежування (Clean Architecture & DIP)
- **Інтерфейс-абстракція (`CoffeeShop.Application`):** Створено контракт [`ICacheService`](file:///Users/violetta/Documents/CoffeeShop/src/CoffeeShop.Application/Common/Caching/ICacheService.cs) із методами `GetAsync<T>`, `SetAsync<T>` та `RemoveAsync`. Шар бізнес-логіки не залежить від бібліотеки `EnyimMemcachedCore`.
- **Реалізація (`CoffeeShop.Infrastructure`):** Клас [`MemcachedCacheService`](file:///Users/violetta/Documents/CoffeeShop/src/CoffeeShop.Infrastructure/Caching/MemcachedCacheService.cs) використовує клієнт `IMemcachedClient`, `System.Text.Json` для крос-платформної серіалізації та структуроване логування (`[CACHE HIT]`, `[CACHE MISS]`, `[CACHE SET]`, `[CACHE INVALIDATE]`).
- **Domain чистий:** `CoffeeShop.Domain` не містить жодних пакетів чи посилань на кеш.

### 2. Патерн Cache-Aside (Lazy Loading) та життєвий цикл (TTL)

Патерн Cache-Aside реалізовано у прикладних сервісах:
- **Читання (OrderQueryService, MenuService):**
  1. Перевірка наявності даних у кеші за відповідним ключем (`GetAsync`).
  2. При влучанні (**Cache Hit**) — миттєве повернення десеріалізованого DTO клієнту без звернення до репозиторію.
  3. При промаху (**Cache Miss**) — читання актуальних сутностей зі сховища, мапінг у DTO, збереження в Memcached (`SetAsync`) із часом життя **TTL = 60 секунд** та повернення клієнту.
- **Модифікація та адресна інвалідація (OrderCommandService, MenuService):**
  - При оновленні статусу замовлення або модифікації меню після успішного коміту транзакції (`await _unitOfWork.SaveChangesAsync(ct)`) відповідні ключі явно видаляються з кешу через `RemoveAsync`.

### 3. Схема ключів кешування та стратегія інвалідації

| Сервіс | Ключ кешу | Тип даних | TTL | Тригер інвалідації |
| :--- | :--- | :--- | :--- | :--- |
| `MenuService` | `menu:all` | `IReadOnlyList<MenuItemSummaryDto>` | 60 с | `CreateAsync`, `UpdatePriceAsync`, `UpdateAvailabilityAsync` |
| `MenuService` | `menu:item:{id}` | `MenuItemSummaryDto` | 60 с | `CreateAsync`, `UpdatePriceAsync`, `UpdateAvailabilityAsync` |
| `OrderQueryService` | `order:{id}` | `OrderDetailsDto` | 60 с | `OrderCommandService.ChangeStatusAsync` |

### 4. Відмовостійкість (Resilience & Graceful Fallback)
- Усі виклики `IMemcachedClient` загорнуті в блок `try-catch`.
- Якщо сервер Memcached не запущено або виникла мережева помилка, `MemcachedCacheService` фіксує `[CACHE WARNING]` у логах і виконує **graceful fallback** — запит безперебійно продовжує виконуватися через основне сховище даних (`InMemoryDataStore`), не повертаючи помилок користувачеві.

### 5. Компроміси архітектури кешування
- **Швидкодія vs Узгодженість (Latency vs Consistency):** Cache-Aside з адресною інвалідацією мінімізує мережеве навантаження на сховище та затримку читання, гарантуючи негайну інвалідацію кешу при змінах. TTL 60 секунд виступає запобіжником від розсинхронізації у разі збоїв видалення.
- **Пам'ять vs Мережа:** Збереження нормалізованих JSON-рядків усуває ризики двійкової несумісності типів та дозволяє легко горизонтально масштабувати пул вузлів Memcached.

### 6. Швидкий запуск Memcached через Docker Compose

Для локального тестування розподіленого кешу в корені проєкту підготовлено `docker-compose.yml`:
```bash
docker compose up -d
```

---

## 📝 Технології

- .NET 8 / ASP.NET Core Web API
- EnyimMemcachedCore 3.5.x (Distributed Caching Client)
- Docker & Docker Compose (Memcached 1.6-alpine)
- FluentValidation 11.x & FluentValidation.AspNetCore 11.x
- Swagger / OpenAPI (Swashbuckle)
- InMemory-сховище (ConcurrentDictionary) — проєкт запускається без налаштування БД
- In-Process Event Bus (Event-Driven Architecture)
