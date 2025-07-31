using eID.PIVR.Contracts.Enums;
using eID.PIVR.Service.Entities;
using eID.PIVR.Service.RegiXResponses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace eID.PIVR.Service.Database;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<DateOfDeath> DatesOfDeath { get; set; }
    public virtual DbSet<DateOfProhibition> DatesOfProhibition { get; set; }
    public virtual DbSet<IdChange> IdChanges { get; set; }
    public virtual DbSet<StatutChange> StatutChanges { get; set; }
    public DbSet<ApiUsageStat> ApiUsageStats { get; set; }


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

        CreateDateOfDeath(modelBuilder);
        CreateProhibitionList(modelBuilder);
        CreateIdChange(modelBuilder);
        CreateStatutChange(modelBuilder);
        CreateApiUsageStatistics(modelBuilder);
    }

    private static void CreateDateOfDeath(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DateOfDeath>(entity =>
        {
            entity.ToTable("DatesOfDeath")
                .HasKey(f => f.Id);
            entity.Property(f => f.Id)
                .UseIdentityByDefaultColumn();

            entity.Property(f => f.PersonalId)
                .IsRequired();

            entity.Property(f => f.Date);
            
            entity.Property(f => f.CreatedOn)
                .IsRequired()
                .HasDefaultValueSql("now() at time zone 'utc'");

            entity.Property(f => f.UidType)
                .IsRequired()
                .HasDefaultValue(UidType.EGN);

            entity.HasIndex(f => new { f.PersonalId, f.CreatedOn })
                .IsDescending(false, true);
        });
    }

    private static void CreateProhibitionList(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DateOfProhibition>(entity =>
        {
            entity.ToTable("DatesOfProhibition").HasKey(f => f.Id);
            
            entity.Property(f => f.PersonalId)
                .IsRequired();
            
            entity.Property(b => b.Date);
            
            entity.Property(f => f.CreatedOn)
                .HasDefaultValueSql("now() at time zone 'utc'");

            entity.Property(f => f.UidType)
                .IsRequired()
                .HasDefaultValue(UidType.EGN);

            entity.Property(f => f.TypeOfProhibition)
                .IsRequired()
                .HasDefaultValue(ProhibitionType.Full);

            entity.Property(f => f.DescriptionOfProhibition)
                .HasMaxLength(256);
        });
    }

    private void CreateIdChange(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdChange>(entity =>
        {
            entity.ToTable("IDChanges").HasKey(f => f.Id);
            entity.Property(f => f.Id)
                .UseIdentityByDefaultColumn();

            entity.Property(f => f.OldPersonalId).IsRequired().HasMaxLength(DBConstraints.UidLength);
            entity.Property(f => f.OldUidType).IsRequired();
            entity.Property(f => f.NewPersonalId).IsRequired().HasMaxLength(DBConstraints.UidLength);
            entity.Property(f => f.NewUidType).IsRequired();
            entity.Property(f => f.Date).IsRequired();
            entity.Property(f => f.CreatedOn).IsRequired()
                .HasDefaultValueSql("now() at time zone 'utc'");
        });
    }

    private void CreateStatutChange(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatutChange>(entity =>
        {
            entity.ToTable("StatutChanges").HasKey(f => f.Id);
            entity.Property(f => f.Id)
                .UseIdentityByDefaultColumn();

            entity.Property(f => f.PersonalId).IsRequired().HasMaxLength(DBConstraints.UidLength);
            entity.Property(f => f.UidType).IsRequired();
            entity.Property(f => f.Date).IsRequired();
            entity.Property(f => f.CreatedOn).IsRequired()
                .HasDefaultValueSql("now() at time zone 'utc'");
        });
    }
    private void CreateApiUsageStatistics(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiUsageStat>(entity =>
        {
            entity.ToTable("ApiUsageStatistics").HasKey(f => f.Id);
            entity.Property(f => f.Id)
                .UseIdentityByDefaultColumn();

            entity.Property(f => f.RegistryKey).IsRequired();
            entity.Property(f => f.Date).IsRequired();
            entity.Property(f => f.Count);
            entity.HasIndex(f => new { f.RegistryKey, f.Date }).IsUnique();
        });
    }
}

public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Design time is only for development purposes only
        var environment = "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../eID.PIVR.Application"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Could not find a connection string named 'DefaultConnection'.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
