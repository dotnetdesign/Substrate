using System;
using System.Runtime.Serialization;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Base implementation of IEntityData.
    /// </summary>
    /// <typeparam name="TOwnableEntityData">The type of the ownable entity data.</typeparam>
    /// <typeparam name="TOwnableEntity">The type of the ownable entity.</typeparam>
    /// <typeparam name="TOwnableEntityRepository">The type of the ownable entity repository.</typeparam>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    /// <typeparam name="TOwnerData">The type of the owner data.</typeparam>
    /// <typeparam name="TOwnerRepository">The type of the owner repository.</typeparam>
    [Serializable]
    [DataContract]
    public abstract class BaseOwnableEntityData<TOwnableEntityData, TOwnableEntity, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository> :
        BaseOwnableEntityData<TOwnableEntityData, TOwnableEntity, Guid, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntityData : class, IOwnableEntityData<TOwnableEntityData, TOwnableEntity, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntity : class, IOwnableEntity<TOwnableEntity, TOwnableEntityData, TOwner, TOwnableEntityRepository, TOwnerData, TOwnerRepository>, TOwnableEntityData
        where TOwnableEntityRepository : class, IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity, TOwnableEntityData, TOwner, TOwnerData, TOwnerRepository>
        where TOwnerData : class,IEntityData<TOwnerData, TOwner, TOwnerRepository>
        where TOwner : class, IEntity<TOwner, TOwnerData, TOwnerRepository>, TOwnerData
        where TOwnerRepository : class, IEntityRepository<TOwnerRepository, TOwner, TOwnerData> { }

    /// <summary>
    /// Base implementation of IEntityData.
    /// </summary>
    /// <typeparam name="TOwnableEntityData">The type of the ownable entity data.</typeparam>
    /// <typeparam name="TOwnableEntity">The type of the ownable entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TOwnableEntityRepository">The type of the ownable entity repository.</typeparam>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    /// <typeparam name="TOwnerData">The type of the owner data.</typeparam>
    /// <typeparam name="TOwnerRepository">The type of the owner repository.</typeparam>
    [Serializable]
    [DataContract]
    public abstract class BaseOwnableEntityData<TOwnableEntityData, TOwnableEntity, TId, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository> :
        BaseEntityData<TOwnableEntityData, TOwnableEntity, TId, TOwnableEntityRepository>,
        IOwnableEntityData<TOwnableEntityData, TOwnableEntity, TId, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntityData : class, IOwnableEntityData<TOwnableEntityData, TOwnableEntity, TId, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntity : class, IOwnableEntity<TOwnableEntity, TId, TOwnableEntityData, TOwner, TOwnableEntityRepository, TOwnerData, TOwnerRepository>, TOwnableEntityData
        where TOwnableEntityRepository : class, IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity, TId, TOwnableEntityData, TOwner, TOwnerData, TOwnerRepository>
        where TOwnerData : class,IEntityData<TOwnerData, TOwner, TId, TOwnerRepository>
        where TOwner : class, IEntity<TOwner, TId, TOwnerData, TOwnerRepository>, TOwnerData
        where TOwnerRepository : class, IEntityRepository<TOwnerRepository, TOwner, TId, TOwnerData>
    {
        #region Implementation of IOwnableEntityData<TOwnableEntityData,out TOwnableEntity,TId,TOwnableEntityRepository,TOwner>

        /// <summary>
        /// Gets or sets the owner id.
        /// </summary>
        /// <value>
        /// The owner id.
        /// </value>
        [DataMember]
        public TId OwnerId { get; set; }

        #endregion
    }
}