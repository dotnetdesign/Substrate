using System;
using Common.Logging;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Extensions for IPermissionAuthorizationManagers.
    /// </summary>
    public static class PermissionAuthrozationManagerExtensions
    {
        /// <summary>
        /// Authorizes the authenticated user is authorized with the specified required permissions.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
        /// <typeparam name="TId">The type of the id.</typeparam>
        /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
        /// <param name="permissionAuthorizationManager">The permission authorization manager.</param>
        /// <param name="requiredPermissions">The required permissions.</param>
        public static void Authorize<TEntity, TEntityData, TId, TEntityRepository>(this IPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository> permissionAuthorizationManager, EntityPermissions requiredPermissions)
            where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
            where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
            where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        {
            using (Logger.Assembly.Scope())
            {
                if (!permissionAuthorizationManager.IsAuthorized(requiredPermissions))
                {
                    var unauthorizedException = new UnauthorizedAccessException(string.Format(
                        "User not authorized with required permissions [{0}] on entity of type {1}.", requiredPermissions, typeof(TEntity)));
                    Logger.Assembly.Error(unauthorizedException.Message, unauthorizedException);
                    throw unauthorizedException;
                }
            }
        }

        /// <summary>
        /// Authorizes the authenticated user is authorized with the specified required permissions.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
        /// <typeparam name="TId">The type of the id.</typeparam>
        /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
        /// <param name="permissionAuthorizationManager">The permission authorization manager.</param>
        /// <param name="requiredPermissions">The required permissions.</param>
        /// <param name="entity">The entity to be authorized.</param>
        public static void Authorize<TEntity, TEntityData, TId, TEntityRepository>(this IPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository> permissionAuthorizationManager, EntityPermissions requiredPermissions, TEntity entity)
            where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
            where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
            where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        {
            using(Logger.Assembly.Scope())
            {
                if (!permissionAuthorizationManager.IsAuthorized(requiredPermissions, entity))
                {
                    var unauthorizedException = new UnauthorizedAccessException(string.Format(
                        "User not authorized with required permissions [{0}] on entity {1}.", requiredPermissions, entity));
                    Logger.Assembly.Error(unauthorizedException.Message, unauthorizedException);
                    throw unauthorizedException;
                }
            }
        }
    }
}