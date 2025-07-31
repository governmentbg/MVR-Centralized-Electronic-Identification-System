using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;

namespace eID.PDEAU.Service.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Provider> Providers { get; set; }
    public virtual DbSet<User> ProvidersUsers { get; set; }
    public virtual DbSet<AISInformation> ProvidersAISInformations { get; set; }
    public virtual DbSet<AdministratorPromotion> ProvidersAdministratorPromotions { get; set; }
    public virtual DbSet<ProviderDetails> ProvidersDetails { get; set; }
    public virtual DbSet<ProviderSection> ProvidersDetailsSections { get; set; }
    public virtual DbSet<ProviderService> ProvidersDetailsServices { get; set; }
    public virtual DbSet<ServiceScope> ServiceScopes { get; set; }
    public virtual DbSet<DefaultServiceScope> DefaultServiceScopes { get; set; }
    public virtual DbSet<ProviderFile> ProvidersFiles { get; set; }
    public virtual DbSet<ProviderStatusHistory> ProvidersStatusHistory { get; set; }
    public virtual DbSet<ProviderOffice> ProvidersOffices { get; set; }
    public virtual DbSet<AdministratorAction> AdministratorActions { get; set; }
    public virtual DbSet<ProviderDoneService> ProviderDoneServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        CreateProviders(modelBuilder);
        CreateUsers(modelBuilder);
        CreateAISInformations(modelBuilder);
        CreateAdministratorPromotion(modelBuilder);
        CreateProvidersDetails(modelBuilder);
        CreateProviderSections(modelBuilder);
        CreateProviderServices(modelBuilder);
        CreateServiceScope(modelBuilder);
        CreateDefaultServiceScopes(modelBuilder);
        CreateProvidersFiles(modelBuilder);
        CreateProviderStatusHistory(modelBuilder);
        CreateProviderOffices(modelBuilder);
        CreateProvidersTimestamps(modelBuilder);
        CreateProvidersNumberRegisters(modelBuilder);
        CreateAdministratorActions(modelBuilder);
        CreateProviderDoneServices(modelBuilder);
    }

    private static void CreateProviders(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Provider>(entity =>
        {
            entity.ToTable("Providers").HasKey(f => f.Id);

            entity.Property(f => f.Number).IsRequired().HasMaxLength(32); // ПДЕАУ00001/16.01.2025 (21)
            entity.Property(f => f.ExternalNumber).IsRequired(false).HasMaxLength(32);
            entity.Property(f => f.IssuerUid).IsRequired().HasMaxLength(13);
            entity.Property(f => f.IssuerUidType).IsRequired();
            entity.Property(f => f.Name).IsRequired().HasMaxLength(200);
            entity.Property(f => f.Bulstat).IsRequired();
            entity.Property(f => f.Headquarters).IsRequired().HasMaxLength(DBConstraints.HeadquartersLength);
            entity.Property(f => f.Address).IsRequired().HasMaxLength(DBConstraints.AddressLength);
            entity.Property(f => f.Email).IsRequired().HasMaxLength(200);
            entity.Property(f => f.Phone).IsRequired().HasMaxLength(200);
            entity.Property(f => f.IdentificationNumber).IsRequired(false).HasMaxLength(200);
            entity.Property(f => f.GeneralInformation).IsRequired().HasMaxLength(Constants.Providers.GeneralInformationMaxLength);
            entity.Property(f => f.XMLRepresentation).IsRequired();

            entity
                .HasMany(f => f.Users)
                .WithOne(f => f.Provider)
                .HasForeignKey(f => f.ProviderId)
                .HasPrincipalKey(f => f.Id);

            modelBuilder.Entity<Provider>()
                .HasOne(p => p.Details)
                .WithMany(pd => pd.Providers)
                .HasForeignKey(p => p.DetailsId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(required: false);
        });
    }

    private static void CreateUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Providers.Users").HasKey(f => f.Id);

            entity.Property(f => f.Uid).IsRequired().HasMaxLength(13);
            entity.Property(f => f.UidType).IsRequired();
            entity.Property(f => f.Name).IsRequired().HasMaxLength(200);
            entity.Property(f => f.EID).IsRequired().HasMaxLength(128);
            entity.Property(f => f.Email).IsRequired(false).HasMaxLength(200);
            entity.Property(f => f.Phone).IsRequired(false).HasMaxLength(200);
            entity.Property(f => f.IsDeleted).IsRequired();

            entity
               .HasOne(f => f.Provider)
               .WithMany(f => f.Users)
               .HasForeignKey(f => f.ProviderId)
               .HasPrincipalKey(f => f.Id);
        });
    }

    private static void CreateAISInformations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AISInformation>(entity =>
        {
            entity.ToTable("Providers.AISInformations").HasKey(f => f.Id);

            entity.Property(f => f.Name).IsRequired().HasMaxLength(200);
            entity.Property(f => f.Project).IsRequired().HasMaxLength(200);
            entity.Property(f => f.SourceIp).IsRequired().HasMaxLength(50);
            entity.Property(f => f.DestinationIp).IsRequired().HasMaxLength(50);
            entity.Property(f => f.ProtocolPort).IsRequired().HasMaxLength(50);

            entity
               .HasOne(f => f.Provider)
               .WithOne(f => f.AISInformation)
               .HasForeignKey<AISInformation>(f => f.ProviderId)
               .IsRequired(false);
        });
    }

    private static void CreateAdministratorPromotion(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdministratorPromotion>(entity =>
        {
            entity.ToTable("Providers.AdministratorPromotions").HasKey(f => f.Id);

            entity.Property(f => f.IssuerId).IsRequired();
            entity.Property(f => f.PromotedUserId).IsRequired().HasMaxLength(200);
            entity.Property(f => f.CreatedOn).IsRequired();
            entity.Property(f => f.Status).IsRequired();
        });
    }

    private static void CreateProvidersDetails(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderDetails>(entity =>
        {
            entity.ToTable("Providers.Details").HasKey(f => f.Id);
            entity.Property(f => f.IdentificationNumber).HasMaxLength(DBConstraints.IdentificationNumberMaxLength); // Max length in IISDA 10
            entity.Property(f => f.Name).IsRequired().HasMaxLength(DBConstraints.ProviderDetails.NameMaxLength);// Max length in IISDA 241
            // IISDA Names are not unique. There are duplicates. Example: "Общинска администрация - Бяла"
            entity.Property(f => f.SyncedFromOnlineRegistry).IsRequired();
            entity.Property(f => f.IsDeleted).IsRequired();
            entity.Property(f => f.Status).IsRequired();
            entity.Property(f => f.UIC).IsRequired().HasMaxLength(DBConstraints.UICMaxLength); // Max length in IISDA is 13
            entity.Property(f => f.Headquarters).IsRequired().HasMaxLength(DBConstraints.HeadquartersLength);
            entity.Property(f => f.Address).IsRequired().HasMaxLength(DBConstraints.AddressLength);
            entity.Property(f => f.WebSiteUrl).IsRequired().HasMaxLength(DBConstraints.ProviderDetails.WebSiteUrlMaxLength);
            entity.Property(f => f.WorkingTimeStart).IsRequired().HasMaxLength(DBConstraints.ProviderDetails.WorkingTimeStartMaxLength);
            entity.Property(f => f.WorkingTimeEnd).IsRequired().HasMaxLength(DBConstraints.ProviderDetails.WorkingTimeEndMaxLength);

            entity.HasIndex(f => f.IdentificationNumber).IsUnique().HasFilter("\"IdentificationNumber\" IS NOT NULL");
            entity.HasIndex(f => new
            {
                f.Name,
                f.IsDeleted
            });
        });
    }

    private static void CreateProviderSections(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderSection>(entity =>
        {
            entity.ToTable("Providers.Details.Sections").HasKey(f => f.Id);
            entity.Property(f => f.Name).IsRequired().HasMaxLength(DBConstraints.ProviderSection.NameMaxLength);// Max length in IISDA 65
            entity.Property(f => f.SyncedFromOnlineRegistry).IsRequired();
            entity.Property(f => f.IsDeleted).IsRequired();

            entity.HasIndex(f => new
            {
                f.Name,
                f.ProviderDetailsId
            }).IsUnique();
            entity.HasIndex(f => new
            {
                f.Name,
                f.IsDeleted
            });
        });
    }

    private static void CreateProviderServices(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderService>(entity =>
        {
            entity.ToTable("Providers.Details.Sections.Services").HasKey(f => f.Id);
            entity.Property(f => f.ServiceNumber).IsRequired();
            entity.Property(f => f.Name).IsRequired().HasMaxLength(DBConstraints.ProviderService.NameMaxLength); // Max length in IISDA 538
            entity.Property(f => f.Description).IsRequired(false).HasMaxLength(DBConstraints.ProviderService.DescriptionMaxLength); // Max length in IISDA 1760
            entity.Property(f => f.PaymentInfoNormalCost).IsRequired(false);
            entity.Property(f => f.IsEmpowerment).IsRequired();
            entity.Property(f => f.SyncedFromOnlineRegistry).IsRequired();
            entity.Property(f => f.IsDeleted).IsRequired();
            entity.Property(f => f.RequiredPersonalInformation)
                .IsRequired(false)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<List<CollectablePersonalInformation>>(x)
                );
            entity.Property(f => f.CreatedOn).IsRequired().HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            entity.Property(f => f.DenialReason).HasMaxLength(DBConstraints.ProviderService.DenialReasonMaxLength);
            entity.Property(f => f.ReviewerFullName).HasMaxLength(DBConstraints.ProviderService.ReviewerFullNameMaxLength);

            entity.HasIndex(f => new
            {
                f.ProviderDetailsId,
                f.ServiceNumber,
            }).IsUnique().HasFilter("\"ServiceNumber\" != 0");
            entity.HasIndex(f => new
            {
                f.ServiceNumber,
                f.Name,
                f.IsEmpowerment,
                f.IsDeleted
            });

            entity
                .HasOne(f => f.ProviderDetails)
                .WithMany(f => f.ProviderServices)
                .HasForeignKey(f => f.ProviderDetailsId)
                .IsRequired();

            entity
                .HasOne(f => f.ProviderSection)
                .WithMany(f => f.ProviderServices)
                .HasForeignKey(f => f.ProviderSectionId)
                .IsRequired();
        });
    }

    private static void CreateServiceScope(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceScope>(entity =>
        {
            entity.ToTable("Providers.Details.Sections.Services.Scopes").HasKey(f => f.Id);
            entity.Property(f => f.Name).IsRequired().HasMaxLength(DBConstraints.ServiceScope.NameMaxLength);
            entity.Property(b => b.CreatedOn).IsRequired();
            entity.Property(b => b.CreatedBy).IsRequired();
            entity.Property(b => b.ModifiedOn).IsRequired(false);
            entity.Property(b => b.ModifiedBy).IsRequired(false);

            entity.HasIndex(f => new
            {
                f.ProviderServiceId,
                f.Name
            }).IsUnique();

            entity
                .HasOne(f => f.ProviderService)
                .WithMany(f => f.ServiceScopes)
                .HasForeignKey(f => f.ProviderServiceId)
                .IsRequired();
        });
    }

    private static void CreateDefaultServiceScopes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DefaultServiceScope>(entity =>
        {
            entity.ToTable("Providers.Details.Sections.Services.Scopes.Defaults").HasKey(f => f.Id);
            entity.Property(f => f.Name).IsRequired().HasMaxLength(DBConstraints.DefaultServiceScope.NameMaxLength);

            entity.HasIndex(f => new
            {
                f.Name
            }).IsUnique();

            entity.HasData(
                new DefaultServiceScope
                {
                    Id = Guid.Parse("C215ECF2-F15C-430D-8061-41F3C0595629"),
                    Name = "Заявяване на услугата"
                },
                new DefaultServiceScope
                {
                    Id = Guid.Parse("42CBEA41-BA99-4223-820B-6FF03B67D56E"),
                    Name = "Заявяване представянето на информация и документи"
                },
                new DefaultServiceScope
                {
                    Id = Guid.Parse("C73D44F7-3FAE-43F3-8413-FD9E18505E75"),
                    Name = "Получаване на съобщения, свързани с електронната административна услуга"
                },
                new DefaultServiceScope
                {
                    Id = Guid.Parse("51994550-9E34-4546-9E33-7DBD586B9532"),
                    Name = "Получаване на резултатите от услугата"
                },
                new DefaultServiceScope
                {
                    Id = Guid.Parse("20AB27DE-C94B-4B35-B596-265DB6E1051C"),
                    Name = "Обжалване на административния акт, резултат от услугата, или на отказа от издаването на такъв"
                },
                new DefaultServiceScope
                {
                    Id = Guid.Parse("2F40F241-98F1-4308-8090-B2EAC2626049"),
                    Name = "Оттегляне на заявлението"
                });
        });
    }

    private static void CreateProvidersFiles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderFile>(entity =>
        {
            entity.ToTable("Providers.Files").HasKey(f => f.Id);

            entity.Property(f => f.UploaderUid).IsRequired().HasMaxLength(13);
            entity.Property(f => f.UploaderUidType).IsRequired();
            entity.Property(f => f.UploaderName).IsRequired().HasMaxLength(DBConstraints.ProviderFile.UploaderNameMaxLength);
            entity.Property(f => f.FileName).IsRequired().HasMaxLength(DBConstraints.ProviderFile.FileName);
            entity.Property(f => f.FilePath).HasMaxLength(DBConstraints.ProviderFile.FilePath);
            entity.Property(f => f.UploadedOn).HasMaxLength(200);

            entity
               .HasOne(f => f.Provider)
               .WithMany(pr => pr.Files)
               .HasForeignKey(f => f.ProviderId)
               .HasPrincipalKey(f => f.Id);
        });
    }

    private void CreateProviderStatusHistory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderStatusHistory>(entity =>
        {
            entity.ToTable("Providers.StatusHistory").HasKey(f => f.Id);

            entity.Property(f => f.DateTime).IsRequired();
            entity.Property(f => f.ModifierUid).IsRequired().HasMaxLength(DBConstraints.UidMaxLength);
            entity.Property(f => f.ModifierUidType).IsRequired();
            entity.Property(f => f.ModifierFullName).IsRequired().HasMaxLength(DBConstraints.ProviderStatusHistory.ModifierNameMaxLength);
            entity.Property(f => f.Status).IsRequired();
            entity.Property(f => f.Comment).HasMaxLength(DBConstraints.ProviderStatusHistory.CommentMaxLength);

            entity
               .HasOne(f => f.Provider)
               .WithMany(pr => pr.StatusHistory)
               .HasForeignKey(f => f.ProviderId)
               .HasPrincipalKey(f => f.Id);
        });
    }

    private void CreateProviderOffices(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderOffice>(entity =>
        {
            entity.ToTable("Providers.Offices").HasKey(f => f.Id);

            entity.Property(f => f.Name).IsRequired().HasMaxLength(Constants.ProviderOffices.NameMaxLength);
            entity.Property(f => f.Address).IsRequired().HasMaxLength(Constants.ProviderOffices.AddressMaxLength);
            entity.Property(f => f.Lat).IsRequired();
            entity.Property(f => f.Lon).IsRequired();

            entity
               .HasOne(f => f.Provider)
               .WithMany(pr => pr.Offices)
               .HasForeignKey(f => f.ProviderId)
               .HasPrincipalKey(f => f.Id);
        });
    }

    private void CreateProvidersTimestamps(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderTimestamp>(entity =>
        {
            entity.ToTable("Providers.Timestamps").HasKey(f => f.ProviderId);

            entity.Property(f => f.Signature).IsRequired();
            entity.Property(f => f.DateTime).IsRequired();

            entity
               .HasOne(pt => pt.Provider)
               .WithOne(pr => pr.Timestamp)
               .HasForeignKey<ProviderTimestamp>(pt => pt.ProviderId)
               .IsRequired();
        });
    }

    private void CreateProvidersNumberRegisters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NumberRegister>(entity =>
        {
            entity.ToTable(NumberRegister.TableName).HasKey(f => f.Id);

            entity.Property(f => f.Id).IsRequired().HasMaxLength(10);
            entity.Property(f => f.Current).IsRequired();
            entity.Property(f => f.LastChange).IsRequired();

            entity.HasData(new NumberRegister
            {
                Id = NumberRegister.RegistrationNumberId,
                Current = 0,
                LastChange = DateOnly.FromDateTime(DateTime.Now)
            });
        });
    }

    private void CreateAdministratorActions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdministratorAction>(entity =>
        {
            entity.ToTable("Providers.Users.AdministratorActions")
                .HasKey(f => f.Id);

            entity.Property(f => f.DateTime).IsRequired();
            entity.Property(f => f.AdministratorUid).IsRequired().HasMaxLength(Constants.AdministratorAction.AdministratorUidMaxLength);
            entity.Property(f => f.AdministratorUidType).IsRequired();
            entity.Property(f => f.AdministratorFullName).IsRequired().HasMaxLength(Constants.AdministratorAction.AdministratorFullNameMaxLength);
            entity.Property(f => f.Action).IsRequired();
            entity.Property(f => f.Comment).IsRequired().HasMaxLength(Constants.AdministratorAction.CommentMaxLength);

            entity
               .HasOne(f => f.User)
               .WithMany(pr => pr.AdministratorActions)
               .HasForeignKey(f => f.UserId)
               .HasPrincipalKey(f => f.Id);
        });
    }

    private void CreateProviderDoneServices(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderDoneService>(entity =>
        {
            entity.ToTable("Providers.DoneServises")
                .HasKey(f => f.Id);

            entity.Property(f => f.ProviderId).IsRequired();
            entity.Property(f => f.ServiceId).IsRequired();
            entity.Property(f => f.Count).IsRequired();
            entity.Property(f => f.CreatedOn).IsRequired();

            entity
               .HasOne(f => f.Provider)
               .WithMany(pr => pr.DoneServices)
               .HasForeignKey(f => f.ProviderId);

            entity
               .HasOne(f => f.Service)
               .WithMany(pr => pr.DoneServices)
               .HasForeignKey(f => f.ServiceId);

            entity
                .HasIndex(f => new
                {
                    f.ProviderId,
                    f.CreatedOn,
                });
        });
    }
}

public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=eid-pdeau;Username=postgres;Password=eid$pass");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
