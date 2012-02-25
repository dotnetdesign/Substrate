using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Activation;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Implementation of <see cref="IEntityRepositoryService&lt;TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation&gt;"/> that handles WebHttpBinding calls.
    /// The purpose of this class is to expose a RESTful api that will still provide full authoriziation and validation.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WebHttpEntityRepositoryService<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation> :
        WebHttpEntityRepositoryService<TEntityData, TEntity, TEntityRepository, Guid, TEntityDataImplementation>,
        IWebHttpEntityRepositoryService<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebHttpEntityRepositoryService&lt;TEntityData, TEntity, TEntityRepository, TEntityDataImplementation&gt;"/> class.
        /// </summary>
        /// <param name="permissionAuthorizationManagerFactory">The permission authorization manager factory.</param>
        /// <param name="entityRepositoryFactory">The entity repository factory.</param>
        /// <param name="entityFactory">The entity factory.</param>
        /// <param name="excludedPropertyNames">The excluded property names.</param>
        public WebHttpEntityRepositoryService(
            Func<IPermissionAuthorizationManager<TEntity, TEntityData, TEntityRepository>> permissionAuthorizationManagerFactory, 
            Func<TEntityRepository> entityRepositoryFactory,
            Func<TEntity> entityFactory,
            IEnumerable<string> excludedPropertyNames = null)
            : base(permissionAuthorizationManagerFactory, entityRepositoryFactory, entityFactory, excludedPropertyNames)
        {
        }
    }

    /// <summary>
    /// Implementation of <see cref="IEntityRepositoryService&lt;TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation&gt;"/> that handles WebHttpBinding calls.
    /// The purpose of this class is to expose a RESTful api that will still provide full authoriziation and validation.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WebHttpEntityRepositoryService<TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation> :
        BaseLogger<WebHttpEntityRepositoryService<TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation>>,
        IWebHttpEntityRepositoryService<TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {

        /// <summary>
        /// Property names to excluded when comparing changes by default.
        /// </summary>
        public static readonly IEnumerable<string> DefaultExcludedPropertyNames = new[] {"Id", "CreatedAt", "UpdatedAt", "Version", "VersionId"};
        private readonly IEnumerable<string> _excludedPropertyNames;

        protected readonly Func<IPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository>> PermissionAuthorizationManagerFactory;
        protected readonly Func<TEntityRepository> EntityRepositoryFactory;
        protected readonly Func<TEntity> EntityFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHttpEntityRepositoryService&lt;TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation&gt;"/> class.
        /// </summary>
        /// <param name="permissionAuthorizationManagerFactory">The permission authorization manager factory.</param>
        /// <param name="entityRepositoryFactory">The entity repository factory.</param>
        /// <param name="entityFactory">The entity factory.</param>
        /// <param name="excludedPropertyNames">The excluded property names.</param>
        public WebHttpEntityRepositoryService(
            Func<IPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository>> permissionAuthorizationManagerFactory,
            Func<TEntityRepository> entityRepositoryFactory,
            Func<TEntity> entityFactory, 
            IEnumerable<string> excludedPropertyNames = null)
        {
            using (Logger.Scope())
            {
                PermissionAuthorizationManagerFactory = permissionAuthorizationManagerFactory;
                EntityRepositoryFactory = entityRepositoryFactory;
                EntityFactory = entityFactory;

                var excludedPropertyNamesList = new List<string>(DefaultExcludedPropertyNames);
                if (excludedPropertyNames != null)
                {
                    excludedPropertyNamesList.AddRange(excludedPropertyNames);
                }
                _excludedPropertyNames = excludedPropertyNamesList;
            }
        }

        /// <summary>
        /// Gets the entity data by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public virtual WebHttpEntityContainer<TEntityDataImplementation> GetById(TId id)
        {
            using (Logger.Scope())
            {
                try
                {
                    Logger.DebugFormat("Getting entity of type {0} by id {1}", typeof (TEntity), id);

                    var entity = EntityRepositoryFactory().GetById(id);

                    return (entity == null)
                               ? WebHttpEntityContainer<TEntityDataImplementation>.NullSuccessContainer()
                               : (entity.EntityData as TEntityDataImplementation).AsSuccessContainer();
                }
                catch(Exception ex)
                {
                    return ex.AsFailureContainer<TEntityDataImplementation>();
                }
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public virtual WebHttpEntityContainer<TEntityDataImplementation> GetVersion(TId id, int version)
        {
            using (Logger.Scope())
            {
                try
                {
                    Logger.DebugFormat("Getting entity of type {0} by id {1} and version {2}", typeof(TEntity), id, version);

                    var entity = EntityRepositoryFactory().GetVersion(id, version);

                    return (entity == null)
                               ? WebHttpEntityContainer<TEntityDataImplementation>.NullSuccessContainer()
                               : (entity.EntityData as TEntityDataImplementation).AsSuccessContainer();
                }
                catch (Exception ex)
                {
                    return ex.AsFailureContainer<TEntityDataImplementation>();
                }
            }
        }

        /// <summary>
        /// Gets the entity data by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        public virtual WebHttpEntityContainer<IEnumerable<TEntityDataImplementation>> GetByIds(IEnumerable<TId> ids)
        {
            using (Logger.Scope())
            {
                try
                {
                    Logger.DebugFormat("Getting all entities of type {0} with ids {1}", typeof (TEntity),
                                       string.Join(",", ids));

                    var entities = EntityRepositoryFactory().GetByIds(ids).Select(x => x.EntityData).Cast<TEntityDataImplementation>();

                    return entities.AsSuccessContainer();
                }
                catch (Exception ex)
                {
                    return ex.AsFailureContainer<IEnumerable<TEntityDataImplementation>>();
                }
            }
        }

        /// <summary>
        /// Gets all entity data.
        /// </summary>
        /// <returns></returns>
        public virtual WebHttpEntityContainer<IEnumerable<TEntityDataImplementation>> GetAll()
        {
            using (Logger.Scope())
            {
                try
                {
                    Logger.DebugFormat("Getting all entities of type {0}", typeof (TEntity));

                    var entities = EntityRepositoryFactory().GetAll().Select(x => x.EntityData).Cast<TEntityDataImplementation>();

                    return entities.AsSuccessContainer();
                }
                catch (Exception ex)
                {
                    return ex.AsFailureContainer<IEnumerable<TEntityDataImplementation>>();
                }
            }
        }

        /// <summary>
        /// Saves the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        public virtual WebHttpEntityContainer<TEntityDataImplementation> Save(TEntityDataImplementation entityData)
        {
            using (Logger.Scope())
            {
                try
                {
                    IEnumerable<ValidationResult> validationResults;
                    var savedEntity = InternalSave(entityData, out validationResults);
                    if (savedEntity == null)
                    {
                        return WebHttpEntityContainer<TEntityDataImplementation>.NullFailureContainer(
                            "Invalid request.", validationResults);
                    }

                    if (validationResults != null)
                    {
                        return savedEntity.AsFailureContainer(validationResults: validationResults);
                    }

                    return savedEntity.AsSuccessContainer(validationResults: validationResults);
                }
                catch (Exception ex)
                {
                    return ex.AsFailureContainer<TEntityDataImplementation>();
                }
            }
        }

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        public virtual IEnumerable<WebHttpEntityContainer<TEntityDataImplementation>> SaveAll(IEnumerable<TEntityDataImplementation> entityData)
        {
            using (Logger.Scope())
            {
                try
                {
                    Logger.DebugFormat("Saving the entities {0}", string.Join(",", entityData));

                    return entityData.Select(Save);
                }
                catch (Exception ex)
                {
                    return new[] {ex.AsFailureContainer<TEntityDataImplementation>()};
                }
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public virtual WebHttpEntityContainer<object> Delete(TId id)
        {
            using (Logger.Scope())
            {
                try
                {
                    InternalDelete(id);
                    return WebHttpEntityContainer<object>.NullSuccessContainer();
                }
                catch (Exception ex)
                {
                    return ex.AsFailureContainer<object>();
                }
            }
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="ids">The ids.</param>
        public virtual WebHttpEntityContainer<object> DeleteAll(IEnumerable<TId> ids)
        {
            using (Logger.Scope())
            {
                try
                {
                    Logger.DebugFormat("Deleting the entities of type {0} by ids {1}", typeof(TEntity), string.Join(",", ids));

                    foreach (var id in ids)
                    {
                        InternalDelete(id);
                    }
                    return WebHttpEntityContainer<object>.NullSuccessContainer();
                }
                catch (Exception ex)
                {
                    return ex.AsFailureContainer<object>();
                }
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

        private TEntityDataImplementation InternalSave(TEntityDataImplementation entityData, out IEnumerable<ValidationResult> validationResults)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Saving the entity {0}", entityData);
                validationResults = null;

                if (entityData == null)
                {
                    return null;
                }

                var isNew = entityData.Id.Equals(default(TId));

                var requiredPermissions = isNew
                                              ? EntityPermissions.Insert
                                              : EntityPermissions.Update;

                PermissionAuthorizationManagerFactory().Authorize(requiredPermissions);

                var existingEntity = isNew
                                         ? EntityRepositoryFactory().GetNew()
                                         : EntityRepositoryFactory().GetById(entityData.Id);

                if (existingEntity == null)
                {
                    return null;
                }

                ApplyChanges(entityData, existingEntity);

                if (!existingEntity.IsDirty)
                {
                    return entityData;
                }

                TEntity savedEntity;
                if (!existingEntity.Save(out savedEntity))
                {
                    validationResults = existingEntity.Validate();
                }

                return savedEntity.EntityData as TEntityDataImplementation;
            }
        }
        
        private void InternalDelete(TId id)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Deleting the entity of type {0} by id {1}", typeof(TEntity), id);

                PermissionAuthorizationManagerFactory().Authorize(EntityPermissions.Delete);

                var entity = EntityRepositoryFactory().GetById(id);

                if (entity != null)
                {
                    EntityRepositoryFactory().Delete(entity);
                }
            }
        }
    }
}