using System.Text;
using System.Threading.RateLimiting;
using CafeApi.Data;
using CafeApi.Helpers;
using CafeApi.Interfaces;
using CafeApi.Repositories;
using CafeApi.Services;
using CafeApi.Services.BonusesService;
using CafeApi.Services.CustomerPromotionService;
using CafeApi.Services.CustomerService;
using CafeApi.Services.MenuItemService;
using CafeApi.Services.OrderService;
using CafeApi.Services.PromotionService;
using CafeApi.Services.TokenService;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace CafeApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection services, IConfiguration configuration
    )
    {
        services.AddDbContext<CafeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(Program).Assembly); });

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();

        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<ICustomerPromotionService, CustomerPromotionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBonusesService, BonusesService>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            )
            .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                    };
                }
            );

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddOpenApiDocumentation(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOpenApi(
            "v1",
            options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); }
        );

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, ct) =>
            {
                var serverUrl = configuration["ApiServerUrl"];
                if (!string.IsNullOrEmpty(serverUrl))
                {
                    document.Servers = [new OpenApiServer { Url = serverUrl }];
                }

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("front", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddTokenBucketLimiter("global", opt =>
            {
                opt.TokenLimit = 50;
                opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
                opt.TokensPerPeriod = 25;
                opt.QueueLimit = 2;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            options.AddFixedWindowLimiter("auth", opt =>
            {
                opt.Window = TimeSpan.FromSeconds(30);
                opt.PermitLimit = 5;
                opt.QueueLimit = 0;
            });
        });

        return services;
    }
}