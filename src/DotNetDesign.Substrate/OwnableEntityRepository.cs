using System;
using System.Collections.Generic;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Base implementation of IOwnableEntityRepository.
    /// </summary>
    /// <typeparam name="TOwnableEntity">The type of the ownable entity.</typeparam>
    /// <typeparam name="TOwnableEntityData">The type of the ownable entity data.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    /// <typeparam name="TOwnableEntityRepository">The type of the ownable entity repository.</typeparam>
    /// <typeparam name="TOwnableEntityRepositoryService">The type of the ownable entity repository service.</typeparam>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    /// <typeparam name="TOwnerData">The type of the owner data.</typeparam>
    /// <typeparam name="TOwnerRepository">The type of the owner repository.</typeparam>
    public class OwnableEntityRepository<TOwnableEntity, TOwnableEntityData, TEntityDataImplementation,
                                         TOwnableEntityRepository,
                                         TOwnableEntityRepositoryService, TOwner, TOwnerData, TOwnerRepository> :
                                             OwnableEntityRepository
                                                 <TOwnableEntity, TOwnableEntityData, Guid, TEntityDataImplementation,
                                                 TOwnableEntityRepository, TOwnableEntityRepositoryService, TOwner,
                                                 TOwnerData, TOwnerRepository>
        where TOwnableEntity : class,
            IOwnableEntity
                <TOwnableEntity, TOwnableEntityData, TOwner, TOwnableEntityRepository, TOwnerData, TOwnerRepository>,
            TOwnableEntityData
        where TOwnableEntityData : class,
            IOwnableEntityData
                <TOwnableEntityData, TOwnableEntity, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TEntityDataImplementation : class, TOwnableEntityData
        where TOwnableEntityRepository : class, IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity,
                                                    TOwnableEntityData, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntityRepositoryService : class,
            IOwnableEntityRepositoryService
                <TOwnableEntityData, TOwnableEntity, TOwnableEntityRepository, TEntityDataImplementation, TOwner,
                TOwnerData, TOwnerRepository>
        where TOwnerData : class, IEntityData<TOwnerData, TOwner, TOwnerRepository>
        where TOwner : class, IEntity<TOwner, TOwnerData, TOwnerRepository>, TOwnerData
        where TOwnerRepository : class, IEntityRepository<TOwnerRepository, TOwner, TOwnerData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnableEntityRepository&lt;TOwnableEntity, TOwnableEntityData, TEntityDataImplementation, TOwnableEntityRepository, TOwnableEntityRepositoryService, TOwner, TOwnerData, TOwnerRepository&gt;"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityRepositoryServiceFactory">The entity repository service factory.</param>
        /// <param name="entityObservers">The entity observers.</param>
        /// <param name="entityCacheFactory">The entity cache factory.</param>
        /// <param name="scopeManagerFactory">The scope manager factory.</param>
        public OwnableEntityRepository(
            Func<TOwnableEntity> entityFactory,
            Func<TOwnableEntityData> entityDataFactory,
            Func<TOwnableEntityRepositoryService> entityRepositoryServiceFactory,
            IEnumerable<IEntityObserver<TOwnableEntity, Guid>> entityObservers,
            Func<IEntityCache<TOwnableEntity, Guid, TOwnableEntityData, TOwnableEntityRepository>> entityCacheFactory,
            Func<IScopeManager> scopeManagerFactory) :
                base(
                entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, entityCacheFactory,
                scopeManagerFactory)
        {
        }
    }

    /// <summary>
    /// Base implementation of IOwnableEntityRepository.
    /// </summary>
    /// <typeparam name="TOwnableEntity">The type of the ownable entity.</typeparam>
    /// <typeparam name="TOwnableEntityData">The type of the ownable entity data.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    /// <typeparam name="TOwnableEntityRepository">The type of the ownable entity repository.</typeparam>
    /// <typeparam name="TOwnableEntityRepositoryService">The type of the ownable entity repository service.</typeparam>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    /// <typeparam name="TOwnerData">The type of the owner data.</typeparam>
    /// <typeparam name="TOwnerRepository">The type of the owner repository.</typeparam>
    public class OwnableEntityRepository<TOwnableEntity, TOwnableEntityData, TId, TEntityDataImplementation,
                                         TOwnableEntityRepository,
                                         TOwnableEntityRepositoryService, TOwner, TOwnerData, TOwnerRepository> :
                                             EntityRepository
                                                 <TOwnableEntity, TOwnableEntityData, TId, TEntityDataImplementation,
                                                 TOwnableEntityRepository, TOwnableEntityRepositoryService>,
                                             IOwnableEntityRepository
                                                 <TOwnableEntityRepository, TOwnableEntity, TId, TOwnableEntityData,
                                                 TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntity : class,
            IOwnableEntity
                <TOwnableEntity, TId, TOwnableEntityData, TOwner, TOwnableEntityRepository, TOwnerData, TOwnerRepository
                >, TOwnableEntityData
        where TOwnableEntityData : class,
            IOwnableEntityData
                <TOwnableEntityData, TOwnableEntity, TId, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository
                >
        where TEntityDataImplementation : class, TOwnableEntityData
        where TOwnableEntityRepository : class,
            IOwnableEntityRepository
                <TOwnableEntityRepository, TOwnableEntity, TId, TOwnableEntityData, TOwner, TOwnerData, TOwnerRepository
                >
        where TOwnableEntityRepositoryService : class,
            IOwnableEntityRepositoryService
                <TOwnableEntityData, TOwnableEntity, TOwnableEntityRepository, TId, TEntityDataImplementation, TOwner,
                TOwnerData, TOwnerRepository>
        where TOwnerData : class, IEntityData<TOwnerData, TOwner, TId, TOwnerRepository>
        where TOwner : class, IEntity<TOwner, TId, TOwnerData, TOwnerRepository>, TOwnerData
        where TOwnerRepository : class, IEntityRepository<TOwnerRepository, TOwner, TId, TOwnerData>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityRepositoryServiceFactory">The entity repository service factory.</param>
        /// <param name="entityObservers">The entity observers.</param>
        /// <param name="entityCacheFactory">The entity cache factory.</param>
        /// <param name="scopeManagerFactory">The scope manager factory.</param>
        public OwnableEntityRepository(
            Func<TOwnableEntity> entityFactory,
            Func<TOwnableEntityData> entityDataFactory,
            Func<TOwnableEntityRepositoryService> entityRepositoryServiceFactory,
            IEnumerable<IEntityObserver<TOwnableEntity, TId>> entityObservers,
            Func<IEntityCache<TOwnableEntity, TId, TOwnableEntityData, TOwnableEntityRepository>> entityCacheFactory,
            Func<IScopeManager> scopeManagerFactory) :
                base(
                entityFactory, entityDataFactory, entityRepositoryServiceFactory, entityObservers, entityCacheFactory,
                scopeManagerFactory)
        {
        }

        #region Implementation of IOwnableEntityRepository<TOwnableEntityRepository,TOwnableEntity,in TId,TOwnableEntityData,TOwner>

        /// <summary>
        /// Gets the entities by their owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <returns></returns>
        public IEnumerable<TOwnableEntity> GetByOwner(TOwner owner, bool forceNew = false)
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNull(owner, "owner");

                // ReSharper disable ImplicitlyCapturedClosure
                Logger.Assembly.Debug(
                    m =>
                    m("Getting [{0}] by owner ID [{1}]. ForceNew [{2}].", typeof (TOwnableEntity), owner.Id, forceNew));
                // ReSharper restore ImplicitlyCapturedClosure
                var cacheKey = string.Format("GetByOwner_{0}_{1}", typeof (TOwnableEntity), owner.Id);

                return ProcessRequestForEnumerable(forceNew, cacheKey,
                                                   x => EntityRepositoryServiceFactory().GetByOwner(owner.Id, x));
            }
        }

        #endregion
    }
}