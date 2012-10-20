using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Common.Logging;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate.WebApi
{
    public class BaseEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation> :
        BaseEntityRepositoryController<TEntityData, TEntity, TEntityRepository, Guid, TEntityDataImplementation>, 
        IEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
        public BaseEntityRepositoryController(
            Func<IPermissionAuthorizationManager<TEntity, TEntityData, TEntityRepository>> permissionAuthorizationManagerFactory, 
            Func<TEntityRepository> entityRepositoryFactory, 
            Func<TEntity> entityFactory,
            string rootUri,
            params string[] excludedPropertyNames)
            : base(permissionAuthorizationManagerFactory, entityRepositoryFactory, entityFactory, rootUri, excludedPropertyNames)
        {
        }
    }

    public class BaseEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation> :
        ApiController, IEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
        /// <summary>
        /// Property names to excluded when comparing changes by default.
        /// </summary>
        public static readonly IEnumerable<string> DefaultExcludedPropertyNames = new[] { "Id", "CreatedAt", "UpdatedAt", "Version", "VersionId" };
        private readonly IEnumerable<string> _excludedPropertyNames;

        protected readonly Func<IPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository>> PermissionAuthorizationManagerFactory;
        protected readonly Func<TEntityRepository> EntityRepositoryFactory;
        protected readonly Func<TEntity> EntityFactory;
        private readonly string _rootUri;

        public BaseEntityRepositoryController(
            Func<IPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository>> permissionAuthorizationManagerFactory,
            Func<TEntityRepository> entityRepositoryFactory,
            Func<TEntity> entityFactory,
            string rootUri,
            params string[] excludedPropertyNames)
        {
            using (Logger.Assembly.Scope())
            {
                PermissionAuthorizationManagerFactory = permissionAuthorizationManagerFactory;
                EntityRepositoryFactory = entityRepositoryFactory;
                EntityFactory = entityFactory;
                _rootUri = rootUri;

                var excludedPropertyNamesList = new List<string>(DefaultExcludedPropertyNames);
                if (excludedPropertyNames != null)
                {
                    excludedPropertyNamesList.AddRange(excludedPropertyNames);
                }
                _excludedPropertyNames = excludedPropertyNamesList;
            }
        }

        /// <summary>
        /// Gets all entity data.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntityDataImplementation> Get()
        {
            using(Logger.Assembly.Scope())
            {
                Logger.Assembly.Debug(m => m("Getting all entities of type {0}", typeof(TEntity)));

                try
                {
                    Authorize(EntityPermissions.Read);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Assembly.Error(m => m("Unauthorized. {0}", ex.Message), ex);
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                }

                var entities = EntityRepositoryFactory().GetAll().Select(x => x.EntityData).Cast<TEntityDataImplementation>();

                return entities.AsQueryable();
            }
        }

        /// <summary>
        /// Gets the entity data by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public virtual TEntityDataImplementation Get(TId id)
        {
            using (Logger.Assembly.Scope())
            {
                Logger.Assembly.Debug(m => m("Getting entity of type {0} by id {1}", typeof(TEntity), id));

                try
                {
                    Authorize(EntityPermissions.Read);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Assembly.Error(m => m("Unauthorized. {0}", ex.Message), ex);
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                }

                var entity = EntityRepositoryFactory().GetById(id);

                return (entity == null)
                           ? null
                           : (entity.EntityData as TEntityDataImplementation);
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public virtual TEntityDataImplementation Get(TId id, int version)
        {
            using (Logger.Assembly.Scope())
            {
                Logger.Assembly.Debug(m => m("Getting entity of type {0} by id {1} and version {2}", typeof(TEntity), id, version));

                try
                {
                    Authorize(EntityPermissions.Read);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Assembly.Error(m => m("Unauthorized. {0}", ex.Message), ex);
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                }

                var entity = EntityRepositoryFactory().GetVersion(id, version);

                return (entity == null)
                           ? null
                           : (entity.EntityData as TEntityDataImplementation);
            }
        }

        /// <summary>
        /// Creates the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        public virtual HttpResponseMessage Post(TEntityDataImplementation entityData)
        {
            using (Logger.Assembly.Scope())
            {
                Logger.Assembly.Debug(m => m("Adding the entity {0}", entityData));

                if (entityData == null)
                {
                    Logger.Assembly.Info("No data posted.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                try
                {
                    Authorize(EntityPermissions.Insert);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Assembly.Error(m => m("Unauthorized. {0}", ex.Message), ex);
                    var unauthorizedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    unauthorizedResponse.Headers.Add(SuppressFormsAuthenticationRedirectModule.SuppressFormsHeaderName, "true");
                    throw new HttpResponseException(unauthorizedResponse);
                }

                var existingEntity = EntityRepositoryFactory().GetNew();

                ApplyChanges(entityData, existingEntity);

                TEntity savedEntity;
                if (!existingEntity.Save(out savedEntity))
                {
                    Logger.Assembly.Info("Validation failed.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, existingEntity);
                }

                Logger.Assembly.Info("Save succeeded.");
                var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity.EntityData);
                response.Headers.Location = new Uri(Request.RequestUri, _rootUri + "/" + savedEntity.Id);

                return response;
            }
        }

        /// <summary>
        /// Saves the specified entity data.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        public virtual HttpResponseMessage Put(TId id, TEntityDataImplementation entityData)
        {
            using (Logger.Assembly.Scope())
            {
                Logger.Assembly.Debug(m => m("Saving the entity {0}", entityData));

                if (entityData == null)
                {
                    Logger.Assembly.Info("No data posted.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                try
                {
                    Authorize(EntityPermissions.Update);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Assembly.Error(m => m("Unauthorized. {0}", ex.Message), ex);
                    var unauthorizedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    unauthorizedResponse.Headers.Add(SuppressFormsAuthenticationRedirectModule.SuppressFormsHeaderName, "true");
                    throw new HttpResponseException(unauthorizedResponse);
                }

                var existingEntity = EntityRepositoryFactory().GetById(id);

                if (existingEntity == null)
                {
                    Logger.Assembly.Info("Existing entity not found.");
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                ApplyChanges(entityData, existingEntity);

                TEntity savedEntity;
                if (!existingEntity.Save(out savedEntity))
                {
                    Logger.Assembly.Info("Validation failed.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, existingEntity);
                }

                Logger.Assembly.Info("Save succeeded.");
                var response = Request.CreateResponse(HttpStatusCode.OK, savedEntity.EntityData);
                response.Headers.Location = new Uri(Request.RequestUri, _rootUri + "/" + savedEntity.Id);

                return response;
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public virtual HttpResponseMessage Delete(TId id)
        {
            using (Logger.Assembly.Scope())
            {
                Logger.Assembly.Debug(m => m("Deleting the entity with ID {0}", id));

                if (id.Equals(default(TId)))
                {
                    Logger.Assembly.Info("No data posted.");
                    return
                        new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                try
                {
                    Authorize(EntityPermissions.Delete);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Assembly.Error(m => m("Unauthorized. {0}", ex.Message), ex);
                    var unauthorizedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    unauthorizedResponse.Headers.Add(SuppressFormsAuthenticationRedirectModule.SuppressFormsHeaderName, "true");
                    throw new HttpResponseException(unauthorizedResponse);
                }

                var existingEntity = EntityRepositoryFactory().GetById(id);

                if (existingEntity == null)
                {
                    Logger.Assembly.Info("Existing entity not found.");
                    return
                        new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                existingEntity.Delete();

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }


        /// <summary>
        /// Applies the changes.
        /// </summary>
        /// <param name="newEntityData">The new entity data.</param>
        /// <param name="existingEntityData">The existing entity data.</param>
        protected void ApplyChanges(TEntityData newEntityData, TEntityData existingEntityData)
        {
            using (Logger.Assembly.Scope())
            {
                foreach (var propertyInfo in typeof (TEntityData).GetProperties().Where(IsPropertyIncluded))
                {
                    propertyInfo.SetValue(existingEntityData, propertyInfo.GetValue(newEntityData, null), null);
                }
            }
        }

        protected bool IsPropertyIncluded(PropertyInfo x)
        {
            using (Logger.Assembly.Scope())
            {
                return !_excludedPropertyNames.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase);
            }
        }

        protected void Authorize(EntityPermissions requiredPermissions)
        {
            using(Logger.Assembly.Scope())
            {
                var authManager = PermissionAuthorizationManagerFactory();
                authManager.Authorize(requiredPermissions);
            }
        }
    }
}