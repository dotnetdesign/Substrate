using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace DotNetDesign.Substrate.WebApi
{
    public interface IEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation, in TApiKey>
        : IEntityRepositoryController<TEntityData, TEntity, TEntityRepository, Guid, TEntityDataImplementation, TApiKey>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
    }

    public interface IEntityRepositoryController<TEntityData, TEntity, TEntityRepository, in TId, TEntityDataImplementation, in TApiKey>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
        /// <summary>
        /// Gets all entity data.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        IQueryable<TEntityDataImplementation> Get(TApiKey apiKey);

        /// <summary>
        /// Gets the entity data by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="apiKey">The API Key.</param>
        /// <returns></returns>
        TEntityDataImplementation Get(TId id, TApiKey apiKey);

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        TEntityDataImplementation Get(TId id, int version, TApiKey apiKey);

        /// <summary>
        /// Creates the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        HttpResponseMessage<TEntityDataImplementation> Post(TEntityDataImplementation entityData, TApiKey apiKey);

        /// <summary>
        /// Saves the specified entity data.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityData">The entity data.</param>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        HttpResponseMessage<TEntityDataImplementation> Put(TId id, TEntityDataImplementation entityData, TApiKey apiKey);

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="apiKey">The API key.</param>
        HttpResponseMessage Delete(TId id, TApiKey apiKey);
    }
}