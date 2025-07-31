using eID.MIS.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eID.MIS.Service.Database;

public class PaymentsDbContext : DbContext
{
    public virtual DbSet<PaymentRequest> PaymentRequests { get; set; }
    public PaymentsDbContext()
    {
    }

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentRequest>(entity =>
        {
            entity.ToTable("PaymentRequests").HasKey(f => f.Id);
            entity.Property(f => f.EPaymentId).IsRequired();
            entity.Property(f => f.CitizenProfileId).IsRequired();
            entity.HasIndex(f => f.CitizenProfileId);


            entity.Property(f => f.CreatedOn).IsRequired();
            entity.Property(f => f.PaymentDate);
            entity.Property(f => f.PaymentDeadline).IsRequired();
            entity.Property(f => f.Status);
            entity.Property(f => f.InitiatorSystemName).IsRequired();
            entity.Property(f => f.Currency).IsRequired().HasMaxLength(10);
            entity.Property(f => f.Amount);
            entity.Property(f => f.Reason).IsRequired().HasMaxLength(200);
            entity.Property(f => f.ReferenceNumber).IsRequired().HasMaxLength(50);
        });
    }
}

public class PaymentsDbContextDesignFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql("Host=localhost;Database=eid-misep;Username=postgres;Password=eid$pass");

        return new PaymentsDbContext(optionsBuilder.Options);
    }
}
