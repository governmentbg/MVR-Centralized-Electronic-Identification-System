using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.Services.Entities;

using Microsoft.EntityFrameworkCore;

#nullable disable

namespace eID.PJS.Services.Verification
{
    public class Exclusions : IReadOnlyList<VerificationExclusion>, IDisposable
    {
        private IVerificationExclusionProvider _provider;
        private readonly List<VerificationExclusion> _exclusions;
        public Exclusions(IVerificationExclusionProvider provider)
        {
            _provider = provider ?? throw new ArgumentException(nameof(provider));

            _exclusions = new List<VerificationExclusion>();
        }

        public Exclusions()
        {
            _exclusions = new List<VerificationExclusion>();
        }

        public Exclusions(IEnumerable<VerificationExclusion> items)
        {
            if (items == null || !items.Any())
            {
                _exclusions = new List<VerificationExclusion>();
            }
            else
            {
                _exclusions = new List<VerificationExclusion>(items);
            }
        }


        public void Reload()
        {
            _exclusions.Clear();

            _exclusions.AddRange(_provider.GetAll().ToList());
        }
        public bool IsExcluded(DateOnly date)
        {

            foreach (var item in _exclusions)
            {
                var w = item as DateRangeExclusion;
                if (w != null)
                {
                    if(w.StartDate <= date && w.EndDate >= date)
                        return true;
                }
            }

            return false;
        }

        public bool IsExcluded(DateOnly startDate, DateOnly endDate)
        {

            foreach (var item in _exclusions)
            {
                var w = item as DateRangeExclusion;
                if (w != null)
                {
                    if (w.StartDate <= startDate && w.EndDate >= endDate)
                        return true;
                }
            }

            return false;
        }

        public bool IsExcluded(string path)
        {

            foreach (var item in _exclusions)
            {
                var w = item as FileORFolderExclusion;
                if (w != null)
                {
                    if(IsSubPathOrEqual(w.ExcludedPath, path))
                        return true;
                }
            }

            return false;
        }

        public VerificationExclusion Get(Guid id)
        { 
            return _exclusions.FirstOrDefault(f => f.Id == id);
        }

        public VerificationExclusion this[int index]
        {
            get
            {
                if (_exclusions == null)
                    return null;

                return _exclusions[index];
            }
        }
        public int Count
        {
            get
            {
                if (_exclusions == null)
                    return 0;

                return _exclusions.Count;
            }
        }

        public IEnumerator<VerificationExclusion> GetEnumerator()
        {
            if (_exclusions == null)
                return null;

            return _exclusions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_exclusions == null)
                return null;

            return _exclusions.GetEnumerator();
        }

        private static bool IsSubPathOrEqual(string basePath, string path)
        {
            // Normalize the paths to a standard format
            var normalizedBasePath = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var normalizedPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Check if paths are identical
            if (String.Equals(normalizedBasePath, normalizedPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Ensure the base path ends with a directory separator before the comparison
            normalizedBasePath += Path.DirectorySeparatorChar;

            // Use a case-insensitive comparison for Windows paths
            return normalizedPath.StartsWith(normalizedBasePath, StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            if (_provider != null)
                _provider.Dispose();    
        }
    }
}
