using System;
using System.Collections.Generic;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Dictionary based scope manager.
    /// </summary>
    public class DictionaryScopeManager : IScopeManager
    {
        /// <summary>
        /// Backing dictionary for DictionaryScopeManager.
        /// </summary>
        protected Dictionary<string, string> ScopeManagerDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Adds the specified key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, string value)
        {
            using(Logger.Assembly.Scope())
            {
                if(ScopeManagerDictionary.ContainsKey(key))
                {
                    ScopeManagerDictionary[key] = value;
                }
                else
                {
                    ScopeManagerDictionary.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Removes the specified key and associated value.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            using (Logger.Assembly.Scope())
            {
                if (ScopeManagerDictionary.ContainsKey(key))
                {
                    ScopeManagerDictionary.Remove(key);
                }
            }
        }

        /// <summary>
        /// Gets the scope context.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetScopeContext()
        {
            using (Logger.Assembly.Scope())
            {
                return new Dictionary<string, string>(ScopeManagerDictionary);
            }
        }
    }
}