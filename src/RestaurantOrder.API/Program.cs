using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using RestaurantOrder.API.Middleware;
using RestaurantOrder.Application;
using RestaurantOrder.Infrastructure;
using RestaurantOrder.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .Enrich.FromLogContext()
       .WriteTo.Console());

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var healthChecks = builder.Services.AddHealthChecks();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
    healthChecks.AddNpgSql(connectionString);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
    if (app.Environment.IsEnvironment("Testing"))
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapControllers();
app.MapMetrics();
app.MapHealthChecks("/health", new HealthCheckOptions { AllowCachingResponses = false });

app.Run();

public partial class Program { }
