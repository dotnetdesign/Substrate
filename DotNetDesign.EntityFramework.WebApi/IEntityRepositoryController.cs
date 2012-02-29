using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace DotNetDesign.EntityFramework.WebApi
{
    public interface IEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation> 
        : IEntityRepositoryController<TEntityData,TEntity, TEntityRepository,Guid,TEntityDataImplementation>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
    }

    public interface IEntityRepositoryController<TEntityData, TEntity, TEntityRepository, in TId, TEntityDataImplementation>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
        /// <summary>
        /// Gets all entity data.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntityDataImplementation> Get();

        /// <summary>
        /// Gets the entity data by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        TEntityDataImplementation Get(TId id);

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        TEntityDataImplementation Get(TId id, int version);

        /// <summary>
        /// Creates the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        HttpResponseMessage<WebHttpEntityContainer<TEntityDataImplementation>> Post(TEntityDataImplementation entityData);

        /// <summary>
        /// Saves the specified entity data.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        HttpResponseMessage<WebHttpEntityContainer<TEntityDataImplementation>> Put(TId id, TEntityDataImplementation entityData);

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        HttpResponseMessage Delete(TId id);
    }
}