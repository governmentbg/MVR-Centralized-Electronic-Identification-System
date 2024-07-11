using eID.PUN.Service.Database;
using Microsoft.EntityFrameworkCore;

namespace eID.PUN.UnitTests.Generic;

public class TestApplicationDbContext : ApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.OpenConnection();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
