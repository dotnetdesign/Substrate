using System;
using Common.Logging;

namespace DotNetDesign.EntityFramework
{
    public static class PermissionAuthrozationManagerExtensions
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(PermissionAuthrozationManagerExtensions));

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
            if (!permissionAuthorizationManager.IsAuthorized(requiredPermissions))
            {
                var unauthorizedException = new UnauthorizedAccessException(string.Format(
                    "User not authorized with required permissions [{0}] on entity of type {1}.", requiredPermissions, typeof(TEntity)));
                Logger.Error(unauthorizedException.Message);
                throw unauthorizedException;
            }
        }
    }
}