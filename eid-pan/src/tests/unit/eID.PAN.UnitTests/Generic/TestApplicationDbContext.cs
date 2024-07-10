using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace eID.PAN.UnitTests.Generic;

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
        modelBuilder.Entity<RegisteredSystem>().Property(p => p.Translations)
            .HasConversion(
                v => Newtonsoft.Json.JsonConvert.SerializeObject(v),
                v => Newtonsoft.Json.JsonConvert.DeserializeObject<List<RegisteredSystemTranslation>>(v));

        modelBuilder.Entity<RegisteredSystemRejected>().Property(p => p.Translations)
            .HasConversion(
                v => Newtonsoft.Json.JsonConvert.SerializeObject(v),
                v => Newtonsoft.Json.JsonConvert.DeserializeObject<List<RegisteredSystemTranslation>>(v));

        modelBuilder.Entity<SystemEvent>().Property(p => p.Translations)
            .HasConversion(
                v => Newtonsoft.Json.JsonConvert.SerializeObject(v),
                v => Newtonsoft.Json.JsonConvert.DeserializeObject<List<Translation>>(v));

        modelBuilder.Entity<NotificationChannel>().Property(p => p.Translations)
            .HasConversion(
                v => Newtonsoft.Json.JsonConvert.SerializeObject(v),
                v => Newtonsoft.Json.JsonConvert.DeserializeObject<List<NotificationChannelTranslation>>(v));
    }
}
