using System;
using System.Linq.Expressions;
using System.ComponentModel;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Base implementation of IEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public abstract class BaseEntity<TEntity, TEntityData, TEntityRepository> : BaseEntity<TEntity, Guid, TEntityData, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, Guid, TEntityRepository>
        where TEntity : class, IEntity<TEntity, Guid, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, Guid, TEntityData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TEntity, TEntityData, TEntityRepository&gt;"/> class.
        /// </summary>
        /// <param name="entityRepository">The entity repository.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        protected BaseEntity(TEntityRepository entityRepository, Func<TEntityData> entityDataFactory)
            :base(entityRepository, entityDataFactory)
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
    public abstract class BaseEntity<TEntity, TId, TEntityData, TEntityRepository> : IEntity<TEntity, TId, TEntityData, TEntityRepository>
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        /// <summary>
        /// The entity data factory.
        /// </summary>
        protected readonly Func<TEntityData> EntityDataFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity&lt;TEntity, TId, TEntityData, TEntityRepository&gt;"/> class.
        /// </summary>
        /// <param name="entityRepository">The entity repository.</param>
        /// <param name="entityDataFactory">The entity data factory.</param>
        protected BaseEntity(TEntityRepository entityRepository, Func<TEntityData> entityDataFactory)
        {
            EntityDataFactory = entityDataFactory;
            EntityRepository = entityRepository;

            PropertyChanged += BaseEntityPropertyChanged;
        }

        /// <summary>
        /// Gets or sets the original entity data.
        /// </summary>
        /// <value>
        /// The original entity data.
        /// </value>
        protected TEntityData OriginalEntityData { get; set; }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        public TEntity Save()
        {
            if (IsDirty)
            {
                Version = Version + 1;
                OnSaving();
                var savedEntity = EntityRepository.Save(this as TEntity);
                OnSaved();
                return savedEntity;
            }

            return this as TEntity;
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
                if (!EntityData.Id.Equals(value))
                {
                    OnPropertyChanging("Id");
                    EntityData.Id = value;
                    OnPropertyChanged("Id");
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
            get { return (EntityData == null) ? default(int) : EntityData.Version; }
            set
            {
                if (!EntityData.Version.Equals(value))
                {
                    OnPropertyChanging("Version");
                    EntityData.Version = value;
                    OnPropertyChanged("Version");
                }
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
        public virtual bool HasPropertyChanged<TProperty>(Expression<Func<TEntityData, TProperty>> property, out TProperty originalValue)
        {
            var propertyName = ((MemberExpression)property.Body).Member.Name;
            
            originalValue = (TProperty)typeof(TEntityData).GetProperty(propertyName).GetValue(OriginalEntityData, null);
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
                throw new InvalidOperationException("Initialize has already been called on this instance. This can only be called once per instance.");
            }
            OnInitializing();
            OriginalEntityData = entityData;
            EntityData = CloneEntityData(entityData);
            _propertyChangedSinceIsDirtySet = false;
            _isDirty = false;
            _init = true;
            OnInitialized();
        }
        private bool _init;

        /// <summary>
        /// Clones the entity data.
        /// </summary>
        /// <param name="entityData">The entity data.</param>
        /// <returns></returns>
        protected virtual TEntityData CloneEntityData(TEntityData entityData)
        {
            var newEntityData = EntityDataFactory();
            
            foreach (var property in typeof(TEntityData).GetProperties())
            {
                var value = property.GetValue(entityData, null);
                property.SetValue(newEntityData, value, null);
            }

            newEntityData.Id = entityData.Id;
            newEntityData.Version = entityData.Version;

            return newEntityData;
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
                    _isDirty = false;
                    foreach (var property in typeof(TEntityData).GetProperties())
                    {
                        if (property.Name != "Id" && property.GetValue(OriginalEntityData, null) != property.GetValue(EntityData, null))
                        {
                            _isDirty = true;
                        }
                    }
                }

                return _isDirty;
            }
        }

        /// <summary>
        /// Bases the entity property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void BaseEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _propertyChangedSinceIsDirtySet = true;
        }
        private bool _propertyChangedSinceIsDirtySet;
        private bool _isDirty;

        /// <summary>
        /// Reverts the changes.
        /// </summary>
        public virtual void RevertChanges()
        {
            Initialize(OriginalEntityData);
        }

        #region Implementation of INotifyPropertyChanging

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changing].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Implementation of IObservableEntity

        /// <summary>
        /// Occurs when [saving].
        /// </summary>
        public event EventHandler Saving;
        /// <summary>
        /// Called when [saving].
        /// </summary>
        protected virtual void OnSaving()
        {
            if(Saving!=null)
                Saving.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Occurs when [saved].
        /// </summary>
        public event EventHandler Saved;
        /// <summary>
        /// Called when [saved].
        /// </summary>
        protected virtual void OnSaved()
        {
            if (Saved != null)
                Saved.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Occurs when [deleting].
        /// </summary>
        public event EventHandler Deleting;
        /// <summary>
        /// Called when [deleting].
        /// </summary>
        protected virtual void OnDeleting()
        {
            if (Deleting != null)
                Deleting.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Occurs when [deleted].
        /// </summary>
        public event EventHandler Deleted;
        /// <summary>
        /// Called when [deleted].
        /// </summary>
        protected virtual void OnDeleted()
        {
            if (Deleted != null)
                Deleted.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Occurs when [initializing].
        /// </summary>
        public event EventHandler Initializing;
        /// <summary>
        /// Called when [initializing].
        /// </summary>
        protected virtual void OnInitializing()
        {
            if (Initializing != null)
                Initializing.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Occurs when [initialized].
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// Called when [initialized].
        /// </summary>
        protected virtual void OnInitialized()
        {
            if (Initialized != null)
                Initialized.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
