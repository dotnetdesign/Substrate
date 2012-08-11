using System;
using System.Collections.Generic;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Defines methods for storing and retrieving ownable entities.
    /// </summary>
    /// <typeparam name="TOwnableEntityRepository">The type of the ownable entity repository.</typeparam>
    /// <typeparam name="TOwnableEntity">The type of the ownable entity.</typeparam>
    /// <typeparam name="TOwnableEntityData">The type of the ownable entity data.</typeparam>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    /// <typeparam name="TOwnerData">The type of the owner data.</typeparam>
    /// <typeparam name="TOwnerRepository">The type of the owner repository.</typeparam>
    public interface IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity, TOwnableEntityData, in TOwner, TOwnerData, TOwnerRepository> :
        IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity, Guid, TOwnableEntityData, TOwner, TOwnerData, TOwnerRepository>,
        IEntityRepository<TOwnableEntityRepository, TOwnableEntity, TOwnableEntityData>
        where TOwnableEntityData : class, IOwnableEntityData<TOwnableEntityData, TOwnableEntity, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntity : class, IOwnableEntity<TOwnableEntity, TOwnableEntityData, TOwner, TOwnableEntityRepository, TOwnerData, TOwnerRepository>, TOwnableEntityData
        where TOwnableEntityRepository : class, IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity, TOwnableEntityData, TOwner, TOwnerData, TOwnerRepository>
        where TOwnerData : class,IEntityData<TOwnerData, TOwner, TOwnerRepository>
        where TOwner : class, IEntity<TOwner, TOwnerData, TOwnerRepository>, TOwnerData
        where TOwnerRepository : class, IEntityRepository<TOwnerRepository, TOwner, TOwnerData>
    {
    }

    /// <summary>
    /// Defines methods for storing and retrieving ownable entities.
    /// </summary>
    /// <typeparam name="TOwnableEntityRepository">The type of the ownable entity repository.</typeparam>
    /// <typeparam name="TOwnableEntity">The type of the ownable entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TOwnableEntityData">The type of the ownable entity data.</typeparam>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    /// <typeparam name="TOwnerData">The type of the owner data.</typeparam>
    /// <typeparam name="TOwnerRepository">The type of the owner repository.</typeparam>
    public interface IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity, in TId, TOwnableEntityData, in TOwner, TOwnerData, TOwnerRepository> :
        IEntityRepository<TOwnableEntityRepository, TOwnableEntity, TId, TOwnableEntityData>
        where TOwnableEntityData : class, IOwnableEntityData<TOwnableEntityData, TOwnableEntity, TId, TOwnableEntityRepository, TOwner, TOwnerData, TOwnerRepository>
        where TOwnableEntity : class, IOwnableEntity<TOwnableEntity, TId, TOwnableEntityData, TOwner, TOwnableEntityRepository, TOwnerData, TOwnerRepository>, TOwnableEntityData
        where TOwnableEntityRepository : class, IOwnableEntityRepository<TOwnableEntityRepository, TOwnableEntity, TId, TOwnableEntityData, TOwner, TOwnerData, TOwnerRepository>
        where TOwnerData : class,IEntityData<TOwnerData, TOwner, TId, TOwnerRepository>
        where TOwner : class, IEntity<TOwner, TId, TOwnerData, TOwnerRepository>, TOwnerData
        where TOwnerRepository : class, IEntityRepository<TOwnerRepository, TOwner, TId, TOwnerData>
    {
        /// <summary>
        /// Gets the entities by their owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="forceNew">if set to <c>true</c> [force new].</param>
        /// <returns></returns>
        IEnumerable<TOwnableEntity> GetByOwner(TOwner owner, bool forceNew = false);
    }
}