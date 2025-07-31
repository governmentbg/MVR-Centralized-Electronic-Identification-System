using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eID.PJS.Services.Verification;
using FluentValidation.Results;

namespace eID.PJS.Services.Entities
{
    /// <summary>
    /// Interface for the verification exceptions provider 
    /// which will manage the exclusions of log files from the verification process.
    /// </summary>
    public interface IVerificationExclusionProvider: IDisposable
    {
        /// <summary>
        /// Adds the specified exclusion to the list of exclusions.
        /// </summary>
        /// <param name="data">The data.</param>
        ValidationResult? Add(VerificationExclusion data);
        
        Task<ValidationResult?> AddAsync(VerificationExclusion data);

        /// <summary>
        /// Removes the specified exclusion by its ID.
        /// </summary>
        /// <param name="exclusionId">The exclusion identifier.</param>
        /// <returns></returns>
        int Remove(Guid exclusionId);

        Task<int> RemoveAsync(Guid exclusionId);


        /// <summary>
        /// Removes all records in the storage.
        /// </summary>
        int RemoveAll();
        Task<int> RemoveAllAsync();


        /// <summary>
        /// Gets the specified exclusion given its identifier.
        /// </summary>
        /// <param name="exclusionId">The exclusion identifier.</param>
        /// <returns></returns>
        VerificationExclusion? Get(Guid exclusionId);
        Task<VerificationExclusion?> GetAsync(Guid exclusionId);


        /// <summary>
        /// Gets list of all exclusions.
        /// </summary>
        /// <returns></returns>
        IEnumerable<VerificationExclusion>? GetAll();

        Task<IEnumerable<VerificationExclusion>?> GetAllAsync();

        /// <summary>
        /// Gets teh total number of exclusions in the storage.
        /// </summary>
        /// <returns></returns>
        int Count();
    }
}
