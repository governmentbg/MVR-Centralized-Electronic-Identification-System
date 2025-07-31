using eID.RO.Service.Entities;
using eID.RO.Service.Extensions;
using eID.RO.Service.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace eID.RO.Service.Database;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<EmpowermentStatement> EmpowermentStatements { get; set; }
    public virtual DbSet<EmpoweredUid> EmpoweredUids { get; set; }
    public virtual DbSet<AuthorizerUid> AuthorizerUids { get; set; }
    public virtual DbSet<EmpowermentWithdrawalReason> EmpowermentWithdrawalReasons { get; set; }
    public virtual DbSet<EmpowermentWithdrawal> EmpowermentWithdrawals { get; set; }
    public virtual DbSet<EmpowermentDisagreementReason> EmpowermentDisagreementReasons { get; set; }
    public virtual DbSet<EmpowermentDisagreement> EmpowermentDisagreements { get; set; }
    public virtual DbSet<EmpowermentSignature> EmpowermentSignatures { get; set; }
    public virtual DbSet<StatusHistoryRecord> EmpowermentStatusHistory { get; set; }
    public virtual DbSet<ScheduledJobSetting> ScheduledJobSettings { get; set; }
    public virtual DbSet<EmpowermentTimestamp> EmpowermentTimestamps { get; set; }
    public virtual DbSet<NumberRegister> NumbersRegister { get; set; }

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
        modelBuilder.UseEncryption();

        CreateEmpowermentStatements(modelBuilder);
        CreateAuthorizedUids(modelBuilder);
        CreateEmpoweredUids(modelBuilder);
        CreateEmpowermentWithdrawReasons(modelBuilder);
        CreateEmpowermentWithdraws(modelBuilder);
        CreateEmpowermentDisagreementReasons(modelBuilder);
        CreateEmpowermentDisagreements(modelBuilder);
        CreateEmpowermentSignatures(modelBuilder);
        CreateEmpowermentStatusHistory(modelBuilder);
        CreateScheduledJobSettingsConfiguration(modelBuilder);
        CreateEmpowermentTimestamps(modelBuilder);
        CreateNumberRegisters(modelBuilder);
    }

    private static void CreateEmpowermentStatements(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmpowermentStatement>(entity =>
        {
            entity.ToTable("EmpowermentStatements").HasKey(f => f.Id);

            entity.Property(f => f.Number).IsRequired().HasMaxLength(32); // РО2147483648/10.11.2024 (24)
            entity.Property(f => f.Uid).IsRequired();
            entity.HasIndex(f => f.Uid);
            entity.Property(f => f.UidType).IsRequired();
            entity.Property(f => f.Name).IsRequired();
            entity.Property(f => f.OnBehalfOf);

            entity
                .HasMany(f => f.AuthorizerUids)
                .WithOne(f => f.EmpowermentStatement)
                .HasForeignKey(f => f.EmpowermentStatementId)
                .HasPrincipalKey(f => f.Id);

            entity
                .HasMany(f => f.EmpoweredUids)
                .WithOne(f => f.EmpowermentStatement)
                .HasForeignKey(f => f.EmpowermentStatementId)
                .HasPrincipalKey(f => f.Id);

            entity.Property(f => f.ProviderId).IsRequired();
            entity.Property(f => f.ProviderName).IsRequired();
            entity.Property(f => f.ServiceId).IsRequired();
            entity.Property(f => f.ServiceName).IsRequired();
            entity.Property(f => f.VolumeOfRepresentation).IsRequired().HasColumnType("jsonb");
            entity.Property(f => f.Status);
            entity.Property(f => f.StartDate).IsRequired();
            entity.Property(f => f.ExpiryDate);
            entity.Property(f => f.XMLRepresentation).IsRequired();

            entity.Property(f => f.CreatedOn);
            entity.Property(f => f.CreatedBy).HasMaxLength(64);

            entity
                .HasMany(f => f.EmpowermentWithdrawals)
                .WithOne(f => f.EmpowermentStatement)
                .HasForeignKey(f => f.EmpowermentStatementId)
                .HasPrincipalKey(f => f.Id);

            entity
                .HasMany(f => f.EmpowermentDisagreements)
                .WithOne(f => f.EmpowermentStatement)
                .HasForeignKey(f => f.EmpowermentStatementId)
                .HasPrincipalKey(f => f.Id);

            entity
                .HasOne(f => f.Timestamp)
                .WithOne(f => f.EmpowermentStatement)
                .HasForeignKey<EmpowermentTimestamp>(f => f.EmpowermentStatementId)
                .IsRequired(false);

            entity.Property(f => f.DenialReasonComment).HasMaxLength(256);
        });
    }
    private static void CreateAuthorizedUids(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthorizerUid>(entity =>
        {
            entity.ToTable("EmpowermentStatements.AuthorizerUids")
                .HasKey(f => f.Id);

            entity.Property(f => f.Uid).IsRequired();
            entity.HasIndex(f => f.Uid);
            entity.Property(f => f.UidType).IsRequired();

            entity.Property(f => f.Name).HasMaxLength(200).IsRequired();
        });
    }
    private static void CreateEmpoweredUids(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmpoweredUid>(entity =>
        {
            entity.ToTable("EmpowermentStatements.EmpoweredUids")
                .HasKey(f => f.Id);

            entity.Property(f => f.Uid).IsRequired();
            entity.HasIndex(f => f.Uid);
            entity.Property(f => f.UidType).IsRequired();
            entity.Property(f => f.Name).HasMaxLength(200).IsRequired();
        });
    }

    private static void CreateEmpowermentWithdrawReasons(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmpowermentWithdrawalReason>(entity =>
        {
            entity.ToTable("EmpowermentStatements.WithdrawalReasons")
                .HasKey(f => f.Id);

            entity.Property(f => f.Translations).IsRequired().HasColumnType("jsonb");
        });
    }

    private static void CreateEmpowermentWithdraws(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmpowermentWithdrawal>(entity =>
        {
            entity.ToTable("EmpowermentStatements.Withdrawals").HasKey(f => f.Id);

            entity.Property(f => f.StartDateTime).IsRequired();
            entity.Property(f => f.ActiveDateTime).IsRequired(false);
            entity.Property(f => f.IssuerUid).IsRequired();
            entity.Property(f => f.IssuerUidType).IsRequired();
            entity.Property(f => f.Reason).IsRequired().HasMaxLength(256);
            entity.Property(f => f.Status).IsRequired();
            entity.Property(f => f.TimestampData);
        });
    }

    private static void CreateEmpowermentDisagreementReasons(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmpowermentDisagreementReason>(entity =>
        {
            entity.ToTable("EmpowermentStatements.DisagreementReasons")
                .HasKey(f => f.Id);

            entity.Property(f => f.Translations).IsRequired().HasColumnType("jsonb");
        });
    }

    private static void CreateEmpowermentDisagreements(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmpowermentDisagreement>(entity =>
        {
            entity.ToTable("EmpowermentStatements.Disagreements").HasKey(f => f.Id);

            entity.Property(f => f.ActiveDateTime).IsRequired();
            entity.Property(f => f.IssuerUid).IsRequired();
            entity.Property(f => f.IssuerUidType).IsRequired();
            entity.Property(f => f.Reason).IsRequired().HasMaxLength(256);
            entity.Property(f => f.TimestampData);
        });
    }

    private static void CreateEmpowermentSignatures(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmpowermentSignature>(entity =>
        {
            entity.ToTable("EmpowermentStatements.Signatures").HasKey(f => f.Id);

            entity.Property(f => f.DateTime).IsRequired();
            entity.Property(f => f.SignerUid).IsRequired();
            entity.Property(f => f.SignerUidType).IsRequired();
            entity.Property(f => f.Signature).IsRequired();
            entity.HasIndex(f => new { f.EmpowermentStatementId, f.SignerUid }).IsUnique();
        });
    }

    private static void CreateEmpowermentStatusHistory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatusHistoryRecord>(entity =>
        {
            entity.ToTable("EmpowermentStatements.StatusHistory").HasKey(f => f.Id);

            entity.Property(f => f.Id).IsRequired();
            entity.Property(f => f.EmpowermentStatementId).IsRequired();
            entity.Property(f => f.DateTime).IsRequired();
            entity.Property(f => f.Status).IsRequired();

            entity.HasIndex(e => new { e.Status, e.DateTime });
        });
    }

    private static void CreateScheduledJobSettingsConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduledJobSetting>(entity =>
        {
            entity.ToTable("ScheduledJobSettings").HasKey(f => f.Id);
            entity.Property(f => f.JobName).IsRequired();
            entity.HasIndex(f => f.JobName);

            entity.Property(f => f.JobSettings).IsRequired().HasColumnType("jsonb");
        });
    }

    private static void CreateEmpowermentTimestamps(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmpowermentTimestamp>(entity =>
        {
            entity.ToTable("EmpowermentStatements.Timestamps").HasKey(f => f.Id);

            entity.Property(f => f.DateTime).IsRequired();
            entity.Property(f => f.Data).IsRequired();
            entity.HasIndex(f => new { f.EmpowermentStatementId }).IsUnique();
        });
    }


    private void CreateNumberRegisters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NumberRegister>(entity =>
        {
            entity.ToTable(NumberRegister.TableName).HasKey(f => f.Id);

            entity.Property(f => f.Id).IsRequired().HasMaxLength(10);
            entity.Property(f => f.Current).IsRequired();
            entity.Property(f => f.LastChange).IsRequired();

            entity.HasData(new NumberRegister
            {
                Id = NumberRegister.EmpowermentNumberId,
                Current = 0,
                LastChange = DateOnly.FromDateTime(DateTime.Now)
            });
        });
    }

    public void Seed(ILogger logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        // Test purposes only, once we've received data by MVR we will populate it.
        if (!EmpowermentWithdrawalReasons.Any())
        {
            EmpowermentWithdrawalReasons.Add(
                new EmpowermentWithdrawalReason
                {
                    Id = Guid.NewGuid(),
                    Translations = new[]
                    {
                        new EmpowermentWithdrawalReasonTranslation
                        {
                            Language = "bg",
                            Name = "Вече не е необходимо",
                        },
                        new EmpowermentWithdrawalReasonTranslation
                        {
                            Language = "en",
                            Name = "No longer needed",
                        }
                    }
                });
        }

        // Test purposes only, once we've received data by MVR we will populate it.
        if (!EmpowermentDisagreementReasons.Any())
        {
            EmpowermentDisagreementReasons.Add(
                new EmpowermentDisagreementReason
                {
                    Id = Guid.NewGuid(),
                    Translations = new[]
                    {
                        new EmpowermentDisagreementReasonTranslation
                        {
                            Language = "bg",
                            Name = "Няма да бъде ползвано",
                        },
                        new EmpowermentDisagreementReasonTranslation
                        {
                            Language = "en",
                            Name = "It will not be used",
                        }
                    }
                });
        }

        // Default configuration seed for Expiring Empowerment statements
        if (!ScheduledJobSettings.Any())
        {
            ScheduledJobSettings.Add(
                new ScheduledJobSetting
                {
                    Id = Guid.NewGuid(),
                    JobName = nameof(ExpiringEmpowermentsNotificationJob),
                    JobSettings = "{ \"DaysUntilExpiration\" : 4 }"
                });
        }

        try
        {
            SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error occurred during seed");
        }
    }
}

public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=eid-ro;Username=postgres;Password=eid$pass");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
