using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Common.Logging;

namespace DotNetDesign.Substrate.WebApi
{
    public class BaseEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation, TApiKey> :
        BaseEntityRepositoryController<TEntityData, TEntity, TEntityRepository, Guid, TEntityDataImplementation, TApiKey>, IEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation, TApiKey>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
        public BaseEntityRepositoryController(
            Func<IApiKeyPermissionAuthorizationManager<TEntity, TEntityData, TEntityRepository, TApiKey>> apiKeyPermissionAuthorizationManagerFactory, 
            Func<TEntityRepository> entityRepositoryFactory, 
            Func<TEntity> entityFactory,
            string rootUri,
            params string[] excludedPropertyNames)
            : base(apiKeyPermissionAuthorizationManagerFactory, entityRepositoryFactory, entityFactory, rootUri, excludedPropertyNames)
        {
        }
    }

    public class BaseEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation, TApiKey> :
        ApiController, IEntityRepositoryController<TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation, TApiKey>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
        protected readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Property names to excluded when comparing changes by default.
        /// </summary>
        public static readonly IEnumerable<string> DefaultExcludedPropertyNames = new[] { "Id", "CreatedAt", "UpdatedAt", "Version", "VersionId" };
        private readonly IEnumerable<string> _excludedPropertyNames;

        protected readonly Func<IApiKeyPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository, TApiKey>> ApiKeyPermissionAuthorizationManagerFactory;
        protected readonly Func<TEntityRepository> EntityRepositoryFactory;
        protected readonly Func<TEntity> EntityFactory;
        private readonly string _rootUri;

        public BaseEntityRepositoryController(
            Func<IApiKeyPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository, TApiKey>> apiKeyPermissionAuthorizationManagerFactory,
            Func<TEntityRepository> entityRepositoryFactory,
            Func<TEntity> entityFactory,
            string rootUri,
            params string[] excludedPropertyNames)
        {
            using (Logger.Scope())
            {
                ApiKeyPermissionAuthorizationManagerFactory = apiKeyPermissionAuthorizationManagerFactory;
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
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        public IQueryable<TEntityDataImplementation> Get(TApiKey apiKey)
        {
            using(Logger.Scope())
            {
                Logger.DebugFormat("Getting all entities of type {0}", typeof(TEntity));

                try
                {
                    ApiKeyAuthorization(apiKey, EntityPermissions.Read);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.ErrorFormat("Unauthorized. {0}", ex.Message);
                    throw new HttpResponseException(ex.Message, HttpStatusCode.Unauthorized);
                }

                var entities = EntityRepositoryFactory().GetAll().Select(x => x.EntityData).Cast<TEntityDataImplementation>();

                return entities.AsQueryable();
            }
        }

        /// <summary>
        /// Gets the entity data by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        public TEntityDataImplementation Get(TId id, TApiKey apiKey)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Getting entity of type {0} by id {1}", typeof(TEntity), id);

                try
                {
                    ApiKeyAuthorization(apiKey, EntityPermissions.Read);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.ErrorFormat("Unauthorized. {0}", ex.Message);
                    throw new HttpResponseException(ex.Message, HttpStatusCode.Unauthorized);
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
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        public TEntityDataImplementation Get(TId id, int version, TApiKey apiKey)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Getting entity of type {0} by id {1} and version {2}", typeof(TEntity), id, version);

                try
                {
                    ApiKeyAuthorization(apiKey, EntityPermissions.Read);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.ErrorFormat("Unauthorized. {0}", ex.Message);
                    throw new HttpResponseException(ex.Message, HttpStatusCode.Unauthorized);
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
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        public HttpResponseMessage<TEntityDataImplementation> Post(TEntityDataImplementation entityData, TApiKey apiKey)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Adding the entity {0}", entityData);

                if (entityData == null)
                {
                    Logger.Info("No data posted.");
                    return new HttpResponseMessage<TEntityDataImplementation>(HttpStatusCode.BadRequest);
                }

                try
                {
                    ApiKeyAuthorization(apiKey, EntityPermissions.Insert);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.ErrorFormat("Unauthorized. {0}", ex.Message);
                    var unauthorizedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    unauthorizedResponse.Headers.Add(SuppressFormsAuthenticationRedirectModule.SuppressFormsHeaderName, "true");
                    throw new HttpResponseException(unauthorizedResponse);
                }

                var existingEntity = EntityRepositoryFactory().GetNew();

                ApplyChanges(entityData, existingEntity);

                TEntity savedEntity;
                if (!existingEntity.Save(out savedEntity))
                {
                    Logger.Info("Validation failed.");
                    return new HttpResponseMessage<TEntityDataImplementation>(
                        (existingEntity as TEntityDataImplementation),
                        HttpStatusCode.BadRequest);
                }

                Logger.Info("Save succeeded.");
                var response = new HttpResponseMessage<TEntityDataImplementation>(
                    (savedEntity.EntityData as TEntityDataImplementation),
                    HttpStatusCode.Created);
                response.Headers.Location = new Uri(Request.RequestUri, _rootUri + "/" + savedEntity.Id);

                return response;
            }
        }

        /// <summary>
        /// Saves the specified entity data.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <param name="entityData">The entity data.</param>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        public HttpResponseMessage<TEntityDataImplementation> Put(TId id, TEntityDataImplementation entityData, TApiKey apiKey)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Saving the entity {0}", entityData);

                if (entityData == null)
                {
                    Logger.Info("No data posted.");
                    return
                        new HttpResponseMessage<TEntityDataImplementation>(HttpStatusCode.BadRequest);
                }

                try
                {
                    ApiKeyAuthorization(apiKey, EntityPermissions.Update);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.ErrorFormat("Unauthorized. {0}", ex.Message);
                    var unauthorizedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    unauthorizedResponse.Headers.Add(SuppressFormsAuthenticationRedirectModule.SuppressFormsHeaderName, "true");
                    throw new HttpResponseException(unauthorizedResponse);
                }

                var existingEntity = EntityRepositoryFactory().GetById(id);

                if (existingEntity == null)
                {
                    Logger.Info("Existing entity not found.");
                    return
                        new HttpResponseMessage<TEntityDataImplementation>(HttpStatusCode.NotFound);
                }

                ApplyChanges(entityData, existingEntity);

                TEntity savedEntity;
                if (!existingEntity.Save(out savedEntity))
                {
                    Logger.Info("Validation failed.");
                    return new HttpResponseMessage<TEntityDataImplementation>(
                        (existingEntity as TEntityDataImplementation),
                        HttpStatusCode.BadRequest);
                }

                Logger.Info("Save succeeded.");
                var response = new HttpResponseMessage<TEntityDataImplementation>((savedEntity.EntityData as TEntityDataImplementation));
                response.Headers.Location = new Uri(Request.RequestUri, _rootUri + "/" + savedEntity.Id);

                return response;
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="apiKey">The API key.</param>
        public HttpResponseMessage Delete(TId id, TApiKey apiKey)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Deleting the entity with ID {0}", id);

                if (id.Equals(default(TId)))
                {
                    Logger.Info("No data posted.");
                    return
                        new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                try
                {
                    ApiKeyAuthorization(apiKey, EntityPermissions.Delete);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.ErrorFormat("Unauthorized. {0}", ex.Message);
                    var unauthorizedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    unauthorizedResponse.Headers.Add(SuppressFormsAuthenticationRedirectModule.SuppressFormsHeaderName, "true");
                    throw new HttpResponseException(unauthorizedResponse);
                }

                var existingEntity = EntityRepositoryFactory().GetById(id);

                if (existingEntity == null)
                {
                    Logger.Info("Existing entity not found.");
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
            foreach (var propertyInfo in typeof(TEntityData).GetProperties().Where(IsPropertyIncluded))
            {
                propertyInfo.SetValue(existingEntityData, propertyInfo.GetValue(newEntityData, null), null);
            }
        }

        private bool IsPropertyIncluded(PropertyInfo x)
        {
            using (Logger.Scope())
            {
                return !_excludedPropertyNames.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase);
            }
        }

        private void ApiKeyAuthorization(TApiKey apiKey, EntityPermissions requiredPermissions)
        {
            using(Logger.Scope())
            {
                var authManager = ApiKeyPermissionAuthorizationManagerFactory();
                authManager.ApiKey = apiKey;
                authManager.Authorize(requiredPermissions);
            }
        }
    }
}