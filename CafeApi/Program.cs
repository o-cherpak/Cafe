using System.Text;
using CafeApi.Data;
using CafeApi.Helpers;
using CafeApi.Interfaces;
using CafeApi.Middleware;
using CafeApi.Repositories;
using CafeApi.Services;
using CafeApi.Services.BonusesService;
using CafeApi.Services.CustomerPromotionService;
using CafeApi.Services.CustomerService;
using CafeApi.Services.MenuItemService;
using CafeApi.Services.OrderService;
using CafeApi.Services.PromotionService;
using CafeApi.Services.TokenService;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/cafe-.txt", rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 5,
        fileSizeLimitBytes: 10_000_000)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(Program).Assembly); });

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<ICustomerPromotionService, CustomerPromotionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBonusesService, BonusesService>();

builder.Services.AddDbContext<CafeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddOpenApi("v1",
    options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("front", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        var serverUrl = builder.Configuration["ApiServerUrl"];
        if (!string.IsNullOrEmpty(serverUrl))
        {
            document.Servers = [new OpenApiServer { Url = serverUrl }];
        }
        return Task.CompletedTask;
    });
});

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