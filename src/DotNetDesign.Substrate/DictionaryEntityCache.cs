using System;
using System.Collections.Generic;
using System.Linq;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Basic Entity Cache implementation using an internal dictionary scoped to this instance only.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public class DictionaryEntityCache<TEntity, TEntityData, TEntityRepository> : DictionaryEntityCache<TEntity, Guid, TEntityData, TEntityRepository>, IEntityCache<TEntity, TEntityData, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
    }

    /// <summary>
    /// Basic Entity Cache implementation using an internal dictionary scoped to this instance only.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public class DictionaryEntityCache<TEntity, TId, TEntityData, TEntityRepository> :
        IEntityCache<TEntity, TId, TEntityData, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        /// <summary>
        /// Dictionary for internal cache store.
        /// </summary>
        protected readonly IDictionary<string, IEnumerable<TEntityData>> DictionaryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEntityCache&lt;TEntity, TId, TEntityData, TEntityRepository&gt;"/> class.
        /// </summary>
        public DictionaryEntityCache()
        {
            using (Logger.Assembly.Scope())
            {
                DictionaryCache = new Dictionary<string, IEnumerable<TEntityData>>();
            }
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public IEnumerable<TEntityData> Get(string key)
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
        /// <param name="entityData">The entity data.</param>
        public void Add(string key, IEnumerable<TEntityData> entityData)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNullOrEmpty(key, "key");

                // ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(entityData, "entityData");
                var entityDataArray = entityData as TEntityData[] ?? entityData.ToArray();
                // ReSharper restore PossibleMultipleEnumeration

                Logger.Assembly.Debug(
                    m =>
                    m("Adding entities of type [{0}] to cache. Key [{1}]. Value(s) [{2}].", typeof (TEntityData), key,
                      string.Join<TEntityData>(",", entityDataArray)));

                if (DictionaryCache.ContainsKey(key))
                {
                    DictionaryCache[key] = entityDataArray;
                }
                else
                {
                    DictionaryCache.Add(key, entityDataArray);
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
        /// <param name="entityData">The entity data.</param>
        public void RemoveIfDataContains(TEntityData entityData)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNull(entityData, "entityData");

                RemoveIfDataContains(new[] {entityData});
            }
        }

        /// <summary>
        /// Removes if data contains.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        public void RemoveIfDataContains(IEnumerable<TEntityData> entityData)
        {
            using (Logger.Assembly.Scope())
            {
// ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(entityData, "entityData"); 
                var entityDataArray = entityData as TEntityData[] ?? entityData.ToArray();
// ReSharper restore PossibleMultipleEnumeration

                var entityDataString = string.Join<TEntityData>(",", entityDataArray);

                Logger.Assembly.Debug(m => m("Removing cached value if data contains entity data [{0}].", entityDataString));

                foreach (var cacheKeyValuePair in DictionaryCache.Where(cacheKeyValuePair => cacheKeyValuePair.Value.Any(entityDataArray.Contains)))
                {
                    var pair = cacheKeyValuePair;

                    Logger.Assembly.Debug(m => m("Removing cached value. Cached key [{0}]. Cached value [{1}]. Entity data [{2}].",
                                                 pair.Key,
                                                 string.Join(",", pair.Value),
                                                 entityDataString));

                    DictionaryCache.Remove(cacheKeyValuePair.Key);
                }
            }
        }
    }
}