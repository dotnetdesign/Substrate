using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        BaseEntity<TEntity, Guid, TEntityData, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TEntity, TEntityData, TEntityRepository&gt;"/> class.
        /// </summary>
        /// <param name="entityRepository">The entity repository.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityValidators">The entity validators.</param>
        protected BaseEntity(
            TEntityRepository entityRepository,
            Func<TEntityData> entityDataFactory,
            IEnumerable<IEntityValidator<TEntity, TEntityData, TEntityRepository>> entityValidators)
            : base(entityRepository, entityDataFactory, entityValidators)
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
        IEntity<TEntity, TId, TEntityData, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        #region Private Members

        private bool _init;
        private bool _isDirty;
        private bool _propertyChangedSinceIsDirtySet;
        private IEnumerable<ValidationResult> _validationResults;

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
        /// <param name="entityRepository">The entity repository.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        /// <param name="entityValidators">The entity validators.</param>
        protected BaseEntity(
            TEntityRepository entityRepository,
            Func<TEntityData> entityDataFactory,
            IEnumerable<IEntityValidator<TEntity, TEntityData, TId, TEntityRepository>> entityValidators)
        {
            EntityDataFactory = entityDataFactory;
            EntityRepository = entityRepository;
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
                if (!IsValid)
                {
                    returnedEntity = this as TEntity;
                    return false;
                }

                Version = Version + 1;
                OnSaving();
                returnedEntity = EntityRepository.Save(this as TEntity);
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
            EntityRepository.Delete(this as TEntity);
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

                OnPropertyChanging("Id");
                EntityData.Id = value;
                OnPropertyChanged("Id");
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

                OnPropertyChanging("Version");
                EntityData.Version = value;
                OnPropertyChanged("Version");
            }
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns></returns>
        public TEntity GetPreviousVersion()
        {
            return EntityRepository.GetPreviousVersion(this as TEntity);
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public TEntity GetVersion(int version)
        {
            return EntityRepository.GetVersion(this as TEntity, version);
        }

        /// <summary>
        /// Gets or sets the entity repository.
        /// </summary>
        /// <value>
        /// The entity repository.
        /// </value>
        public TEntityRepository EntityRepository { get; set; }

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
            var propertyName = ((MemberExpression) property.Body).Member.Name;

            originalValue =
                (TProperty) typeof (TEntityData).GetProperty(propertyName).GetValue(OriginalEntityData, null);
            var currentValue = (TProperty) typeof (TEntityData).GetProperty(propertyName).GetValue(EntityData, null);

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
        public IEnumerable<ValidationResult> Validate()
        {
            return EntityValidators.SelectMany(x => x.Validate(this as TEntity));
        }

        #endregion

        #region Implementation of IObservableEntity

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging = delegate { }; 

        /// <summary>
        /// Called when a property value is changing.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanging(string propertyName)
        {
            PropertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when a property value changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { }; 

        /// <summary>
        /// Called when a property value changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when saving.
        /// </summary>
        public event EventHandler Saving = delegate { };

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
        public event EventHandler Saved = delegate { };

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
        public event EventHandler Deleting = delegate { };

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
        public event EventHandler Deleted = delegate { };

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
        public event EventHandler Initializing = delegate { };

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
        public event EventHandler Initialized = delegate { };

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
        private void BaseEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _propertyChangedSinceIsDirtySet = true;
        }

        #endregion
    }
}