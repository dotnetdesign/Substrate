using System;
using System.Collections.Generic;
using System.Linq;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
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
        /// <param name="entityCacheFactory">The entity cache factory.</param>
        /// <param name="scopeManagerFactory">The scope manager factory.</param>
        public EntityRepository(
            Func<TEntity> entityFactory,
            Func<TEntityData> entityDataFactory,
            Func<TEntityRepositoryService> entityRepositoryServiceFactory,
            IEnumerable<IEntityObserver<TEntity>> entityObservers,
            Func<IEntityCache<TEntity, TEntityData, TEntityRepository>> entityCacheFactory,
            Func<IScopeManager> scopeManagerFactory)
            : base(
                entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, entityCacheFactory,
                scopeManagerFactory)
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
                                  TEntityRepositoryService> :
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
        protected readonly Func<IEntityCache<TEntity, TId, TEntityData, TEntityRepository>> EntityCacheFactory;

        /// <summary>
        /// The scope manager factory.
        /// </summary>
        protected readonly Func<IScopeManager> ScopeManagerFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepository&lt;TEntity, TEntityData, TId, TEntityDataImplementation, TEntityRepository, TEntityRepositoryService&gt;"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityRepositoryServiceFactory">The entity repository service factory.</param>
        /// <param name="entityObservers">The entity observers.</param>
        /// <param name="entityCacheFactory">The entity cache factory.</param>
        /// <param name="scopeManagerFactory">The scope manager factory.</param>
        public EntityRepository(
            Func<TEntity> entityFactory,
            Func<TEntityData> entityDataFactory,
            Func<TEntityRepositoryService> entityRepositoryServiceFactory,
            IEnumerable<IEntityObserver<TEntity, TId>> entityObservers,
            Func<IEntityCache<TEntity, TId, TEntityData, TEntityRepository>> entityCacheFactory,
            Func<IScopeManager> scopeManagerFactory)
        {
            using (Logger.Assembly.Scope())
            {
                // ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(entityFactory, "entityFactory");
                Guard.ArgumentNotNull(entityDataFactory, "entityDataFactory");
                Guard.ArgumentNotNull(entityRepositoryServiceFactory, "entityRepositoryServiceFactory");
                Guard.ArgumentNotNull(entityObservers, "entityObservers");
                Guard.ArgumentNotNull(entityCacheFactory, "entityCacheFactory");
                Guard.ArgumentNotNull(scopeManagerFactory, "scopeManagerFactory");

                EntityFactory = entityFactory;
                EntityDataFactory = entityDataFactory;
                EntityRepositoryServiceFactory = entityRepositoryServiceFactory;
                EntityObservers = entityObservers;
                EntityCacheFactory = entityCacheFactory;
                ScopeManagerFactory = scopeManagerFactory;
                // ReSharper restore PossibleMultipleEnumeration
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
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNull(entity, "entity");

                foreach (var entityObserver in EntityObservers)
                {
                    // ReSharper disable AccessToForEachVariableInClosure
                    Logger.Assembly.Debug(
                        m => m("Attaching entity observer [{0}] to entity [{1}].", entityObserver, entity));
                    // ReSharper restore AccessToForEachVariableInClosure
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
            using (Logger.Assembly.Scope())
            {
                // ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(entities, "entities");

                foreach (var entity in entities)
                {
                    AttachObservers(entity);
                }
                // ReSharper restore PossibleMultipleEnumeration
            }
        }

        /// <summary>
        /// Attaches the observers.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void DetatchObservers(TEntity entity)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNull(entity, "entity");

                foreach (var entityObserver in EntityObservers)
                {
                    // ReSharper disable AccessToForEachVariableInClosure
                    Logger.Assembly.Debug(
                        m => m("Detaching entity observer [{0}] to entity [{1}].", entityObserver, entity));
                    // ReSharper restore AccessToForEachVariableInClosure
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
            using (Logger.Assembly.Scope())
            {
                // ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(entities, "entities");

                foreach (var entity in entities)
                {
                    DetatchObservers(entity);
                }
                // ReSharper restore PossibleMultipleEnumeration
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
            using (Logger.Assembly.Scope())
            {
                return entityData == null
                           ? null
                           : InitializeEntities(new[] {entityData}).Single();
            }
        }

        /// <summary>
        /// Initializes the entities.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> InitializeEntities(IEnumerable<TEntityData> entityData)
        {
            using (Logger.Assembly.Scope())
            {
                if (entityData == null)
                {
                    yield break;
                }

                foreach (var entityDataInstance in entityData.Where(x => x != null))
                {
                    var entity = EntityFactory();

                    Logger.Assembly.Debug("Attaching Entity Observers");
                    AttachObservers(entity);

                    Logger.Assembly.Debug("Pre-initializing Entity");
                    PreInitializeEntity(entity);

                    Logger.Assembly.Debug("Initializing Entity");
                    entity.Initialize(entityDataInstance);

                    Logger.Assembly.Debug("Post-initializing Entity");
                    PostInitializeEntity(entity);

                    yield return entity;
                }
            }
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="repositoryCall">The repository call.</param>
        /// <returns></returns>
        protected virtual TEntity ProcessRequest(
            bool forceNew,
            string cacheKey,
            Func<Dictionary<string, string>, TEntityData> repositoryCall)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNullOrEmpty(cacheKey, "cacheKey");
                Guard.ArgumentNotNull(repositoryCall, "repositoryCall");

                return
                    ProcessRequestForEnumerable(forceNew, cacheKey, x => (new[] {repositoryCall(x)})).FirstOrDefault();
            }
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="repositoryCall">The repository call.</param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> ProcessRequestForEnumerable(
            bool forceNew,
            string cacheKey,
            Func<Dictionary<string, string>, IEnumerable<TEntityData>> repositoryCall)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNullOrEmpty(cacheKey, "cacheKey");
                Guard.ArgumentNotNull(repositoryCall, "repositoryCall");

                var entityData = EntityCacheFactory().Get(cacheKey);

                // ReSharper disable PossibleMultipleEnumeration
                if (forceNew || entityData == null)
                {
                    entityData = repositoryCall(ScopeManagerFactory().GetScopeContext()).RemoveNulls().ToArray();
                    EntityCacheFactory()
                        .Add(cacheKey, entityData.GetClones<TEntityData, TEntity, TId, TEntityRepository>());
                }

                return InitializeEntities(entityData.GetClones<TEntityData, TEntity, TId, TEntityRepository>());
                // ReSharper restore PossibleMultipleEnumeration
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
            {
                Logger.Assembly.Debug(m => m("Getting all [{0}]. ForceNew [{1}].", typeof (TEntity), forceNew));
                var cacheKey = string.Format("GetAll_{0}", typeof (TEntity));

                return ProcessRequestForEnumerable(forceNew, cacheKey, x => EntityRepositoryServiceFactory().GetAll(x));
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
            using (Logger.Assembly.Scope())
            {
                // ReSharper disable ImplicitlyCapturedClosure
                Logger.Assembly.Debug(
                    m => m("Getting [{0}] by ID [{1}]. ForceNew [{2}].", typeof (TEntity), id, forceNew));
                // ReSharper restore ImplicitlyCapturedClosure
                var cacheKey = string.Format("GetById_{0}_{1}", typeof (TEntity), id);

                return ProcessRequest(forceNew, cacheKey, x => EntityRepositoryServiceFactory().GetById(id, x));
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
            using (Logger.Assembly.Scope())
            {
                // ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(ids, "ids");
                var idsArray = ids as TId[] ?? ids.ToArray();
                // ReSharper restore PossibleMultipleEnumeration

                Logger.Assembly.Debug(
                    m =>
                    m("Getting [{0}] by ID(s) [{1}]. ForceNew [{2}].", typeof (TEntity), string.Join(",", idsArray),
                      forceNew));
                var cacheKey = string.Format("GetByIds_{0}_{1}", typeof (TEntity),
                                             string.Join("_", idsArray.OrderBy(x => x)));

                return ProcessRequestForEnumerable(forceNew, cacheKey,
                                                   x => EntityRepositoryServiceFactory().GetByIds(idsArray, x));
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <returns></returns>
        public TEntity GetVersion(TId id, int version, bool forceNew = false)
        {
            using (Logger.Assembly.Scope())
            {
                // ReSharper disable ImplicitlyCapturedClosure
                Logger.Assembly.Debug(
                    m => m("Getting [{0}] by version [{1}]. ForceNew [{2}].", typeof (TEntity), version, forceNew));
                // ReSharper restore ImplicitlyCapturedClosure
                var cacheKey = string.Format("GetVersion_{0}_{1}_{2}", typeof (TEntity), id, version);

                return ProcessRequest(forceNew, cacheKey,
                                      x => EntityRepositoryServiceFactory().GetVersion(id, version, x));
            }
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity Save(TEntity entity)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNull(entity, "entity");

                Logger.Assembly.Debug(m => m("Saving [{0}].", entity));
                var entityData = EntityRepositoryServiceFactory()
                    .Save(entity.EntityData as TEntityDataImplementation, ScopeManagerFactory().GetScopeContext());

                EntityCacheFactory().RemoveIfDataContains(entityData);
                var cacheKey = string.Format("GetById_{0}_{1}", typeof (TEntity), entityData.Id);
                EntityCacheFactory().Add(cacheKey, new[] {entityData.Clone()});

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
            using (Logger.Assembly.Scope())
            {
                // ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(entities, "entities");
                var entitiesArray = entities as TEntity[] ?? entities.ToArray();
                // ReSharper restore PossibleMultipleEnumeration

                Logger.Assembly.Debug(m => m("Saving all [{0}].", string.Join<TEntity>(",", entitiesArray)));
                var entityData =
                    EntityRepositoryServiceFactory()
                        .SaveAll(entitiesArray.Select(x => x.EntityData).Cast<TEntityDataImplementation>(),
                                 ScopeManagerFactory().GetScopeContext());

                var entityDataImplementations = entityData as TEntityDataImplementation[] ?? entityData.ToArray();
                EntityCacheFactory().RemoveIfDataContains(entityDataImplementations);
                foreach (
                    var entityDataImplementation in
                        entityDataImplementations.GetClones<TEntityData, TEntity, TId, TEntityRepository>())
                {
                    var cacheKey = string.Format("GetById_{0}_{1}", typeof (TEntity), entityDataImplementation.Id);
                    EntityCacheFactory().Add(cacheKey, new[] {entityDataImplementation});
                }

                return
                    InitializeEntities(
                        entityDataImplementations.GetClones<TEntityData, TEntity, TId, TEntityRepository>());
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Delete(TEntity entity)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNull(entity, "entity");

                Logger.Assembly.Debug(m => m("Deleting [{0}].", entity));
                EntityRepositoryServiceFactory().Delete(entity.Id, ScopeManagerFactory().GetScopeContext());

                DetatchObservers(entity);

                EntityCacheFactory().RemoveIfDataContains(entity.EntityData);
            }
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void DeleteAll(IEnumerable<TEntity> entities)
        {
            using (Logger.Assembly.Scope())
            {
                // ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(entities, "entities");
                var entitiesArray = entities as TEntity[] ?? entities.ToArray();
                // ReSharper restore PossibleMultipleEnumeration

                Logger.Assembly.Debug(m => m("Deleting all [{0}].", string.Join<TEntity>(",", entitiesArray)));

                EntityRepositoryServiceFactory()
                    .DeleteAll(entitiesArray.Select(x => x.Id), ScopeManagerFactory().GetScopeContext());
                DetatchObservers(entitiesArray);

                EntityCacheFactory().RemoveIfDataContains(entitiesArray.Select(x => x.EntityData));
            }
        }

        #endregion
    }
}