using System.Collections.Generic;
using System.ServiceModel;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines methods for entity repository services.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    [ServiceContract]
    public interface IEntityRepositoryService<TEntityData, TEntity, TEntityRepository, in TId, TEntityDataImplementation>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
        /// <summary>
        /// Gets the entity data by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [OperationContract]
        TEntityDataImplementation GetById(TId id);

        /// <summary>
        /// Gets the entity data by version.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        [OperationContract]
        TEntityDataImplementation GetVersion(TEntityDataImplementation entityData, int version);

        /// <summary>
        /// Gets the entity data's previous version.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        [OperationContract]
        TEntityDataImplementation GetPreviousVersion(TEntityDataImplementation entityData);

        /// <summary>
        /// Gets the entity data by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<TEntityDataImplementation> GetByIds(IEnumerable<TId> ids);

        /// <summary>
        /// Gets all entity data.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<TEntityDataImplementation> GetAll();

        /// <summary>
        /// Saves the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        TEntityDataImplementation Save(TEntityDataImplementation entityData);

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IEnumerable<TEntityDataImplementation> SaveAll(IEnumerable<TEntityDataImplementation> entityData);

        /// <summary>
        /// Deletes the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Delete(TEntityDataImplementation entityData);

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteAll(IEnumerable<TEntityDataImplementation> entityData);
    }
}