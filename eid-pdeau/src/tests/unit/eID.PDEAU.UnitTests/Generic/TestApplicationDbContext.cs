using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Service.Database;
using eID.PDEAU.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace eID.PDEAU.UnitTests.Generic;

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
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ProviderService>().Property(es => es.RequiredPersonalInformation)
            .HasConversion(
                v => Newtonsoft.Json.JsonConvert.SerializeObject(v),
                v => Newtonsoft.Json.JsonConvert.DeserializeObject<List<CollectablePersonalInformation>>(v));
        modelBuilder.Entity<ProviderService>().Property(f => f.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}

