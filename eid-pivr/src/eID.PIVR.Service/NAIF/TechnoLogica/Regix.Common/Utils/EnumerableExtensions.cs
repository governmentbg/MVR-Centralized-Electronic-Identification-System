using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace TechnoLogica.RegiX.Common.Utils
{
    /// <summary>
    /// IEnumerable exnetion methods
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// MethodInfo for the actual convertion method
        /// </summary>
        private static readonly MethodInfo ToStrongListMethod = typeof(EnumerableExtensions)
          .GetMethod("ToStrongListImpl", BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// Converts IEnumerable to IList&lt;T&gt;
        /// </summary>
        /// <param name="source">Source enumeration</param>
        /// <param name="targetType">Target type</param>
        /// <returns>Constructed IList&lt;T&gt;</returns>
        public static IList ToStrongList(this IEnumerable source, Type targetType)
        {
            var method = ToStrongListMethod.MakeGenericMethod(targetType);
            return (IList)method.Invoke(null, new object[] { source });
        }

        /// <summary>
        /// The actual logic for converting the enumeration
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="source">Source enumeration</param>
        /// <returns>Constrcuted IList&lt;T&gt;</returns>
        private static List<T> ToStrongListImpl<T>(this IEnumerable source)
        {
            return source.Cast<T>().ToList();
        }
    }
}
