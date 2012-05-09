using System.Collections.Generic;
using System.Linq;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Common extension methods
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Removes the nulls.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T> values) where T : class
        {
            return values.Where(x => x != null);
        }
    }
}