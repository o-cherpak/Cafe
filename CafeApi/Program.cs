using CafeApi.Data;
using CafeApi.Extensions;
using CafeApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/cafe-.txt", rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 5,
        fileSizeLimitBytes: 10_000_000
    )
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers();

builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOpenApiDocumentation(builder.Configuration);
builder.Services.AddCorsPolicy();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();

    if (db.Database.IsNpgsql())
    {
        db.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("App Server")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication(
                "Bearer",
                auth => { auth.Token = ""; }
            )
            .EnablePersistentAuthentication();
    });
}

app.UseExceptionHandler();
app.UseSerilogRequestLogging();

app.UseCors("front");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();