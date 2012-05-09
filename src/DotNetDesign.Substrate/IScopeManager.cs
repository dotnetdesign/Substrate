using System.Collections.Generic;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Manages changes to scope values.
    /// </summary>
    public interface IScopeManager
    {
        /// <summary>
        /// Adds the specified key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Add(string key, string value);

        /// <summary>
        /// Removes the specified key and associated value.
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);

        /// <summary>
        /// Gets the scope context.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetScopeContext();
    }
}