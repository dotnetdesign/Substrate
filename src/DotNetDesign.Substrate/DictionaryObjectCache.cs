using System.Collections.Generic;
using System.Linq;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Basic Entity Cache implementation using an internal dictionary scoped to this instance only.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    public class DictionaryObjectCache<TObject> :
        IObjectCache<TObject>
        where TObject : class
    {
        /// <summary>
        /// Dictionary for internal cache store.
        /// </summary>
        protected readonly IDictionary<string, IEnumerable<TObject>> DictionaryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryObjectCache&lt;TObject&gt;"/> class.
        /// </summary>
        public DictionaryObjectCache()
        {
            using (Logger.Assembly.Scope())
            {
                DictionaryCache = new Dictionary<string, IEnumerable<TObject>>();
            }
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public IEnumerable<TObject> Get(string key)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNullOrEmpty(key, "key");

                Logger.Assembly.Debug(m => m("Getting cached value for key [{0}].", key));
                return DictionaryCache.ContainsKey(key) ? DictionaryCache[key] : null;
            }
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="objectData">The object data.</param>
        public void Add(string key, IEnumerable<TObject> objectData)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNullOrEmpty(key, "key");
// ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(objectData, "objectData");
                var objectDataArray = objectData as TObject[] ?? objectData.ToArray();
// ReSharper restore PossibleMultipleEnumeration

                Logger.Assembly.Debug(m => m("Adding entities of type [{0}] to cache. Key [{1}]. Value(s) [{2}].", typeof(TObject), key, string.Join<TObject>(",", objectDataArray)));

                if (DictionaryCache.ContainsKey(key))
                {
                    DictionaryCache[key] = objectDataArray;
                }
                else
                {
                    DictionaryCache.Add(key, objectDataArray);
                }
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNullOrEmpty(key, "key");

                Logger.Assembly.Debug(m => m("Removing cached value for key [{0}].", key));
                DictionaryCache.Remove(key);
            }
        }

        /// <summary>
        /// Removes if data contains.
        /// </summary>
        /// <param name="objectData">The object data.</param>
        public void RemoveIfDataContains(TObject objectData)
        {
            using (Logger.Assembly.Scope())
            {
                if (objectData != null)
                {
                    RemoveIfDataContains(new[] { objectData });
                }
            }
        }

        /// <summary>
        /// Removes if data contains.
        /// </summary>
        /// <param name="objectData">The object data.</param>
        public void RemoveIfDataContains(IEnumerable<TObject> objectData)
        {
            using (Logger.Assembly.Scope())
            {
// ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(objectData, "objectData");
                var objectDataArray = objectData as TObject[] ?? objectData.ToArray();
// ReSharper restore PossibleMultipleEnumeration

                var objectDataString = string.Join<TObject>(",", objectDataArray);

                Logger.Assembly.Debug(m => m("Removing cached value if data contains object data [{0}].", objectDataString));
                foreach (var cacheKeyValuePair in DictionaryCache)
                {
                    if (!cacheKeyValuePair.Value.Any(objectDataArray.Contains)) continue;

                    var pair = cacheKeyValuePair;
                    Logger.Assembly.Debug(m => m("Removing cached value. Cached key [{0}]. Cached value [{1}]. Object data [{2}].", 
                                                 pair.Key, 
                                                 string.Join(",", pair.Value),
                                                 objectDataString));

                    DictionaryCache.Remove(cacheKeyValuePair.Key);
                }
            }
        }
    }
}