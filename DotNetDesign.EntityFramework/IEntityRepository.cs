using System;
using System.Collections.Generic;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines methods for storing and retrieving entities.
    /// </summary>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    public interface IEntityRepository<TEntityRepository, TEntity, TEntityData> : IEntityRepository<TEntityRepository, TEntity, Guid, TEntityData>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
    }

    /// <summary>
    /// Defines methods for storing and retrieving entities.
    /// </summary>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    public interface IEntityRepository<TEntityRepository, TEntity, in TId, TEntityData>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        /// <summary>
        /// Gets a new entity.
        /// </summary>
        /// <returns></returns>
        TEntity GetNew();

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Gets the entity by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        TEntity GetById(TId id);

        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        IEnumerable<TEntity> GetByIds(IEnumerable<TId> ids);

        /// <summary>
        /// Gets the specified version of the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        TEntity GetVersion(TEntity entity, int version);

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        TEntity GetPreviousVersion(TEntity entity);

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        TEntity Save(TEntity entity);

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        IEnumerable<TEntity> SaveAll(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void DeleteAll(IEnumerable<TEntity> entities);
    }
}