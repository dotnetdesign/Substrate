using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Represents an entity data.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public interface IEntityData<TEntityData, out TEntity, TEntityRepository> : IEntityData<TEntityData, TEntity, Guid, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, Guid, TEntityRepository>
        where TEntity : class, IEntity<TEntity, Guid, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, Guid, TEntityData>
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
    }
}
