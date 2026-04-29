using Microsoft.EntityFrameworkCore;

namespace CafeApi;

public class CafeDbContext : DbContext
{
    public CafeDbContext(DbContextOptions options) : base(options)
    {
    }
}