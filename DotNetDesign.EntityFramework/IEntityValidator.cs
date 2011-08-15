using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines functionality required to validate an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public interface IEntityValidator<in TEntity, TEntityData, TEntityRepository> : IEntityValidator<TEntity, TEntityData, Guid, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
    }

    /// <summary>
    /// Defines functionality required to validate an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public interface IEntityValidator<in TEntity, TEntityData, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        /// <summary>
        /// Validates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        IEnumerable<IValidationResult> Validate(TEntity entity);
    }
}