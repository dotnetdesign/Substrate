using System;
using System.Collections.Generic;
using System.Linq;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Extension methods for <see>
    ///                           <cref>IEntityData{TEntityData,TEntity,TEntityRepository}</cref>
    ///                           <cref>IEntityData{TEntityData,TEntity, TId,TEntityRepository}</cref>
    ///                       </see>
    /// </summary>
    public static class EntityDataExtensions
    {
        /// <summary>
        /// Gets the clones.
        /// </summary>
        /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
        /// <param name="entityDataImplementations">The entity data implementations.</param>
        /// <returns></returns>
        public static IEnumerable<TEntityData> GetClones
            <TEntityData, TEntity, TEntityRepository>(
            this IEnumerable<TEntityData> entityDataImplementations)
            where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
            where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
            where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
        {
            using (Logger.Assembly.Scope())
            {
                return entityDataImplementations.GetClones<TEntityData, TEntity, Guid, TEntityRepository>();
            }
        }

        /// <summary>
        /// Gets the clones.
        /// </summary>
        /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TId">The type of the id.</typeparam>
        /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
        /// <param name="entityDataImplementations">The entity data implementations.</param>
        /// <returns></returns>
        public static IEnumerable<TEntityData> GetClones
            <TEntityData, TEntity, TId, TEntityRepository>(
            this IEnumerable<TEntityData> entityDataImplementations)
            where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
            where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
            where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        {
            using (Logger.Assembly.Scope())
            {
                // ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(entityDataImplementations, "entityDataImplementations");

                return entityDataImplementations.Select(x => x.Clone());
                // ReSharper restore PossibleMultipleEnumeration
            }
        }
    }
}