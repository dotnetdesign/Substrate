using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
        IEntity<TEntity, TId, TEntityData, TEntityRepository>, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        #region Private Members

        private bool _isDirty;
        private bool _propertyChangedSinceIsDirtySet;
        private bool _propertyChangedSinceValidationResultsPopulated;
        private IEnumerable<ValidationResult> _validationResults;
        private TEntityData _entityData;

        #endregion

        #region Protected Members

        /// <summary>
        /// The is initialized
        /// </summary>
        protected bool IsInitialized;
        /// <summary>
        /// The bypass read permission check
        /// </summary>
        protected bool BypassReadPermissionCheck;
        /// <summary>
        /// The bypass insert permission check
        /// </summary>
        protected bool BypassInsertPermissionCheck;
        /// <summary>
        /// The bypass update permission check
        /// </summary>
        protected bool BypassUpdatePermissionCheck;
        /// <summary>
        /// The bypass delete permission check
        /// </summary>
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
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TEntity, TId, TEntityData, TEntityRepository&gt;"/> class.
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
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNull(entityDataFactory, "entityDataFactory");
                Guard.ArgumentNotNull(entityRepositoryFactory, "entityRepositoryFactory");
                Guard.ArgumentNotNull(entityConcurrencyManagerFactory, "entityConcurrencyManagerFactory");
// ReSharper disable PossibleMultipleEnumeration
                Guard.ArgumentNotNull(entityValidators, "entityValidators");
// ReSharper restore PossibleMultipleEnumeration
                Guard.ArgumentNotNull(permissionAuthorizationManagerFactory, "permissionAuthorizationManagerFactory");

                EntityDataFactory = entityDataFactory;
                EntityRepositoryFactory = entityRepositoryFactory;
                EntityConcurrencyManagerFactory = entityConcurrencyManagerFactory;
// ReSharper disable PossibleMultipleEnumeration
                EntityValidators = entityValidators;
