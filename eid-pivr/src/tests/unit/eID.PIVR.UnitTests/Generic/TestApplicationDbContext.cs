//using eID.PIVR.Contracts.Commands;
using eID.PIVR.Service.Database;
//using eID.PIVR.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace eID.PIVR.UnitTests.Generic;

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
