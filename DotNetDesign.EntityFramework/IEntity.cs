using System;
using System.Linq.Expressions;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines method and properties for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public interface IEntity<out TEntity, TEntityData, TEntityRepository> :
        IEntity<TEntity, Guid, TEntityData, TEntityRepository>, IObservableEntity
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
    }

    /// <summary>
    /// Defines method and properties for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public interface IEntity<out TEntity, TId, TEntityData, TEntityRepository> 
        : IVersionable<TEntity>, IObservableEntity<TId>, IValidatable
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the entity repository.
        /// </summary>
        /// <value>
        /// The entity repository.
        /// </value>
        TEntityRepository EntityRepository { get; set; }

        /// <summary>
        /// Gets the entity data.
        /// </summary>
        TEntityData EntityData { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        bool IsDirty { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance with the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        void Initialize(TEntityData entityData);

        /// <summary>
        /// Determines whether the specified property has changed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>
        ///   <c>true</c> if the specified property has changed; otherwise, <c>false</c>.
        /// </returns>
        bool HasPropertyChanged<TProperty>(Expression<Func<TEntityData, TProperty>> property);

        /// <summary>
        /// Determines whether the specified property has changed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="originalValue">The original value.</param>
        /// <returns>
        ///   <c>true</c> if the specified property has changed; otherwise, <c>false</c>.
        /// </returns>
        bool HasPropertyChanged<TProperty>(
            Expression<Func<TEntityData, TProperty>> property,
            out TProperty originalValue);

        /// <summary>
        /// Reverts the changes.
        /// </summary>
        void RevertChanges();

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        TEntity Save();

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        void Delete();

        #endregion
    }
}