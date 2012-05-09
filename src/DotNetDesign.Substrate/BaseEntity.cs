using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using Common.Logging;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Base implementation of IEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public abstract class BaseEntity<TEntity, TEntityData, TEntityRepository> :
        BaseEntity<TEntity, Guid, TEntityData, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity{TEntity,TEntityPermission,TEntityData,TEntityRepository}"/> class.
        /// </summary>
        /// <param name="entityRepositoryFactory">The entity repository factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityConcurrencyManagerFactory">The entity concurrency manager factory.</param>
        /// <param name="entityValidators">The entity validators.</param>
        /// <param name="permissionAuthorizationManagerFactory">The permission authorization manager factory.</param>
        protected BaseEntity(
            Func<TEntityRepository> entityRepositoryFactory,
            Func<TEntityData> entityDataFactory,
            Func<IConcurrencyManager<TEntity, TEntityData, TEntityRepository>> entityConcurrencyManagerFactory,
            IEnumerable<IEntityValidator<TEntity, TEntityData, TEntityRepository>> entityValidators,
            Func<IPermissionAuthorizationManager<TEntity, TEntityData, TEntityRepository>> permissionAuthorizationManagerFactory)
            : base(entityRepositoryFactory, entityDataFactory, entityConcurrencyManagerFactory, entityValidators, permissionAuthorizationManagerFactory)
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
        BaseLogger<BaseEntity<TEntity, TId, TEntityData, TEntityRepository>>,
        IEntity<TEntity, TId, TEntityData, TEntityRepository>, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        #region Private Members

        private bool _init;
        private bool _isDirty;
        private bool _propertyChangedSinceIsDirtySet;
        private bool _propertyChangedSinceValidationResultsPopulated;
        private IEnumerable<ValidationResult> _validationResults;
        private TEntityData _entityData;

        #endregion

        #region Protected Members

        protected bool BypassReadPermissionCheck;
        protected bool BypassInsertPermissionCheck;
        protected bool BypassUpdatePermissionCheck;
        protected bool BypassDeletePermissionCheck;

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

        /// <summary>
        /// Gets or sets the permission authorization manager factory.
        /// </summary>
        /// <value>
        /// The permission authorization manager factory.
        /// </value>
        protected Func<IPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository>> PermissionAuthorizationManagerFactory { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TEntity, TEntityPermission, TId, TEntityData, TEntityRepository&gt;"/> class.
        /// </summary>
        /// <param name="entityRepositoryFactory">The entity repository factory.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityConcurrencyManagerFactory">The entity concurrency manager factory.</param>
        /// <param name="entityValidators">The entity validators.</param>
        /// <param name="permissionAuthorizationManagerFactory">The permission authorization manager factory.</param>
        protected BaseEntity(
            Func<TEntityRepository> entityRepositoryFactory,
            Func<TEntityData> entityDataFactory,
            Func<IConcurrencyManager<TEntity, TId, TEntityData, TEntityRepository>> entityConcurrencyManagerFactory,
            IEnumerable<IEntityValidator<TEntity, TEntityData, TId, TEntityRepository>> entityValidators,
            Func<IPermissionAuthorizationManager<TEntity, TEntityData, TId, TEntityRepository>> permissionAuthorizationManagerFactory)
        {
            using (Logger.Scope())
            {
                EntityDataFactory = entityDataFactory;
                EntityRepositoryFactory = entityRepositoryFactory;
                EntityConcurrencyManagerFactory = entityConcurrencyManagerFactory;
                EntityValidators = entityValidators;
                PermissionAuthorizationManagerFactory = permissionAuthorizationManagerFactory;

                PropertyChanged += BaseEntityPropertyChanged;
            }
        }

        #endregion

        #region IEntity<TEntity,TId,TEntityData,TEntityRepository> Members

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public TEntityData Clone()
        {
            using (Logger.Scope())
            {
                var newEntity = EntityRepositoryFactory().GetNew();
                newEntity.Initialize(OriginalEntityData);
                return newEntity;
            }
        }

        /// <summary>
        /// Saves this instance. Returns if the save process was successful.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            using (Logger.Scope())
            {
                TEntity returnedEntity;
                return Save(out returnedEntity);
            }
        }

        /// <summary>
        /// Saves this instance. Returns if the save process was successful.
        /// </summary>
        /// <returns></returns>
        public bool Save(out TEntity returnedEntity)
        {
            using (Logger.Scope())
            {
                if (IsDirty)
                {
                    Logger.DebugFormat("Entity [{0}] is dirty.", this);
                    if (!IsValid && Validate().Any(x => x.StatusType == ValidationResultStatusType.Error))
                    {
                        Logger.DebugFormat("Entity [{0}] is not valid.", this);
                        returnedEntity = this as TEntity;
                        return false;
                    }
                    
                    if (Logger.IsInfoEnabled)
                    {
                        Logger.DebugFormat("Entity [{0}] is valid.", this);
                    }

                    EntityConcurrencyManagerFactory().Verify(this as TEntity);

                    Version++;
                    if (Version == 1)
                    {
                        CreatedAt = DateTime.Now;

                        // Inserting, verify permissions
                        if (!BypassInsertPermissionCheck)
                        {
                            PermissionAuthorizationManagerFactory().Authorize(EntityPermissions.Insert);
                        }
                    }
                    else
                    {
                        // Updating, verify permissions
                        if (!BypassUpdatePermissionCheck)
                        {
                            PermissionAuthorizationManagerFactory().Authorize(EntityPermissions.Update);
                        }
                    }
                    LastUpdatedAt = DateTime.Now;

                    OnSaving();
                    returnedEntity = EntityRepositoryFactory().Save(this as TEntity);

                    // reset entity state now that it has been saved.
                    _init = false;
                    Initialize(returnedEntity.EntityData);

                    OnSaved();
                    
                    return true;
                }
                
                if(Logger.IsInfoEnabled)
                {
                    Logger.DebugFormat("Entity [{0}] is not dirty.", this);
                }

                returnedEntity = this as TEntity;
                return true;
            }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Delete()
        {
            using (Logger.Scope())
            {
                // Deleting, verify permissions
                if (!BypassDeletePermissionCheck)
                {
                    PermissionAuthorizationManagerFactory().Authorize(EntityPermissions.Delete);
                }

                OnDeleting();
                EntityRepositoryFactory().Delete(this as TEntity);
                OnDeleted();
            }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public TId Id
        {
            get
            {
                using (Logger.Scope())
                {
                    return (!_init) ? default(TId) : EntityData.Id;
                }
            }
            set
            {
                using (Logger.Scope())
                {
                    if (EntityData.Id.Equals(value)) return;

                    var oldValue = EntityData.Id;
                    OnPropertyChanging("Id", oldValue, value);
                    EntityData.Id = value;
                    OnPropertyChanged("Id", oldValue, value);
                }
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
            get
            {
                using (Logger.Scope())
                {
                    return (!_init) ? default(int) : EntityData.Version;
                }
            }
            set
            {
                using (Logger.Scope())
                {
                    if (EntityData.Version.Equals(value)) return;

                    var oldValue = EntityData.Version;
                    OnPropertyChanging("Version", oldValue, value);
                    EntityData.Version = value;
                    OnPropertyChanged("Version", oldValue, value);
                }
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
            get
            {
                using (Logger.Scope())
                {
                    return (!_init) ? DateTime.MinValue : EntityData.CreatedAt;
                }
            }
            set
            {
                using (Logger.Scope())
                {
                    if (EntityData.CreatedAt.Equals(value)) return;

                    var oldValue = EntityData.Version;
                    OnPropertyChanging("CreatedAt", oldValue, value);
                    EntityData.CreatedAt = value;
                    OnPropertyChanged("CreatedAt", oldValue, value);
                }
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
            get
            {
                using (Logger.Scope())
                {
                    return (!_init) ? DateTime.MinValue : EntityData.LastUpdatedAt;
                }
            }
            set
            {
                using (Logger.Scope())
                {
                    if (EntityData.LastUpdatedAt.Equals(value)) return;

                    var oldValue = EntityData.LastUpdatedAt;
                    OnPropertyChanging("LastUpdatedAt", oldValue, value);
                    EntityData.LastUpdatedAt = value;
                    OnPropertyChanged("LastUpdatedAt", oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets the version id.
        /// </summary>
        public string VersionId
        {
            get
            {
                using (Logger.Scope())
                {
                    return (!_init) ? "NOT INITIALIZED" : EntityData.VersionId;
                }
            }
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns></returns>
        public TEntity GetPreviousVersion()
        {
            using (Logger.Scope())
            {
                ThrowIfNotInitialized();

                return EntityRepositoryFactory().GetVersion(Id, Version - 1);
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public TEntity GetVersion(int version)
        {
            using (Logger.Scope())
            {
                ThrowIfNotInitialized();

                return EntityRepositoryFactory().GetVersion(Id, version);
            }
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
        public TEntityData EntityData
        {
            get
            {
                using (Logger.Scope())
                {
                    ThrowIfNotInitialized();
                    return _entityData;
                }
            }
            private set
            {
                using (Logger.Scope())
                {
                    _entityData = value;
                }
            }
        }

        /// <summary>
        /// Determines whether the specified property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///   <c>true</c> if the specified property has changed; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasPropertyChanged(string propertyName)
        {
            using (Logger.Scope())
            {
                ThrowIfNotInitialized();

                object originalValue;
                return HasPropertyChanged(propertyName, out originalValue);
            }
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
            using (Logger.Scope())
            {
                ThrowIfNotInitialized();

                originalValue = typeof(TEntityData).GetProperty(propertyName).GetValue(OriginalEntityData, null);
                var currentValue = typeof(TEntityData).GetProperty(propertyName).GetValue(EntityData, null);

                return HasPropertyChanged(propertyName, originalValue, currentValue);
            }
        }

        private bool HasPropertyChanged(string propertyName, object originalValue, object currentValue)
        {
            bool hasPropertyChanged;

            if (originalValue == null && currentValue == null)
            {
                hasPropertyChanged = false;
            }
            else
            {
                hasPropertyChanged = originalValue == null || !originalValue.Equals(currentValue);
            }

            if (Logger.IsInfoEnabled)
            {
                Logger.DebugFormat("Property [{0}] has changed [{1}]. Current value [{2}]. Original value [{3}].", propertyName,
                                   hasPropertyChanged, currentValue, originalValue);
            }

            return hasPropertyChanged;
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
            using (Logger.Scope())
            {
                ThrowIfNotInitialized();

                TProperty originalValue;
                return HasPropertyChanged(property, out originalValue);
            }
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
            using (Logger.Scope())
            {
                ThrowIfNotInitialized();

                var propertyName = ((MemberExpression)property.Body).Member.Name;

                originalValue =
                    (TProperty)typeof(TEntityData).GetProperty(propertyName).GetValue(OriginalEntityData, null);
                var currentValue = (TProperty)typeof(TEntityData).GetProperty(propertyName).GetValue(EntityData, null);

                return HasPropertyChanged(propertyName, originalValue, currentValue);
            }
        }

        /// <summary>
        /// Initializes the specified entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        public void Initialize(TEntityData entityData)
        {
            using (Logger.Scope())
            {
                if (_init)
                {
                    var invalidOperationException = new InvalidOperationException(
                        "Initialize has already been called on this instance. This can only be called once per instance.");
                    Logger.Error(invalidOperationException.Message);
                    throw invalidOperationException;
                }

                OnInitializing();
                
                // Initializing, verify read permissions
                if (!BypassReadPermissionCheck)
                {
                    PermissionAuthorizationManagerFactory().Authorize(EntityPermissions.Read);
                }

                OriginalEntityData = entityData;
                EntityData = entityData.Clone();

                _propertyChangedSinceIsDirtySet = false;
                _isDirty = false;
                _validationResults = null;
                _init = true;

                OnInitialized();
            }
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
                using (Logger.Scope())
                {
                    ThrowIfNotInitialized();

                    if (_propertyChangedSinceIsDirtySet)
                    {
                        _propertyChangedSinceIsDirtySet = false;
                        _isDirty =
                            typeof(TEntityData).GetProperties().Any(
                                x =>
                                x.GetValue(OriginalEntityData, null) != x.GetValue(EntityData, null));
                    }

                    return _isDirty;
                }
            }
        }

        /// <summary>
        /// Reverts the changes.
        /// </summary>
        public virtual void RevertChanges()
        {
            using (Logger.Scope())
            {
                ThrowIfNotInitialized();

                _init = false;
                Initialize(OriginalEntityData);
            }
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
                using (Logger.Scope())
                {
                    ThrowIfNotInitialized();

                    if (_validationResults == null)
                    {
                        _validationResults = Validate();
                    }

                    return !_validationResults.Any();
                }
            }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate()
        {
            using (Logger.Scope())
            {
                ThrowIfNotInitialized();

                if (_validationResults == null || _propertyChangedSinceValidationResultsPopulated)
                {
                    _propertyChangedSinceValidationResultsPopulated = false;
                    _validationResults = EntityValidators.SelectMany(x => x.Validate(this as TEntity));
                }

                return _validationResults;
            }
        }

        /// <summary>
        /// Get's the properties display name from DataAnnotation attribute if it exists.
        /// </summary>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="property">Expression for property</param>
        /// <returns></returns>
        public string GetPropertyDisplayName<TProperty>(Expression<Func<TEntityData, TProperty>> property)
        {
            using (Logger.Scope())
            {
                var propertyName = ((MemberExpression)property.Body).Member.Name;

                return GetPropertyDisplayName(propertyName);
            }
        }

        /// <summary>
        /// Get's the properties display name from DataAnnotation attribute if it exists.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <returns></returns>
        public string GetPropertyDisplayName(string propertyName)
        {
            using (Logger.Scope())
            {
                var propertyInfo = typeof(TEntityData).GetProperty(propertyName);

                var displayNameAttribute =
                    ((DisplayNameAttribute)
                     propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault());

                return (displayNameAttribute == null) ? string.Empty : displayNameAttribute.DisplayName;
            }
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
            using (Logger.Scope())
            {
                PropertyChanging.Invoke(this, new PropertyChangeEventArgs(propertyName, oldValue, newValue));
            }
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
            using (Logger.Scope())
            {
                PropertyChanged.Invoke(this, new PropertyChangeEventArgs(propertyName, oldValue, newValue));
            }
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
            using (Logger.Scope())
            {
                Saving.Invoke(this, EventArgs.Empty);
            }
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
            using (Logger.Scope())
            {
                Saved.Invoke(this, EventArgs.Empty);
            }
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
            using (Logger.Scope())
            {
                Deleting.Invoke(this, EventArgs.Empty);
            }
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
            using (Logger.Scope())
            {
                Deleted.Invoke(this, EventArgs.Empty);
            }
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
            using (Logger.Scope())
            {
                Initializing.Invoke(this, EventArgs.Empty);
            }
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
            using (Logger.Scope())
            {
                Initialized.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Bases the entity property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void BaseEntityPropertyChanged(object sender, PropertyChangeEventArgs e)
        {
            using (Logger.Scope())
            {
                _propertyChangedSinceIsDirtySet = true;
                _propertyChangedSinceValidationResultsPopulated = true;
            }
        }

        /// <summary>
        /// Checks if the entity has been initialized. If not, will throw an InvalidOperationException.
        /// </summary>
        /// <exception cref="InvalidOperationException">InvalidOperationException if entity hasn't been initialized.</exception>
        protected void ThrowIfNotInitialized()
        {
            using(Logger.Scope())
            {
                if(!_init)
                {
                    var exception =
                        new InvalidOperationException(string.Format("Entity {0} has not been initialized.", this));
                    Logger.Error(exception.Message);
                    throw exception;
                }
            }
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
            using (Logger.Scope())
            {
                return string.Format("{0}::{1}::{2}", typeof(TEntity), Id, Version);
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            using (Logger.Scope())
            {
                return ToString().GetHashCode();
            }
        }
    }
}