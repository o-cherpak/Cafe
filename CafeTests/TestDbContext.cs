using CafeApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CafeTests;

public static class TestDbContextFactory
{
    public static CafeDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CafeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CafeDbContext(options);
    }
}