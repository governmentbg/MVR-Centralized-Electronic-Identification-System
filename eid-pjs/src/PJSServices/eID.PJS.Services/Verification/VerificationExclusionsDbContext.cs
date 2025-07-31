using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace eID.PJS.Services.Verification
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class VerificationExclusionsDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the verification exclusions.
        /// </summary>
        /// <value>
        /// The verification exclusions.
        /// </value>
        public DbSet<VerificationExclusion> VerificationExclusions { get; set; }
        public DbSet<PathCheckResult> PathCheckResults { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationExclusionsDbContext"/> class.
        /// </summary>
        /// <remarks>
        /// See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see>
        /// for more information and examples.
        /// </remarks>
        public VerificationExclusionsDbContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationExclusionsDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public VerificationExclusionsDbContext(DbContextOptions<VerificationExclusionsDbContext> options): base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            //configurationBuilder.DefaultTypeMapping<PathCheckResult>();
        }

        /// <summary>
        /// Override this method to configure the database (and other options) to be used for this context.
        /// This method is called for each instance of the context that is created.
        /// The base implementation does nothing.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context. Databases (and other extensions)
        /// typically define extension methods on this object that allow you to configure the context.</param>
        /// <remarks>
        /// <para>
        /// In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have been passed
        /// to the constructor, you can use <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" /> to determine if
        /// the options have already been set, and skip some or all of the logic in
        /// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see>
        /// for more information and examples.
        /// </para>
        /// </remarks>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            // Configure your database connection here
            // Example: optionsBuilder.UseSqlServer(@"Your_Connection_String");
        }

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>
        /// <para>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run. However, it will still run when creating a compiled model.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
        /// examples.
        /// </para>
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            CreateExclusions(modelBuilder);
        }

        private static void CreateExclusions(ModelBuilder modelBuilder)
        {

            // Configure base type
            modelBuilder.Entity<VerificationExclusion>()
                .ToTable("Exclusions") 
                .HasKey(v => v.Id); // Primary key

            // Configure discriminator
            modelBuilder.Entity<VerificationExclusion>()
                .HasDiscriminator<string>("ExclusionType")
                .HasValue<VerificationExclusion>("Base") // Base class discriminator value
                .HasValue<FileORFolderExclusion>("FileFolder") 
                .HasValue<DateRangeExclusion>("DateRange");

            modelBuilder.Entity<VerificationExclusion>()
                .Property(f => f.ReasonForExclusion).IsRequired().HasMaxLength(256);

            // Configure properties of derived types
            modelBuilder.Entity<FileORFolderExclusion>()
                .Property(f => f.ExcludedPath).IsRequired();

            modelBuilder.Entity<DateRangeExclusion>()
                .Property(d => d.StartDate)
                .IsRequired();

            modelBuilder.Entity<DateRangeExclusion>()
                .Property(d => d.EndDate)
                .IsRequired();

        }

        /// <summary>
        /// Seeds the database.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void Seed(ILogger logger)
        {

            Database.EnsureDeleted();
            Database.EnsureCreated();

            
            VerificationExclusions.Add(new FileORFolderExclusion { 
                Id = Guid.NewGuid(),
                CreatedBy = "seed",
                DateCreated = DateTime.UtcNow,
                ExcludedPath = "audit\\test"
            });

            VerificationExclusions.Add(new DateRangeExclusion
            {
                Id = Guid.NewGuid(),
                CreatedBy = "seed",
                DateCreated = DateTime.UtcNow,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            });

            try
            {
                SaveChanges();
                logger.LogInformation("Database seeded");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding database");
            }
        }
    }
}
