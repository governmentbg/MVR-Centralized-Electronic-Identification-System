using eID.PUN.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace eID.PUN.Service.Database;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<Carrier> Carriers { get; set; }


    /// <summary>
    /// Only for test purposes
    /// </summary>
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        CreateConfigurations(modelBuilder);
    }

    private static void CreateConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carrier>(entity =>
        {
            entity.ToTable("Carriers").HasKey(h => h.Id);
            entity.Property(h => h.SerialNumber).IsRequired();
            entity.Property(h => h.Type).IsRequired().HasMaxLength(100);
            entity.Property(h => h.CertificateId).IsRequired();
            entity.Property(h => h.EId).IsRequired();
            entity.Property(h => h.UserId);
            entity.Property(f => f.ModifiedBy).HasMaxLength(64);
        });
    }
}

public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=eid-pun;Username=postgres;Password=eid$pass");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
