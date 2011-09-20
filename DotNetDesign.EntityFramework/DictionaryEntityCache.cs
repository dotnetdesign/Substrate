using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Basic Entity Cache implementation using an internal dictionary scoped to this instance only.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public class DictionaryEntityCache<TEntity, TEntityData, TEntityRepository> : DictionaryEntityCache<TEntity, EntityIdentifier, TEntityData, TEntityRepository>, IEntityCache<TEntity, TEntityData, TEntityRepository>
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
    public class DictionaryEntityCache<TEntity, TId, TEntityData, TEntityRepository> : IEntityCache<TEntity, TId, TEntityData, TEntityRepository>
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
            DictionaryCache = new Dictionary<string, IEnumerable<TEntityData>>();
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public IEnumerable<TEntityData> Get(string key)
        {
            return DictionaryCache.ContainsKey(key) ? DictionaryCache[key] : null;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="entityData">The entity data.</param>
        public void Add(string key, IEnumerable<TEntityData> entityData)
        {
            if (DictionaryCache.ContainsKey(key))
            {
                DictionaryCache[key] = entityData;
            }
            else
            {
                DictionaryCache.Add(key, entityData);
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            DictionaryCache.Remove(key);
        }

        /// <summary>
        /// Removes if data contains.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        public void RemoveIfDataContains(TEntityData entityData)
        {
            if (entityData != null)
            {
                RemoveIfDataContains(new[] {entityData});
            }
        }

        /// <summary>
        /// Removes if data contains.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        public void RemoveIfDataContains(IEnumerable<TEntityData> entityData)
        {
            foreach (var cacheKeyValuePair in DictionaryCache)
            {
                if (cacheKeyValuePair.Value.Any(x => entityData.Contains(x)))
                {
                    DictionaryCache.Remove(cacheKeyValuePair.Key);
                }
            }
        }
    }
}