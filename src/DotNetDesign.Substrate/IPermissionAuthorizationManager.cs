using System;

namespace DotNetDesign.Substrate
{
    ///<summary>
    /// Defines methods and properties for IPermissionAuthorizationManagers
    ///</summary>
    public interface IPermissionAuthorizationManager<in TEntity, TEntityData, TEntityRepository> : IPermissionAuthorizationManager<TEntity, TEntityData, Guid, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
    }

    ///<summary>
    /// Defines methods and properties for IPermissionAuthorizationManagers
    ///</summary>
    public interface IPermissionAuthorizationManager<in TEntity, TEntityData, TId, TEntityRepository>
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
        bool IsAuthorized(EntityPermissions requiredPermissions);
    }
}