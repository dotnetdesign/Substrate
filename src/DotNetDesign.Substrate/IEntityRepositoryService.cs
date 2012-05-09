using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Defines methods for entity repository services.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    public interface IEntityRepositoryService<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation>
        : IEntityRepositoryService<TEntityData, TEntity, TEntityRepository, Guid, TEntityDataImplementation>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    { }

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
        /// <param name="scopeContext">The scope context.</param>
        /// <returns></returns>
        [OperationContract] 
        TEntityDataImplementation GetById(TId id, Dictionary<string, string> scopeContext);

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <param name="scopeContext">The scope context.</param>
        /// <returns></returns>
        [OperationContract]
        TEntityDataImplementation GetVersion(TId id, int version, Dictionary<string, string> scopeContext);

        /// <summary>
        /// Gets the entity data by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="scopeContext">The scope context.</param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<TEntityDataImplementation> GetByIds(IEnumerable<TId> ids, Dictionary<string, string> scopeContext);

        /// <summary>
        /// Gets all entity data.
        /// </summary>
        /// <param name="scopeContext">The scope context.</param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<TEntityDataImplementation> GetAll(Dictionary<string, string> scopeContext);

        /// <summary>
        /// Saves the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <param name="scopeContext">The scope context.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        TEntityDataImplementation Save(TEntityDataImplementation entityData, Dictionary<string, string> scopeContext);

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <param name="scopeContext">The scope context.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IEnumerable<TEntityDataImplementation> SaveAll(IEnumerable<TEntityDataImplementation> entityData, Dictionary<string, string> scopeContext);

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="scopeContext">The scope context.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Delete(TId id, Dictionary<string, string> scopeContext);

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="scopeContext">The scope context.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteAll(IEnumerable<TId> ids, Dictionary<string, string> scopeContext);
    }
}