using System;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Represents an entity data.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public interface IEntityData<TEntityData, out TEntity, TEntityRepository> : IEntityData<TEntityData, TEntity, EntityIdentifier, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
    }

    /// <summary>
    /// Represents an entity data.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public interface IEntityData<TEntityData, out TEntity, TId, TEntityRepository> : IVersionable, IIdentifiable<TId>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        /// <summary>
        /// Gets the version id.
        /// </summary>
        string VersionId { get; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last updated at.
        /// </summary>
        /// <value>
        /// The last updated at.
        /// </value>
        DateTime LastUpdatedAt { get; set;  }
    }
}
