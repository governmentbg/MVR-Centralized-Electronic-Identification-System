using eID.RO.Service.Database;
using eID.RO.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace eID.RO.UnitTests.Generic;

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
        modelBuilder.Entity<EmpowermentStatement>().Property(es => es.VolumeOfRepresentation)
            .HasConversion(
                v => Newtonsoft.Json.JsonConvert.SerializeObject(v),
                v => Newtonsoft.Json.JsonConvert.DeserializeObject<List<VolumeOfRepresentation>>(v));

        modelBuilder.Entity<EmpowermentWithdrawalReason>().Property(es => es.Translations)
            .HasConversion(
                v => Newtonsoft.Json.JsonConvert.SerializeObject(v),
                v => Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmpowermentWithdrawalReasonTranslation>>(v));

        modelBuilder.Entity<EmpowermentDisagreementReason>().Property(es => es.Translations)
            .HasConversion(
                v => Newtonsoft.Json.JsonConvert.SerializeObject(v),
                v => Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmpowermentDisagreementReasonTranslation>>(v));
    }
}