// ReSharper restore PossibleMultipleEnumeration
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
            {
                if (IsNew || IsDirty)
                {
                    Logger.Assembly.Debug(m => m("Entity [{0}] is dirty.", this));
                    if (!IsValid && Validate().Any(x => x.StatusType == ValidationResultStatusType.Error))
                    {
                        Logger.Assembly.Debug(m => m("Entity [{0}] is not valid.", this));
                        returnedEntity = this as TEntity;
                        return false;
                    }

                    Logger.Assembly.Info(m => m("Entity [{0}] is valid.", this));

                    EntityConcurrencyManagerFactory().Verify(this as TEntity);

                    if (IsNew)
                    {
                        CreatedAt = DateTime.Now;

                        // Inserting, verify permissions
                        if (!BypassInsertPermissionCheck)
                        {
                            PermissionAuthorizationManagerFactory().Authorize(EntityPermissions.Insert, this as TEntity);
                        }
                    }
                    else
                    {
                        // Updating, verify permissions
                        if (!BypassUpdatePermissionCheck)
                        {
                            PermissionAuthorizationManagerFactory().Authorize(EntityPermissions.Update, this as TEntity);
                        }
                    }

                    Version++; //Should not increment version until after authrozation checks occur.
                    LastUpdatedAt = DateTime.Now;

                    OnSaving();
                    returnedEntity = EntityRepositoryFactory().Save(this as TEntity);

                    // reset entity state now that it has been saved.
                    IsInitialized = false;
                    Initialize(returnedEntity.EntityData);

                    OnSaved();
                    
                    return true;
                }

                Logger.Assembly.Info(m => m("Entity [{0}] is not dirty.", this));

                returnedEntity = this as TEntity;
                return true;
            }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Delete()
        {
            using (Logger.Assembly.Scope())
            {
                // Deleting, verify permissions
                if (!BypassDeletePermissionCheck)
                {
                    PermissionAuthorizationManagerFactory().Authorize(EntityPermissions.Delete, this as TEntity);
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
                using (Logger.Assembly.Scope())
                {
                    return (!IsInitialized) ? default(TId) : EntityData.Id;
                }
            }
            set
            {
                using (Logger.Assembly.Scope())
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
                using (Logger.Assembly.Scope())
                {
                    return (!IsInitialized) ? default(int) : EntityData.Version;
                }
            }
            set
            {
                using (Logger.Assembly.Scope())
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
                using (Logger.Assembly.Scope())
                {
                    return (!IsInitialized) ? DateTime.MinValue : EntityData.CreatedAt;
                }
            }
            set
            {
                using (Logger.Assembly.Scope())
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
                using (Logger.Assembly.Scope())
                {
                    return (!IsInitialized) ? DateTime.MinValue : EntityData.LastUpdatedAt;
                }
            }
            set
            {
                using (Logger.Assembly.Scope())
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
                using (Logger.Assembly.Scope())
                {
                    return (!IsInitialized) ? "NOT INITIALIZED" : EntityData.VersionId;
                }
            }
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns></returns>
        public TEntity GetPreviousVersion()
        {
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
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
                using (Logger.Assembly.Scope())
                {
                    ThrowIfNotInitialized();
                    return _entityData;
                }
            }
            private set
            {
                using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
            {
                ThrowIfNotInitialized();

                Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");

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
            using (Logger.Assembly.Scope())
            {
                ThrowIfNotInitialized();

                Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");

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

            Logger.Assembly.Info(m => m("Property [{0}] has changed [{1}]. Current value [{2}]. Original value [{3}].", propertyName,
                               hasPropertyChanged, currentValue, originalValue));

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
            using (Logger.Assembly.Scope())
            {
                ThrowIfNotInitialized();

                Guard.ArgumentNotNull(property, "property");

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
            using (Logger.Assembly.Scope())
            {
                ThrowIfNotInitialized();

                Guard.ArgumentNotNull(property, "property");

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
            using (Logger.Assembly.Scope())
            {
                if (IsInitialized)
                {
                    var invalidOperationException = new InvalidOperationException(
                        "Initialize has already been called on this instance. This can only be called once per instance.");
                    Logger.Assembly.Error(invalidOperationException.Message, invalidOperationException);
                    throw invalidOperationException;
                }


                Guard.ArgumentNotNull(entityData, "entityData");

                OnInitializing();
                
                // Initializing, verify read permissions
                if (!BypassReadPermissionCheck)
                {
                    PermissionAuthorizationManagerFactory().Authorize(EntityPermissions.Read, this as TEntity);
                }

                OriginalEntityData = entityData;
                EntityData = entityData.Clone();

                _propertyChangedSinceIsDirtySet = false;
                _isDirty = false;
                _validationResults = null;
                IsInitialized = true;

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
                using (Logger.Assembly.Scope())
                {
                    ThrowIfNotInitialized();

                    if (_propertyChangedSinceIsDirtySet)
                    {
                        _propertyChangedSinceIsDirtySet = false;
                        _isDirty = typeof(TEntityData)
                            .GetProperties()
                            .Any(x => x.GetValue(OriginalEntityData, null) != x.GetValue(EntityData, null));
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
            using (Logger.Assembly.Scope())
            {
                ThrowIfNotInitialized();

                IsInitialized = false;
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
                using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNull(property, "property");

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
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");

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
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");

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
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");

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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
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
            using(Logger.Assembly.Scope())
            {
                if (IsInitialized) return;

                var exception = new InvalidOperationException(string.Format("Entity {0} has not been initialized.", this));
                Logger.Assembly.Error(exception.Message, exception);
                throw exception;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool IsNew
        {
            get
            {
                using (Logger.Assembly.Scope())
                {
                    return Version == 0;
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
            using (Logger.Assembly.Scope())
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
            using (Logger.Assembly.Scope())
            {
                return ToString().GetHashCode();
            }
        }
    }
}