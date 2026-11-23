using CoffeeShop.Application;
using CoffeeShop.Infrastructure;
using CoffeeShop.WebApi.Middleware;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Composition Root: Модульна реєстрація залежностей по шарах ──
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// ── ASP.NET Core & FluentValidation ──
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CoffeeShop API",
        Version = "v1",
        Description = "API кав'ярні — практична робота: Чиста архітектура, DDD, Event-Driven, SOLID та Dependency Injection"
    });
});

var app = builder.Build();

// ── Middleware Pipeline ──
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoffeeShop API v1"));

app.MapControllers();

app.Run();
