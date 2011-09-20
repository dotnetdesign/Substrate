using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Base implementation of IEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public abstract class BaseEntity<TEntity, TEntityData, TEntityRepository> :
        BaseEntity<TEntity, EntityIdentifier, TEntityData, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TEntity, TEntityData, TEntityRepository&gt;"/> class.
        /// </summary>
        /// <param name="entityRepositoryFactory">The entity repository factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityConcurrencyManagerFactory">The entity concurrency manager factory.</param>
        /// <param name="entityValidators">The entity validators.</param>
        protected BaseEntity(
            Func<TEntityRepository> entityRepositoryFactory,
            Func<TEntityData> entityDataFactory,
            Func<IConcurrencyManager<TEntity, TEntityData, TEntityRepository>> entityConcurrencyManagerFactory,
            IEnumerable<IEntityValidator<TEntity, TEntityData, TEntityRepository>> entityValidators)
            : base(entityRepositoryFactory, entityDataFactory, entityConcurrencyManagerFactory, entityValidators)
        {
        }
    }

    /// <summary>
    /// Base implementation of IEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public abstract class BaseEntity<TEntity, TId, TEntityData, TEntityRepository> :
        IEntity<TEntity, TId, TEntityData, TEntityRepository>, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        #region Private Members

        private bool _init;
        private bool _isDirty;
        private bool _propertyChangedSinceIsDirtySet;
        private IEnumerable<IValidationResult> _validationResults;

        #endregion

        #region Protected Members

        /// <summary>
        /// The entity data factory.
        /// </summary>
        protected readonly Func<TEntityData> EntityDataFactory;

        /// <summary>
        /// The entity validators.
        /// </summary>
        protected readonly IEnumerable<IEntityValidator<TEntity, TEntityData, TId, TEntityRepository>> EntityValidators;

        /// <summary>
        /// Gets or sets the original entity data.
        /// </summary>
        /// <value>
        /// The original entity data.
        /// </value>
        protected TEntityData OriginalEntityData { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TEntity, TId, TEntityData, TEntityRepository&gt;"/> class.
        /// </summary>
        /// <param name="entityRepositoryFactory">The entity repository factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityConcurrencyManagerFactory">The concurrency manager factory.</param>
        /// <param name="entityValidators">The entity validators.</param>
        protected BaseEntity(
            Func<TEntityRepository> entityRepositoryFactory,
            Func<TEntityData> entityDataFactory,
            Func<IConcurrencyManager<TEntity, TId, TEntityData, TEntityRepository>> entityConcurrencyManagerFactory,
            IEnumerable<IEntityValidator<TEntity, TEntityData, TId, TEntityRepository>> entityValidators)
        {
            EntityDataFactory = entityDataFactory;
            EntityRepositoryFactory = entityRepositoryFactory;
            EntityConcurrencyManagerFactory = entityConcurrencyManagerFactory;
            EntityValidators = entityValidators;

            PropertyChanged += BaseEntityPropertyChanged;
        }

        #endregion

        #region IEntity<TEntity,TId,TEntityData,TEntityRepository> Members

        /// <summary>
        /// Saves this instance. Returns if the save process was successful.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            TEntity returnedEntity;
            return Save(out returnedEntity);
        }

        /// <summary>
        /// Saves this instance. Returns if the save process was successful.
        /// </summary>
        /// <returns></returns>
        public bool Save(out TEntity returnedEntity)
        {
            if (IsDirty)
            {
                if (!IsValid && Validate().Any(x => x.StatusType == ValidationResultStatusType.Error))
                {
                    returnedEntity = this as TEntity;
                    return false;
                }

                EntityConcurrencyManagerFactory().Verify(this as TEntity);

                Version = Version + 1;
                if (Version == 1)
                {
                    CreatedAt = DateTime.Now;
                }
                LastUpdatedAt = DateTime.Now;

                OnSaving();
                returnedEntity = EntityRepositoryFactory().Save(this as TEntity);
                OnSaved();
                return true;
            }

            returnedEntity = this as TEntity;
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Delete()
        {
            OnDeleting();
            EntityRepositoryFactory().Delete(this as TEntity);
            OnDeleted();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public TId Id
        {
            get { return (EntityData == null) ? default(TId) : EntityData.Id; }
            set
            {
                if (EntityData.Id.Equals(value)) return;

                var oldValue = EntityData.Id;
                OnPropertyChanging("Id", oldValue, value);
                EntityData.Id = value;
                OnPropertyChanged("Id", oldValue, value);
            }
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version
        {
            get { return (EntityData == null) ? default(int) : EntityData.Version; }
            set
            {
                if (EntityData.Version.Equals(value)) return;

                var oldValue = EntityData.Version;
                OnPropertyChanging("Version", oldValue, value);
                EntityData.Version = value;
                OnPropertyChanged("Version", oldValue, value);
            }
        }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt
        {
            get { return (EntityData == null) ? DateTime.MinValue : EntityData.CreatedAt; }
            set
            {
                if (EntityData.CreatedAt.Equals(value)) return;

                var oldValue = EntityData.Version;
                OnPropertyChanging("CreatedAt", oldValue, value);
                EntityData.CreatedAt = value;
                OnPropertyChanged("CreatedAt", oldValue, value);
            }
        }

        /// <summary>
        /// Gets or sets the last updated at.
        /// </summary>
        /// <value>
        /// The last updated at.
        /// </value>
        public DateTime LastUpdatedAt
        {
            get { return (EntityData == null) ? DateTime.MinValue : EntityData.LastUpdatedAt; }
            set
            {
                if (EntityData.LastUpdatedAt.Equals(value)) return;

                var oldValue = EntityData.LastUpdatedAt;
                OnPropertyChanging("LastUpdatedAt", oldValue, value);
                EntityData.LastUpdatedAt = value;
                OnPropertyChanged("LastUpdatedAt", oldValue, value);
            }
        }

        /// <summary>
        /// Gets the version id.
        /// </summary>
        public string VersionId { get { return EntityData.VersionId; } }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns></returns>
        public TEntity GetPreviousVersion()
        {
            return EntityRepositoryFactory().GetPreviousVersion(this as TEntity);
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public TEntity GetVersion(int version)
        {
            return EntityRepositoryFactory().GetVersion(this as TEntity, version);
        }

        /// <summary>
        /// Gets or sets the entity repository factory.
        /// </summary>
        /// <value>
        /// The entity repository factory.
        /// </value>
        public Func<TEntityRepository> EntityRepositoryFactory { get; set; }

        /// <summary>
        /// Gets or sets the entity concurrency manager factory.
        /// </summary>
        /// <value>
        /// The entity concurrency manager factory.
        /// </value>
        public Func<IConcurrencyManager<TEntity, TId, TEntityData, TEntityRepository>> EntityConcurrencyManagerFactory { get; set; }

        /// <summary>
        /// Gets or sets the entity data.
        /// </summary>
        /// <value>
        /// The entity data.
        /// </value>
        public TEntityData EntityData { get; protected set; }

        /// <summary>
        /// Determines whether the specified property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///   <c>true</c> if the specified property has changed; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasPropertyChanged(string propertyName)
        {
            object originalValue;
            return HasPropertyChanged(propertyName, out originalValue);
        }

        /// <summary>
        /// Determines whether the specified property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="originalValue">The original value.</param>
        /// <returns>
        ///   <c>true</c> if the specified property has changed; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasPropertyChanged(string propertyName, out object originalValue)
        {
            originalValue = typeof (TEntityData).GetProperty(propertyName).GetValue(OriginalEntityData, null);
            var currentValue = typeof(TEntityData).GetProperty(propertyName).GetValue(EntityData, null);

            return !originalValue.Equals(currentValue);
        }

        /// <summary>
        /// Determines whether the specified property has changed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>
        ///   <c>true</c> if the specified property has changed; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasPropertyChanged<TProperty>(Expression<Func<TEntityData, TProperty>> property)
        {
            TProperty originalValue;
            return HasPropertyChanged(property, out originalValue);
        }

        /// <summary>
        /// Determines whether the specified property has changed.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="originalValue">The original value.</param>
        /// <returns>
        ///   <c>true</c> if the specified property has changed; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasPropertyChanged<TProperty>(
            Expression<Func<TEntityData, TProperty>> property,
            out TProperty originalValue)
        {
            var propertyName = ((MemberExpression)property.Body).Member.Name;

            originalValue =
                (TProperty)typeof(TEntityData).GetProperty(propertyName).GetValue(OriginalEntityData, null);
            var currentValue = (TProperty)typeof(TEntityData).GetProperty(propertyName).GetValue(EntityData, null);

            return !originalValue.Equals(currentValue);
        }

        /// <summary>
        /// Initializes the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        public void Initialize(TEntityData entityData)
        {
            if (_init)
            {
                throw new InvalidOperationException(
                    "Initialize has already been called on this instance. This can only be called once per instance.");
            }

            OnInitializing();

            OriginalEntityData = entityData;
            EntityData = CloneEntityData(entityData);

            _propertyChangedSinceIsDirtySet = false;
            _isDirty = false;
            _validationResults = null;
            _init = true;

            OnInitialized();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsDirty
        {
            get
            {
                if (_propertyChangedSinceIsDirtySet)
                {
                    _propertyChangedSinceIsDirtySet = false;
                    _isDirty =
                        typeof (TEntityData).GetProperties().Any(
                            x =>
                            x.GetValue(OriginalEntityData, null) != x.GetValue(EntityData, null));
                }

                return _isDirty;
            }
        }

        /// <summary>
        /// Reverts the changes.
        /// </summary>
        public virtual void RevertChanges()
        {
            _init = false;
            Initialize(OriginalEntityData);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get
            {
                if (_validationResults == null)
                {
                    _validationResults = Validate();
                }

                return !_validationResults.Any();
            }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IValidationResult> Validate()
        {
            if (_validationResults == null)
            {
                _validationResults = EntityValidators.SelectMany(x => x.Validate(this as TEntity));
            }

            return _validationResults;
        }

        #endregion

        #region Implementation of IObservableEntity

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event EventHandler<PropertyChangeEventArgs> PropertyChanging = delegate { };

        /// <summary>
        /// Called when a property value is changing.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPropertyChanging(string propertyName, object oldValue, object newValue)
        {
            PropertyChanging.Invoke(this, new PropertyChangeEventArgs(propertyName, oldValue, newValue));
        }

        /// <summary>
        /// Occurs when a property value changed.
        /// </summary>
        public event EventHandler<PropertyChangeEventArgs> PropertyChanged = delegate { };

        /// <summary>
        /// Called when a property value changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            PropertyChanged.Invoke(this, new PropertyChangeEventArgs(propertyName, oldValue, newValue));
        }

        /// <summary>
        /// Occurs when saving.
        /// </summary>
        public event EventHandler<EventArgs> Saving = delegate { };

        /// <summary>
        /// Called when saving.
        /// </summary>
        protected virtual void OnSaving()
        {
            Saving.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when saved.
        /// </summary>
        public event EventHandler<EventArgs> Saved = delegate { };

        /// <summary>
        /// Called when saved.
        /// </summary>
        protected virtual void OnSaved()
        {
            Saved.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when deleting.
        /// </summary>
        public event EventHandler<EventArgs> Deleting = delegate { };

        /// <summary>
        /// Called when deleting.
        /// </summary>
        protected virtual void OnDeleting()
        {
            Deleting.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when deleted.
        /// </summary>
        public event EventHandler<EventArgs> Deleted = delegate { };

        /// <summary>
        /// Called when deleted.
        /// </summary>
        protected virtual void OnDeleted()
        {
            Deleted.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when initializing.
        /// </summary>
        public event EventHandler<EventArgs> Initializing = delegate { };

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected virtual void OnInitializing()
        {
            Initializing.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when initialized.
        /// </summary>
        public event EventHandler<EventArgs> Initialized = delegate { };

        /// <summary>
        /// Called when initialized.
        /// </summary>
        protected virtual void OnInitialized()
        {
            Initialized.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Clones the entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        protected virtual TEntityData CloneEntityData(TEntityData entityData)
        {
            var newEntityData = EntityDataFactory();

            foreach (var property in typeof (TEntityData).GetProperties())
            {
                var value = property.GetValue(entityData, null);
                property.SetValue(newEntityData, value, null);
            }

            newEntityData.Id = entityData.Id;
            newEntityData.Version = entityData.Version;

            return newEntityData;
        }

        /// <summary>
        /// Bases the entity property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void BaseEntityPropertyChanged(object sender, PropertyChangeEventArgs e)
        {
            _propertyChangedSinceIsDirtySet = true;
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}::{1}::{2}", typeof (TEntity), Id, Version);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}