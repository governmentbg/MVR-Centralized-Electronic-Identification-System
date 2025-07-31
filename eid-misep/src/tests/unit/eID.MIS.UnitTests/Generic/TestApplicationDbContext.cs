using eID.MIS.Service.Database;
using eID.MIS.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace eID.MIS.UnitTests.Generic;

public class TestApplicationDbContext : PaymentsDbContext
{
    public TestApplicationDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
        Database.OpenConnection();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}

