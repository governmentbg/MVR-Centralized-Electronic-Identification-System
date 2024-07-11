using eID.POD.Service.Database;
using eID.POD.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace eID.POD.UnitTests.Generic;

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
        modelBuilder.Entity<Dataset>().Property(es => es.DatasetUri).IsRequired(false);
    }
}

