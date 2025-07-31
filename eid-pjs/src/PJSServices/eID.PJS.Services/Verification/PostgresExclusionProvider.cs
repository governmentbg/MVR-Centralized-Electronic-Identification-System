using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using eID.PJS.Services.Entities;

using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

using OpenSearch.Client;

namespace eID.PJS.Services.Verification
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="eID.PJS.Services.Entities.IVerificationExclusionProvider" />
    public class PostgresExclusionProvider : IVerificationExclusionProvider
    {

        private VerificationExclusionsDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresExclusionProvider"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public PostgresExclusionProvider(IDbContextFactory<VerificationExclusionsDbContext> factory)
        {
            _dbContext = factory.CreateDbContext();
        }

        /// <summary>
        /// Adds the specified exclusion to the list of exclusions.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public ValidationResult? Add(VerificationExclusion data)
        {
            if (data.Id == Guid.Empty)
                data.Id = Guid.NewGuid();

            if (data.IsValid())
            {

                data.DateCreated = DateTime.UtcNow;
                _dbContext.VerificationExclusions.Add(data);
                _dbContext.SaveChanges();

                return null;
            }
            else
            {
                return data.GetValidationResult();
            }
        }

        public async Task<ValidationResult?> AddAsync(VerificationExclusion data)
        {
            if (data.Id == Guid.Empty)
                data.Id = Guid.NewGuid();

            if (data.IsValid())
            {
                data.DateCreated = DateTime.UtcNow;
                await _dbContext.VerificationExclusions.AddAsync(data);
                await _dbContext.SaveChangesAsync();

                return null;
            }
            else
            {
                return data.GetValidationResult();
            };
        }

        /// <summary>
        /// Gets teh total number of exclusions in the storage.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _dbContext.VerificationExclusions.Count();
        }

        public void Dispose()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }

        /// <summary>
        /// Gets the specified exclusion given its identifier.
        /// </summary>
        /// <param name="exclusionId">The exclusion identifier.</param>
        /// <returns></returns>
        public VerificationExclusion? Get(Guid exclusionId)
        {
            return _dbContext.VerificationExclusions.Where(w => w.Id == exclusionId).FirstOrDefault();
        }

        /// <summary>
        /// Gets list of all exclusions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VerificationExclusion>? GetAll()
        {
            return _dbContext.VerificationExclusions.ToList();
        }

        public async Task<IEnumerable<VerificationExclusion>?> GetAllAsync()
        {
            return await _dbContext.VerificationExclusions.ToListAsync();

        }

        public async Task<VerificationExclusion?> GetAsync(Guid exclusionId)
        {
            return await _dbContext.VerificationExclusions.Where(w => w.Id == exclusionId).FirstOrDefaultAsync();

        }

        /// <summary>
        /// Removes the specified exclusion by its ID.
        /// </summary>
        /// <param name="exclusionId">The exclusion identifier.</param>
        /// <returns></returns>
        public int Remove(Guid exclusionId)
        {
            _dbContext.VerificationExclusions.Remove(new FileORFolderExclusion { Id = exclusionId });
            return _dbContext.SaveChanges();
        }

        /// <summary>
        /// Removes all records in the storage.
        /// </summary>
        public int RemoveAll()
        {
            return _dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE public.\"Exclusions\"");
        }

        public async Task<int> RemoveAllAsync()
        {
            return await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE public.\"Exclusions\"");
        }

        public async Task<int> RemoveAsync(Guid exclusionId)
        {
            _dbContext.VerificationExclusions.Remove(new FileORFolderExclusion { Id = exclusionId });
            return await _dbContext.SaveChangesAsync();
        }
    }

    [Keyless]
    public class PathCheckResult
    {
        public bool IsExcluded { get; set; }
    }
}
