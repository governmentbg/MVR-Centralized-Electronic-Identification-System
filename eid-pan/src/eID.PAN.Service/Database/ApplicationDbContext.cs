using eID.PAN.Contracts;
using eID.PAN.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace eID.PAN.Service.Database;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<SmtpConfiguration> SmtpConfigurations { get; set; }
    public virtual DbSet<RegisteredSystem> RegisteredSystems { get; set; }
    public virtual DbSet<SystemEvent> SystemEvents { get; set; }
    public virtual DbSet<NotificationChannelApproved> NotificationChannels { get; set; }
    public virtual DbSet<NotificationChannelPending> NotificationChannelsPending { get; set; }
    public virtual DbSet<NotificationChannelRejected> NotificationChannelsRejected { get; set; }
    public virtual DbSet<NotificationChannelArchive> NotificationChannelsArchive { get; set; }
    public virtual DbSet<DeactivatedUserEvent> DeactivatedUserEvents { get; set; }
    public virtual DbSet<UserNotificationChannel> UserNotificationChannels { get; set; }
    public virtual DbSet<RegisteredSystemRejected> RegisteredSystemsRejected { get; set; }


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

        CreateRegisteredSystem(modelBuilder);
        CreateSystemEvent(modelBuilder);
        CreateDeactivatedUserEvent(modelBuilder);

        CreateNotificationChannel(modelBuilder);
        CreateNotificationChannelPending(modelBuilder);
        CreateNotificationChannelRejected(modelBuilder);
        CreateNotificationChannelArchive(modelBuilder);

        CreateUserNotificationChannel(modelBuilder);
        CreateRegisteredSystemsRejected(modelBuilder);
        CreateSmtpConfigurations(modelBuilder);
    }

    private static void CreateRegisteredSystem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegisteredSystem>(entity =>
        {
            entity.ToTable("RegisteredSystems")
                .HasKey(f => f.Id);

            entity.Property(f => f.Name).IsRequired().HasMaxLength(64);
            entity.HasIndex(f => f.Name).IsUnique();

            entity.Property(f => f.ModifiedBy).HasMaxLength(64);
            entity.Property(f => f.IsApproved).IsRequired();
            entity.Property(f => f.IsDeleted).IsRequired();

            entity.Property(f => f.Translations).IsRequired().HasColumnType("jsonb");

            entity
                .HasMany(f => f.Events)
                .WithOne(f => f.RegisteredSystem)
                .HasForeignKey(f => f.RegisteredSystemId)
                .HasPrincipalKey(f => f.Id);

            entity.HasIndex(f => new
            {
                f.Name,
                f.IsApproved,
                f.IsDeleted
            });
        });
    }

    private static void CreateSystemEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemEvent>(entity =>
        {
            entity.ToTable("SystemEvents")
                .HasKey(f => f.Id);

            entity.Property(f => f.Code).IsRequired().HasMaxLength(64);
            entity.Property(f => f.ModifiedBy).HasMaxLength(64);
            entity.Property(f => f.IsMandatory).IsRequired();
            entity.Property(f => f.IsDeleted).IsRequired();

            entity.Property(f => f.Translations).IsRequired().HasColumnType("jsonb");

            entity.HasIndex(f => new
            {
                f.RegisteredSystemId,
                f.Code
            }).IsUnique();

            entity.HasIndex(f => new
            {
                f.Code,
                f.IsDeleted
            });
        });
    }

    private static void CreateNotificationChannel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationChannel>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).ValueGeneratedNever();
            entity.Property(f => f.SystemName).IsRequired().HasMaxLength(1024);
            entity.Property(f => f.Name).IsRequired().HasMaxLength(64);
            entity.Property(f => f.Description).HasMaxLength(2048);
            entity.Property(f => f.ModifiedBy).HasMaxLength(64);
            entity.Property(f => f.CallbackUrl).IsRequired().HasMaxLength(2048);
            entity.Property(f => f.Price).IsRequired();
            entity.Property(f => f.Email).IsRequired().HasMaxLength(1024);
            entity.Property(f => f.InfoUrl).IsRequired().HasMaxLength(2048);

            entity.Property(f => f.Translations).IsRequired().HasColumnType("jsonb");
            entity.UseTpcMappingStrategy();
        });
        modelBuilder.Entity<NotificationChannelApproved>(entity =>
        {
            entity.ToTable("NotificationChannels");
            entity.HasIndex(f => f.Name).IsUnique();
        });
    }

    private static void CreateNotificationChannelPending(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationChannelPending>(entity =>
        {
            entity.ToTable("NotificationChannels.Pending");
            entity.HasIndex(f => f.Name).IsUnique();
        });
    }

    private static void CreateNotificationChannelRejected(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationChannelRejected>(entity =>
        {
            entity.ToTable("NotificationChannels.Rejected");
            entity.Property(f => f.Reason)
                .IsRequired()
                .HasMaxLength(FieldLength.NotificationChannel.RejectReason);
        });
    }

    private static void CreateNotificationChannelArchive(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationChannelArchive>(entity =>
        {
            entity.ToTable("NotificationChannels.Archive");
        });
    }

    private static void CreateDeactivatedUserEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeactivatedUserEvent>(entity =>
        {
            entity.ToTable("DeactivatedUserEvents")
                .HasKey(f => f.Id);

            entity.Property(f => f.ModifiedBy).HasMaxLength(64);

            entity
                .HasOne(f => f.Event)
                .WithMany(f => f.DeactivatedUserEvent)
                .HasForeignKey(f => f.SystemEventId)
                .IsRequired();

            entity.HasIndex(f => new
            {
                f.UserId,
                f.SystemEventId
            }).IsUnique();
        });
    }

    private static void CreateUserNotificationChannel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserNotificationChannel>(entity =>
        {
            entity.ToTable("UserNotificationChannels")
                .HasKey(f => f.Id);

            entity.Property(f => f.ModifiedBy).HasMaxLength(64);

            entity
                .HasOne(f => f.Channel)
                .WithMany(f => f.UserNotificationChannels)
                .HasForeignKey(f => f.NotificationChannelId)
                .IsRequired();

            entity.HasIndex(f => new
            {
                f.UserId,
                f.NotificationChannelId
            }).IsUnique();
        });
    }

    public void Seed(ILogger logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        var willSeed = false;
        if (!NotificationChannels.Any(nc => nc.Name == ConfigurationsConstants.SMTP && nc.IsBuiltIn))
        {
            willSeed = true;
            logger.LogInformation("Seeding SMTP channel.");
            NotificationChannels.Add(new NotificationChannelApproved
            {
                Id = Guid.NewGuid(),
                IsBuiltIn = true,
                Name = ConfigurationsConstants.SMTP,
                Description = "Канал за изпращане на известия по електронна поща",
                CallbackUrl = "http://localhost/",
                Price = 0,
                InfoUrl = "http://localhost/",
                Email = "support@localhost",
                SystemName = "eID",
                Translations = new List<NotificationChannelTranslation>
                {
                    new NotificationChannelTranslation
                    {
                        Language = "bg",
                        Name = "Електронна поща",
                        Description = "Канал за изпращане на известия по електронна поща",
                    },
                    new NotificationChannelTranslation
                    {
                        Language = "en",
                        Name = "Email",
                        Description = "Notification delivery channel via email",
                    }
                }
            });
        }

        if (!NotificationChannels.Any(nc => nc.Name == ConfigurationsConstants.SMS && nc.IsBuiltIn))
        {
            willSeed = true;
            logger.LogInformation("Seeding SMS channel.");
            NotificationChannels.Add(new NotificationChannelApproved
            {
                Id = Guid.NewGuid(),
                IsBuiltIn = true,
                Name = ConfigurationsConstants.SMS,
                Description = "Вътрешен канал за изпращане на SMS",
                CallbackUrl = "http://localhost/",
                Price = 0,
                InfoUrl = "http://localhost/",
                Email = "support@localhost",
                SystemName = "eID",
                Translations = new List<NotificationChannelTranslation>
                {
                    new NotificationChannelTranslation
                    {
                        Language = "bg",
                        Name = ConfigurationsConstants.SMS,
                        Description = "Вътрешен канал за изпращане на SMS",
                    },
                    new NotificationChannelTranslation
                    {
                        Language = "en",
                        Name = ConfigurationsConstants.SMS,
                        Description = "Internal channel for sending SMS",
                    }
                }
            });
        }

        if (!NotificationChannels.Any(nc => nc.Name == ConfigurationsConstants.PUSH && nc.IsBuiltIn))
        {
            willSeed = true;
            logger.LogInformation("Seeding PUSH channel.");
            NotificationChannels.Add(new NotificationChannelApproved
            {
                Id = Guid.NewGuid(),
                IsBuiltIn = true,
                Name = ConfigurationsConstants.PUSH,
                Description = "Канал за изпращане на известия през мобилното приложение на централизираната система за електронна идентификация (ЦСЕИ)",
                CallbackUrl = "http://localhost/",
                Price = 0,
                InfoUrl = "http://localhost/",
                Email = "support@localhost",
                SystemName = "eID",
                Translations = new List<NotificationChannelTranslation>
                {
                    new NotificationChannelTranslation
                    {
                        Language = "bg",
                        Name = "Мобилно приложение",
                        Description = "Канал за изпращане на известия през мобилното приложение на централизираната система за електронна идентификация (ЦСЕИ)",
                    },
                    new NotificationChannelTranslation
                    {
                        Language = "en",
                        Name = "Mobile application",
                        Description = "Notification delivery channel via the mobile application of the centralized electronic identification system",
                    }
                }
            });
        }

        try
        {
            if (willSeed)
            {
                logger.LogInformation("Saving seeded information.");
            }
            SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error occurred during channels seed.");
        }
    }

    private static void CreateRegisteredSystemsRejected(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegisteredSystemRejected>(entity =>
        {
            entity.ToTable("RegisteredSystems.Rejected")
                .HasKey(f => f.Id);

            entity.Property(f => f.Name).IsRequired().HasMaxLength(64);

            entity.Property(f => f.RejectedOn).IsRequired();
            entity.Property(f => f.RejectedBy).HasMaxLength(64);

            entity.Property(f => f.Translations).IsRequired().HasColumnType("jsonb");
            
            entity.Property(f => f.Reason)
                .IsRequired()
                .HasMaxLength(FieldLength.RegisteredSystemRejected.RejectReason);
        });
    }

    private static void CreateSmtpConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SmtpConfiguration>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Server).IsRequired();
            entity.Property(b => b.UserName).IsRequired().HasMaxLength(50);
            entity.Property(b => b.Password).IsRequired();
            entity.Property(b => b.SecurityProtocol).IsRequired().HasConversion<string>();

            entity.Property(b => b.CreatedOn).IsRequired();
            entity.Property(b => b.CreatedBy).IsRequired().HasMaxLength(50);
            entity.Property(b => b.ModifiedOn).IsRequired(false);
            entity.Property(b => b.ModifiedBy).IsRequired(false).HasMaxLength(50);
            entity.Property(b => b.DeletedOn).IsRequired(false);
            entity.Property(b => b.DeletedBy).IsRequired(false).HasMaxLength(50);
        });
    }
}

public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=eid-pan;Username=postgres;Password=eid$pass");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
