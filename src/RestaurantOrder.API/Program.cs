using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using RestaurantOrder.API.Middleware;
using RestaurantOrder.Application;
using RestaurantOrder.Infrastructure;
using RestaurantOrder.Infrastructure.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .Enrich.FromLogContext()
           .WriteTo.Console());

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddHealthChecks()
        .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
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
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
