using System;
using System.Collections.Generic;
using System.Threading;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Base implementation of IOwnableEntity.
    /// </summary>
    /// <typeparam name="TOwnableEntity">The type of the ownable entity.</typeparam>
    /// <typeparam name="TOwnableEntityData">The type of the ownable entity data.</typeparam>
    /// <typeparam name="TOwnableEntityRepository">The type of the ownable entity repository.</typeparam>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    /// <typeparam name="TOwnerData">The type of the owner data.</typeparam>
    /// <typeparam name="TOwnerRepository">The type of the owner repository.</typeparam>
    public abstract class BaseOwnableEntity<TOwnableEntity, TOwnableEntityData, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository> :
        BaseOwnableEntity<TOwnableEntity, Guid, TOwnableEntityData, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntityData : class, IOwnableEntityData<TOwnableEntityData, TOwnableEntity, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntity : class, IOwnableEntity<TOwnableEntity, TOwnableEntityData, TOwner, TOwnableEntityRepository, TOwnerData, TOwnerRepository>, TOwnableEntityData
        where TOwnableEntityRepository : class, IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity, TOwnableEntityData, TOwner, TOwnerData, TOwnerRepository>
        where TOwnerData : class,IEntityData<TOwnerData, TOwner, TOwnerRepository>
        where TOwner : class, IEntity<TOwner, TOwnerData, TOwnerRepository>, TOwnerData
        where TOwnerRepository : class, IEntityRepository<TOwnerRepository, TOwner, TOwnerData>
    {
        protected BaseOwnableEntity(
            Func<TOwnableEntityRepository> entityRepositoryFactory, 
            Func<TOwnableEntityData> entityDataFactory, 
            Func<IConcurrencyManager<TOwnableEntity, Guid, TOwnableEntityData, TOwnableEntityRepository>> entityConcurrencyManagerFactory, 
            IEnumerable<IEntityValidator<TOwnableEntity, TOwnableEntityData, Guid, TOwnableEntityRepository>> entityValidators, 
            Func<IPermissionAuthorizationManager<TOwnableEntity, TOwnableEntityData, Guid, TOwnableEntityRepository>> permissionAuthorizationManagerFactory, 
            Func<TOwnerRepository> ownerRepositoryFactory) : 
            base(entityRepositoryFactory, entityDataFactory, entityConcurrencyManagerFactory, entityValidators, permissionAuthorizationManagerFactory, ownerRepositoryFactory)
        {
        }
    }

    /// <summary>
    /// Base implementation of IOwnableEntity.
    /// </summary>
    /// <typeparam name="TOwnableEntity">The type of the ownable entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TOwnableEntityData">The type of the ownable entity data.</typeparam>
    /// <typeparam name="TOwnableEntityRepository">The type of the ownable entity repository.</typeparam>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    /// <typeparam name="TOwnerRepository">The type of the owner repository.</typeparam>
    /// <typeparam name="TOwnerData">The type of the owner data.</typeparam>
    public abstract class BaseOwnableEntity<TOwnableEntity, TId, TOwnableEntityData, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository> :
        BaseEntity<TOwnableEntity, TId, TOwnableEntityData, TOwnableEntityRepository>,
        IOwnableEntity<TOwnableEntity, TId, TOwnableEntityData, TOwner, TOwnableEntityRepository, TOwnerData, TOwnerRepository>
        where TOwnableEntityData : class, IOwnableEntityData<TOwnableEntityData, TOwnableEntity, TId, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntity : class, IOwnableEntity<TOwnableEntity, TId, TOwnableEntityData, TOwner, TOwnableEntityRepository, TOwnerData, TOwnerRepository>, TOwnableEntityData
        where TOwnableEntityRepository : class, IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity, TId, TOwnableEntityData, TOwner, TOwnerData, TOwnerRepository>
        where TOwnerData : class, IEntityData<TOwnerData, TOwner, TId, TOwnerRepository>
        where TOwner : class, IEntity<TOwner, TId, TOwnerData, TOwnerRepository>, TOwnerData
        where TOwnerRepository : class, IEntityRepository<TOwnerRepository, TOwner, TId, TOwnerData>
    {
        protected BaseOwnableEntity(
            Func<TOwnableEntityRepository> entityRepositoryFactory,
            Func<TOwnableEntityData> entityDataFactory,
            Func<IConcurrencyManager<TOwnableEntity, TId, TOwnableEntityData, TOwnableEntityRepository>> entityConcurrencyManagerFactory,
            IEnumerable<IEntityValidator<TOwnableEntity, TOwnableEntityData, TId, TOwnableEntityRepository>> entityValidators,
            Func<IPermissionAuthorizationManager<TOwnableEntity, TOwnableEntityData, TId, TOwnableEntityRepository>> permissionAuthorizationManagerFactory,
            Func<TOwnerRepository> ownerRepositoryFactory) :
            base(entityRepositoryFactory, entityDataFactory, entityConcurrencyManagerFactory, entityValidators, permissionAuthorizationManagerFactory)
        {
            using (Logger.Scope())
            {
                OwnerRepositoryFactory = ownerRepositoryFactory;
            }
        }

        #region Private Members

        private Lazy<TOwner> _lazyOwner; 

        #endregion Private Members

        #region Protected Members

        protected readonly Func<TOwnerRepository> OwnerRepositoryFactory; 

        #endregion Protected Members

        #region Implementation of IOwnableEntity<TOwnableEntity,TId,TEntityData,TOwner,TOwnableEntityRepository>

        /// <summary>
        /// Gets or sets the owner id.
        /// </summary>
        /// <value>
        /// The owner id.
        /// </value>
        public TId OwnerId
        {
            get
            {
                using (Logger.Scope())
                {
                    return (!IsInitialized) ? default(TId) : EntityData.OwnerId;
                }
            }
            set
            {
                using (Logger.Scope())
                {
                    if (EntityData.Id.Equals(value)) return;

                    var oldValue = EntityData.OwnerId;
                    OnPropertyChanging("OwnerId", oldValue, value);
                    EntityData.OwnerId = value;
                    OnPropertyChanged("OwnerId", oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <returns></returns>
        public TOwner GetOwner
        {
            get
            {
                using (Logger.Scope())
                {
                    ThrowIfNotInitialized();
                    if (_lazyOwner == null)
                    {
                        InitializeLazyOwner();
                    }

                    return _lazyOwner.IsValueCreated ? _lazyOwner.Value : null;
                }
            }
            set
            {
                using (Logger.Scope())
                {
                    OwnerId = value.Id;
                }
            }
        }

        #endregion

        #region Private Methods

        private void InitializeLazyOwner()
        {
            using (Logger.Scope())
            {
                _lazyOwner = new Lazy<TOwner>(LazyOwnerInitializationHandler, LazyThreadSafetyMode.ExecutionAndPublication);
            }
        }

        private TOwner LazyOwnerInitializationHandler()
        {
            using (Logger.Scope())
            {
                Logger.Debug(m => m("Getting owner for {0}", this));
                
                return OwnerRepositoryFactory().GetById(OwnerId);
            }
        }

        #endregion Private Methods
    }
}