using System.Text;
using CafeApi.Data;
using CafeApi.Interfaces;
using CafeApi.Middleware;
using CafeApi.Repositories;
using CafeApi.Services;
using CafeApi.Services.CustomerPromotionService;
using CafeApi.Services.CustomerService;
using CafeApi.Services.MenuItemService;
using CafeApi.Services.OrderService;
using CafeApi.Services.PromotionService;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<ICustomerPromotionService, CustomerPromotionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<CafeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();

    if (db.Database.IsNpgsql())
    {
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
builder.Services.AddOpenApi(options =>
{
    var serverUrl = builder.Configuration["ApiServerUrl"];
    if (!string.IsNullOrEmpty(serverUrl))
    {
        options.AddDocumentTransformer((document, context, ct) =>
        {
            document.Servers = [new OpenApiServer { Url = serverUrl }];
            return Task.CompletedTask;
        });
    }
});

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.UseCors("front");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();