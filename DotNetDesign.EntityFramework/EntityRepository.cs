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
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TEntityRepositoryService">The type of the entity repository service.</typeparam>
    public class EntityRepository<TEntity, TEntityData, TId, TEntityDataImplementation, TEntityRepository,
                                  TEntityRepositoryService>
        : IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
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
        /// The entity repository service.
        /// </summary>
        protected readonly TEntityRepositoryService EntityRepositoryService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepository&lt;TEntity, TEntityData, TId, TEntityDataImplementation, TEntityRepository, TEntityRepositoryService&gt;"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityRepositoryService">The entity repository service.</param>
        /// <param name="entityObservers">The entity observers.</param>
        public EntityRepository(Func<TEntity> entityFactory, Func<TEntityData> entityDataFactory,
                                TEntityRepositoryService entityRepositoryService,
                                IEnumerable<IEntityObserver<TEntity, TId>> entityObservers)
        {
            EntityFactory = entityFactory;
            EntityDataFactory = entityDataFactory;
            EntityRepositoryService = entityRepositoryService;
            EntityObservers = entityObservers;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Attaches the observers.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void AttachObservers(TEntity entity)
        {
            foreach (var entityObserver in EntityObservers)
            {
                entityObserver.Attach(entity);
            }
        }

        /// <summary>
        /// Attaches the observers.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected virtual void AttachObservers(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                AttachObservers(entity);
            }
        }

        /// <summary>
        /// Attaches the observers.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void DetatchObservers(TEntity entity)
        {
            foreach (var entityObserver in EntityObservers)
            {
                entityObserver.Detach(entity);
            }
        }

        /// <summary>
        /// Detatches the observers.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected virtual void DetatchObservers(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DetatchObservers(entity);
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
            return InitializeEntities(new[] {entityData}).Single();
        }

        /// <summary>
        /// Initializes the entities.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> InitializeEntities(IEnumerable<TEntityData> entityData)
        {
            if (entityData == null)
            {
                yield break;
            }

            foreach (var entityDataInstance in entityData.Where(x => x != null))
            {
                var entity = EntityFactory();

                AttachObservers(entity);

                PreInitializeEntity(entity);

                entity.Initialize(entityDataInstance);

                PostInitializeEntity(entity);

                yield return entity;
            }

            yield break;
        }

        #endregion

        #region Implementation of IEntityRepository<TEntityRepository,TEntity,in TId,TEntityData>

        /// <summary>
        /// Gets the new.
        /// </summary>
        /// <returns></returns>
        public TEntity GetNew()
        {
            var entity = EntityFactory();
            AttachObservers(entity);
            entity.Initialize(EntityDataFactory());
            return entity;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll()
        {
            var entityData = EntityRepositoryService.GetAll();
            return InitializeEntities(entityData);
        }

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public TEntity GetById(TId id)
        {
            var entityData = EntityRepositoryService.GetById(id);
            return InitializeEntities(entityData);
        }

        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetByIds(IEnumerable<TId> ids)
        {
            var entityData = EntityRepositoryService.GetByIds(ids);
            return InitializeEntities(entityData);
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public TEntity GetVersion(TEntity entity, int version)
        {
            var entityData = EntityRepositoryService.GetVersion(entity.EntityData as TEntityDataImplementation, version);
            return InitializeEntities(entityData);
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity GetPreviousVersion(TEntity entity)
        {
            var entityData = EntityRepositoryService.GetPreviousVersion(entity.EntityData as TEntityDataImplementation);
            return InitializeEntities(entityData);
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity Save(TEntity entity)
        {
            var entityData = EntityRepositoryService.Save(entity.EntityData as TEntityDataImplementation);
            return InitializeEntities(entityData);
        }

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> SaveAll(IEnumerable<TEntity> entities)
        {
            var entityData =
                EntityRepositoryService.SaveAll(entities.Select(x => x.EntityData).Cast<TEntityDataImplementation>());
            return InitializeEntities(entityData);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Delete(TEntity entity)
        {
            EntityRepositoryService.Delete(entity.EntityData as TEntityDataImplementation);
            DetatchObservers(entity);
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void DeleteAll(IEnumerable<TEntity> entities)
        {
            EntityRepositoryService.DeleteAll(entities.Select(x => x.EntityData).Cast<TEntityDataImplementation>());
            DetatchObservers(entities);
        }

        #endregion
    }
}