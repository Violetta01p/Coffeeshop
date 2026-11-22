using CoffeeShop.Infrastructure;
using CoffeeShop.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Шар інфраструктури (репозиторії, gateway, сервіси) ──
builder.Services.AddInfrastructure();

// ── ASP.NET Core ──
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CoffeeShop API",
        Version = "v1",
        Description = "API кав'ярні — практична робота: Чиста архітектура та DDD"
    });
});

var app = builder.Build();

// ── Middleware Pipeline ──
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoffeeShop API v1"));

app.MapControllers();

app.Run();
