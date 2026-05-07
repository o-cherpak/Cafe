using CafeApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<CafeDbContext>)
            );

            if (descriptor != null)
                services.Remove(descriptor);
            
            var dbConnectionDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(System.Data.Common.DbConnection));

            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<CafeDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
                options.UseInternalServiceProvider(serviceProvider);
            });

        });
    }
}