using System;
using System.Runtime.Serialization;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Base implementation of IEntityData.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    [Serializable]
    [DataContract]
    public abstract class BaseEntityData<TEntityData, TEntity, TEntityRepository> : BaseEntityData<TEntityData, TEntity, Guid, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntityData&lt;TEntityData, TEntity, TEntityRepository&gt;"/> class.
        /// </summary>
        protected BaseEntityData()
        {
            using (Logger.Scope())
            {
                Id = Guid.NewGuid();
            }
        }
    }

    /// <summary>
    /// Base implementation of IEntityData.
    /// </summary>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    [DataContract]
    [Serializable]
    public abstract class BaseEntityData<TEntityData, TEntity, TId, TEntityRepository> :
        BaseLogger<BaseEntityData<TEntityData, TEntity, TId, TEntityRepository>>,
        IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntityData&lt;TEntityData, TEntity, TId, TEntityRepository&gt;"/> class.
        /// </summary>
        protected BaseEntityData()
        {
            using (Logger.Scope())
            {
                Version = 0;
            }
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DataMember]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        [DataMember]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last updated at.
        /// </summary>
        /// <value>
        /// The last updated at.
        /// </value>
        [DataMember]
        public DateTime LastUpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DataMember]
        public TId Id { get; set; }

        /// <summary>
        /// Gets the version id.
        /// </summary>
        public string VersionId
        {
            get
            {
                using (Logger.Scope())
                { 
                    return Id + "::" + Version; 
                }
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public TEntityData Clone()
        {
            using (Logger.Scope())
            {
                return ObjectCopier.Clone(this) as TEntityData;
            }
        }
    }
}
