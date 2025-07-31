using eID.Signing.Service.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eID.Signing.Service.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessToken> AccessTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        CreateAccessTokens(modelBuilder);
    }

    private static void CreateAccessTokens(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessToken>(entity =>
        {
            entity.ToTable("AccessTokens").HasKey(x => x.Id);

            entity.Property(x => x.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedOnAdd();

            entity.Property(x => x.AccessTokenValue)
                   .HasColumnName("AccessToken")
            .IsRequired();

            entity.Property(x => x.ExpirationDate)
                   .HasColumnName("ExpirationDate")
                   .HasColumnType("date");

            entity.Property(x => x.Status)
                   .HasColumnName("Status")
                   .HasConversion<int>()
                   .IsRequired();

            entity.Property(x => x.CreatedAt)
                   .HasColumnName("CreatedAt")
                   .HasDefaultValueSql("timezone('utc', now())");

            entity.Property(x => x.UpdatedAt)
                   .HasColumnName("UpdatedAt")
                   .HasDefaultValueSql("timezone('utc', now())");

        });
    }
}


public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=eid-signing;Username=postgres;Password=eid$pass");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
