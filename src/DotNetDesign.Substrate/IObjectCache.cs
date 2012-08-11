using System.Collections.Generic;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Defines methods for an entity cache.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public interface IObjectCache<TObject>
    {
        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        IEnumerable<TObject> Get(string key);

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="objects">The object.</param>
        void Add(string key, IEnumerable<TObject> objects);

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);

        /// <summary>
        /// Removes from the cache if it contains an entity with the same data.
        /// </summary>
        /// <param name="objectData">The object data.</param>
        void RemoveIfDataContains(TObject objectData);

        /// <summary>
        /// Removes from the cache if it contains an entity with the same data.
        /// </summary>
        /// <param name="objectDatas">The object datas.</param>
        void RemoveIfDataContains(IEnumerable<TObject> objectDatas);
    }
}