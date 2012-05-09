using System;

namespace DotNetDesign.Substrate
{
    public interface IApiKeyPermissionAuthorizationManager<in TEntity, TEntityData, TEntityRepository, TApiKey> :
        IApiKeyPermissionAuthorizationManager<TEntity, TEntityData, Guid, TEntityRepository, TApiKey>, IPermissionAuthorizationManager<TEntity, TEntityData, TEntityRepository> 
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {

    }

    public interface IApiKeyPermissionAuthorizationManager<in TEntity, TEntityData, TId, TEntityRepository, TApiKey> : 
        IPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository> 
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData 
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository> 
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        ///<summary>
        /// API Key that is used by the authorization manager if provided.
        ///</summary>
        TApiKey ApiKey { get; set; }
    }
}