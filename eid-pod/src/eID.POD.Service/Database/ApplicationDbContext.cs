using eID.POD.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace eID.POD.Service.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dataset> Datasets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        CreateDatasets(modelBuilder);
    }

    private static void CreateDatasets(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dataset>(entity =>
        {
            entity.HasKey(od => od.Id);
            entity.Property(f => f.DatasetName).HasMaxLength(100).IsRequired();
            entity.Property(f => f.CronPeriod).HasMaxLength(100).IsRequired();
            entity.Property(f => f.DataSource).IsRequired();
            entity.Property(f => f.DatasetUri).IsRequired(false);
        });
    }

    public void Seed(ILogger logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }
        try
        {
            SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Seed method failed");
        }
    }
}

public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=eid-pod;Username=postgres;Password=eid$pass");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
