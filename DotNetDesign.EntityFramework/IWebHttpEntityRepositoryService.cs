using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Required functionality every IWebHttpEntityRepositoryService must implement.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    public interface IWebHttpEntityRepositoryService<TEntityData, TEntity, TEntityRepository, TEntityDataImplementation>:
        IWebHttpEntityRepositoryService<TEntityData, TEntity, TEntityRepository, Guid, TEntityDataImplementation>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
        where TEntityDataImplementation : class, TEntityData
    {
        
    }

    /// <summary>
    /// Required functionality every IWebHttpEntityRepositoryService must implement.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityDataImplementation">The type of the entity data implementation.</typeparam>
    [ServiceContract]
    public interface IWebHttpEntityRepositoryService<TEntityData, TEntity, TEntityRepository, in TId, TEntityDataImplementation>
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
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        WebHttpEntityContainer<TEntityDataImplementation> GetById(TId id);

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(
            BodyStyle = WebMessageBodyStyle.Wrapped,
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        WebHttpEntityContainer<TEntityDataImplementation> GetVersion(TId id, int version);

        /// <summary>
        /// Gets the entity data by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        WebHttpEntityContainer<IEnumerable<TEntityDataImplementation>> GetByIds(IEnumerable<TId> ids);

        /// <summary>
        /// Gets all entity data.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(
            UriTemplate = "/",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        WebHttpEntityContainer<IEnumerable<TEntityDataImplementation>> GetAll();

        /// <summary>
        /// Saves the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [WebInvoke(
            UriTemplate = "/",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
         WebHttpEntityContainer<TEntityDataImplementation> Save(TEntityDataImplementation entityData);

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        IEnumerable<WebHttpEntityContainer<TEntityDataImplementation>> SaveAll(IEnumerable<TEntityDataImplementation> entityData);

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [WebInvoke(
            UriTemplate = "/",
            Method = "DELETE",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        WebHttpEntityContainer<object> Delete(TId id);

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="ids">The ids.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [WebInvoke(
            Method = "DELETE",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        WebHttpEntityContainer<object> DeleteAll(IEnumerable<TId> ids);
    }
}