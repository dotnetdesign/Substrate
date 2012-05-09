using System;

namespace DotNetDesign.Substrate
{
    ///<summary>
    /// Basic implementation of IPermissionAuthorizationManager that always returns true for IsAuthorized.
    ///</summary>
    public class AnonymousPermissionAuthorizationManager<TEntity, TEntityData, TEntityRepository, TApiKey> :
        AnonymousPermissionAuthorizationManager<TEntity, TEntityData, Guid, TEntityRepository, TApiKey>, IApiKeyPermissionAuthorizationManager<TEntity, TEntityData, TEntityRepository, TApiKey>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
    }

    ///<summary>
    /// Basic implementation of IPermissionAuthorizationManager that always returns true for IsAuthorized.
    ///</summary>
    public class AnonymousPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository, TApiKey> :
        BaseLogger<AnonymousPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository, TApiKey>>, IApiKeyPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository, TApiKey>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        /// <summary>
        /// Determines whether the authenticated user is authorized with the specified required permissions.
        /// </summary>
        /// <param name="requiredPermissions">The required permissions.</param>
        /// <returns>
        ///   <c>true</c> if the authenticated user is authorized with the specified required permissions; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAuthorized(EntityPermissions requiredPermissions)
        {
            using (Logger.Scope())
            {
                Logger.InfoFormat("Returning true for IsAuthrozied call for required permissions {0}", requiredPermissions);
                return true;
            }
        }

        ///<summary>
        /// API Key that is used by the authorization manager if provided.
        ///</summary>
        public TApiKey ApiKey { get; set; }
    }
}