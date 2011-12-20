using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Base implementation of IEntityRepository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TEntityRepositoryService">The type of the entity repository service.</typeparam>
    public class EntityRepository<TEntity, TEntityData, TEntityDataImplementation, TEntityRepository,
                                  TEntityRepositoryService>
        : EntityRepository<TEntity, TEntityData, Guid, TEntityDataImplementation, TEntityRepository,
                                  TEntityRepositoryService>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData, IObservableEntity
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityDataImplementation : class, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
        where TEntityRepositoryService : class,
            IEntityRepositoryService<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepository&lt;TEntity, TEntityData, TEntityDataImplementation, TEntityRepository, TEntityRepositoryService&gt;"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityRepositoryServiceFactory">The entity repository service factory.</param>
        /// <param name="entityObservers">The entity observers.</param>
        /// <param name="entityCache">The entity cache.</param>
        public EntityRepository(
            Func<TEntity> entityFactory, 
            Func<TEntityData> entityDataFactory, 
            Func<TEntityRepositoryService> entityRepositoryServiceFactory, 
            IEnumerable<IEntityObserver<TEntity>> entityObservers,
            IEntityCache<TEntity, TEntityData, TEntityRepository> entityCache) 
            : base(entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, entityCache)
        {
        }
    }

    /// <summary>
    /// Base implementation of IEntityRepository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TEntityRepositoryService">The type of the entity repository service.</typeparam>
    public class EntityRepository<TEntity, TEntityData, TId, TEntityDataImplementation, TEntityRepository,
                                  TEntityRepositoryService>
        :
        BaseLogger<EntityRepository<TEntity, TEntityData, TId, TEntityDataImplementation, TEntityRepository, TEntityRepositoryService>>,
        IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityDataImplementation : class, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        where TEntityRepositoryService : class,
            IEntityRepositoryService<TEntityData, TEntity, TEntityRepository, TId, TEntityDataImplementation>
    {
        #region Protected Members

        /// <summary>
        /// The entity data factory.
        /// </summary>
        protected readonly Func<TEntityData> EntityDataFactory;

        /// <summary>
        /// The entity factory
        /// </summary>
        protected readonly Func<TEntity> EntityFactory;

        /// <summary>
        /// Registered entity observers.
        /// </summary>
        protected readonly IEnumerable<IEntityObserver<TEntity, TId>> EntityObservers;

        /// <summary>
        /// The entity repository service factory.
        /// </summary>
        protected readonly Func<TEntityRepositoryService> EntityRepositoryServiceFactory;

        /// <summary>
        /// The entity cache.
        /// </summary>
        protected readonly IEntityCache<TEntity, TId, TEntityData, TEntityRepository> EntityCache;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepository&lt;TEntity, TEntityData, TId, TEntityDataImplementation, TEntityRepository, TEntityRepositoryService&gt;"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityRepositoryServiceFactory">The entity repository service factory.</param>
        /// <param name="entityObservers">The entity observers.</param>
        /// <param name="entityCache">The entity cache.</param>
        public EntityRepository(
            Func<TEntity> entityFactory,
            Func<TEntityData> entityDataFactory,
            Func<TEntityRepositoryService> entityRepositoryServiceFactory,
            IEnumerable<IEntityObserver<TEntity, TId>> entityObservers,
            IEntityCache<TEntity, TId, TEntityData, TEntityRepository> entityCache)
        {
            using (Logger.Scope())
            {
                EntityFactory = entityFactory;
                EntityDataFactory = entityDataFactory;
                EntityRepositoryServiceFactory = entityRepositoryServiceFactory;
                EntityObservers = entityObservers;
                EntityCache = entityCache;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Attaches the observers.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void AttachObservers(TEntity entity)
        {
            using (Logger.Scope())
            {
                foreach (var entityObserver in EntityObservers)
                {
                    Logger.DebugFormat("Attaching entity observer [{0}] to entity [{1}].", entityObserver, entity);
                    entityObserver.Attach(entity);
                }
            }
        }

        /// <summary>
        /// Attaches the observers.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected virtual void AttachObservers(IEnumerable<TEntity> entities)
        {
            using (Logger.Scope())
            {
                foreach (var entity in entities)
                {
                    AttachObservers(entity);
                }
            }
        }

        /// <summary>
        /// Attaches the observers.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void DetatchObservers(TEntity entity)
        {
            using (Logger.Scope())
            {
                foreach (var entityObserver in EntityObservers)
                {
                    Logger.DebugFormat("Detaching entity observer [{0}] to entity [{1}].", entityObserver, entity);
                    entityObserver.Detach(entity);
                }
            }
        }

        /// <summary>
        /// Detatches the observers.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected virtual void DetatchObservers(IEnumerable<TEntity> entities)
        {
            using (Logger.Scope())
            {
                foreach (var entity in entities)
                {
                    DetatchObservers(entity);
                }
            }
        }

        /// <summary>
        /// Pre-initialize the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void PreInitializeEntity(TEntity entity)
        {
        }

        /// <summary>
        /// Post-initialize entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void PostInitializeEntity(TEntity entity)
        {
        }

        /// <summary>
        /// Initializes the entities.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        protected virtual TEntity InitializeEntities(TEntityData entityData)
        {
            using (Logger.Scope())
            {
                return entityData == null
                           ? null
                           : InitializeEntities(new[] { entityData }).Single();
            }
        }

        /// <summary>
        /// Initializes the entities.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> InitializeEntities(IEnumerable<TEntityData> entityData)
        {
            using (Logger.Scope())
            {
                if (entityData == null)
                {
                    yield break;
                }

                foreach (var entityDataInstance in entityData.Where(x => x != null))
                {
                    var entity = EntityFactory();

                    Logger.Debug("Attaching Entity Observers");
                    AttachObservers(entity);

                    Logger.Debug("Pre-initializing Entity");
                    PreInitializeEntity(entity);

                    Logger.Debug("Initializing Entity");
                    entity.Initialize(entityDataInstance);

                    Logger.Debug("Post-initializing Entity");
                    PostInitializeEntity(entity);

                    yield return entity;
                }

                yield break;
            }
        }

        #endregion

        #region Implementation of IEntityRepository<TEntityRepository,TEntity,in TId,TEntityData>

        /// <summary>
        /// Gets the new.
        /// </summary>
        /// <returns></returns>
        public TEntity GetNew()
        {
            using (Logger.Scope())
            {
                return InitializeEntities(EntityDataFactory());
            }
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll(bool forceNew = false)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Getting all [{0}]. ForceNew [{1}].", typeof(TEntity), forceNew);
                var cacheKey = string.Format("GetAll_{0}", typeof(TEntity));

                var entityData = EntityCache.Get(cacheKey);

                if (forceNew || entityData == null)
                {
                    entityData = EntityRepositoryServiceFactory().GetAll().RemoveNulls();
                    EntityCache.Add(cacheKey, entityData.Select(x => x.Clone()));
                }

                return InitializeEntities(entityData.Select(x => x.Clone()));
            }
        }

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <returns></returns>
        public TEntity GetById(TId id, bool forceNew = false)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Getting [{0}] by ID [{1}]. ForceNew [{2}].", typeof(TEntity), id, forceNew);
                var cacheKey = string.Format("GetById_{0}_{1}", typeof(TEntity), id);

                var entityData = EntityCache.Get(cacheKey);

                if (forceNew || entityData == null)
                {
                    entityData = new[] { EntityRepositoryServiceFactory().GetById(id) };
                    EntityCache.Add(cacheKey, entityData.Select(x => x.Clone()));
                }

                return InitializeEntities(entityData.Select(x => x.Clone())).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetByIds(IEnumerable<TId> ids, bool forceNew = false)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Getting [{0}] by ID(s) [{1}]. ForceNew [{2}].", typeof(TEntity), string.Join(",", ids), forceNew);
                var cacheKey = string.Format("GetByIds_{0}_{1}", typeof(TEntity), string.Join("_", ids.OrderBy(x => x)));

                var entityData = EntityCache.Get(cacheKey);

                if (forceNew || entityData == null)
                {
                    entityData = EntityRepositoryServiceFactory().GetByIds(ids).RemoveNulls();
                    EntityCache.Add(cacheKey, entityData.Select(x => x.Clone()));
                }

                return InitializeEntities(entityData.RemoveNulls().Select(x => x.Clone()));
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="version">The version.</param>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <returns></returns>
        public TEntity GetVersion(TEntity entity, int version, bool forceNew = false)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Getting [{0}] by version [{1}]. ForceNew [{2}].", typeof(TEntity), version, forceNew);
                var cacheKey = string.Format("GetVersion_{0}_{1}", entity, version);

                var entityData = EntityCache.Get(cacheKey);

                if (forceNew || entityData == null)
                {
                    var returnedEntityData = EntityRepositoryServiceFactory().GetVersion(
                                             entity.EntityData as TEntityDataImplementation, version);

                    if (returnedEntityData == null)
                    {
                        return null;
                    }

                    entityData = (new[] { returnedEntityData }).RemoveNulls();

                    EntityCache.Add(cacheKey, entityData.Select(x => x.Clone()));
                }

                return InitializeEntities(entityData.Select(x => x.Clone())).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <returns></returns>
        public TEntity GetPreviousVersion(TEntity entity, bool forceNew = false)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Getting previous version of [{0}]. ForceNew [{1}].", typeof(TEntity), forceNew);
                var cacheKey = string.Format("GetPreviousVersion_{0}_{1}", entity, entity.Version);

                var entityData = EntityCache.Get(cacheKey);

                if (forceNew || entityData == null)
                {
                    var returnedEntityData = EntityRepositoryServiceFactory().GetPreviousVersion(
                        entity.EntityData as TEntityDataImplementation);

                    if (returnedEntityData == null)
                    {
                        return null;
                    }

                    entityData = (new[] { returnedEntityData }).RemoveNulls();

                    EntityCache.Add(cacheKey, entityData.Select(x => x.Clone()));
                }

                return InitializeEntities(entityData.Select(x => x.Clone())).FirstOrDefault();
            }
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity Save(TEntity entity)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Saving [{0}].", entity);
                var entityData = EntityRepositoryServiceFactory().Save(entity.EntityData as TEntityDataImplementation);

                EntityCache.RemoveIfDataContains(entityData);
                var cacheKey = string.Format("GetById_{0}_{1}", typeof(TEntity), entityData.Id);
                EntityCache.Add(cacheKey, new[] { entityData.Clone() });

                return InitializeEntities(entityData.Clone());
            }
        }

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> SaveAll(IEnumerable<TEntity> entities)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Saving all [{0}].", string.Join(",", entities));
                var entityData =
                    EntityRepositoryServiceFactory().SaveAll(entities.Select(x => x.EntityData).Cast<TEntityDataImplementation>());

                EntityCache.RemoveIfDataContains(entityData);
                foreach (var entityDataImplementation in entityData.Select(x => x.Clone()))
                {
                    var cacheKey = string.Format("GetById_{0}_{1}", typeof(TEntity), entityDataImplementation.Id);
                    EntityCache.Add(cacheKey, new[] { entityDataImplementation });
                }

                return InitializeEntities(entityData.Select(x => x.Clone()));
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Delete(TEntity entity)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Deleting [{0}].", entity);
                EntityRepositoryServiceFactory().Delete(entity.EntityData as TEntityDataImplementation);

                DetatchObservers(entity);

                EntityCache.RemoveIfDataContains(entity.EntityData);
            }
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void DeleteAll(IEnumerable<TEntity> entities)
        {
            using (Logger.Scope())
            {
                Logger.DebugFormat("Deleting all [{0}].", string.Join(",", entities));

                EntityRepositoryServiceFactory().DeleteAll(entities.Select(x => x.EntityData).Cast<TEntityDataImplementation>());
                DetatchObservers(entities);

                EntityCache.RemoveIfDataContains(entities.Select(x => x.EntityData));
            }
        }

        #endregion
    }
}