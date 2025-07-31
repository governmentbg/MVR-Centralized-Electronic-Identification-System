using eID.MIS.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eID.MIS.Service.Database;

public class DeliveriesDbContext : DbContext
{
    public virtual DbSet<DeliveryRequest> Deliveries { get; set; }
    public DeliveriesDbContext()
    {
    }

    public DeliveriesDbContext(DbContextOptions<DeliveriesDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DeliveryRequest>(entity =>
        {
            entity.ToTable("Deliveries").HasKey(f => f.Id);
            entity.Property(f => f.EIdentityId).IsRequired();
            entity.HasIndex(f => f.EIdentityId);

            entity.Property(f => f.SentOn).IsRequired();
            entity.Property(f => f.SystemName).IsRequired().HasMaxLength(512);
            entity.Property(f => f.Subject).IsRequired().HasMaxLength(512);
            entity.Property(f => f.ReferencedOrn).IsRequired(false).HasMaxLength(128);
            entity.Property(f => f.MessageId).IsRequired();
        });
    }
}

public class DeliveriesDbContextDesignFactory : IDesignTimeDbContextFactory<DeliveriesDbContext>
{
    public DeliveriesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DeliveriesDbContext>()
            .UseNpgsql("Host=localhost;Database=eid-missev;Username=postgres;Password=eid$pass");

        return new DeliveriesDbContext(optionsBuilder.Options);
    }
}
