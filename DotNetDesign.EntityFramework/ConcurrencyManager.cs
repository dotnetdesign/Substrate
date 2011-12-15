using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Manages concurrency of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public class ConcurrencyManager<TEntity, TEntityData, TEntityRepository> : ConcurrencyManager<TEntity, Guid, TEntityData, TEntityRepository>, IConcurrencyManager<TEntity, TEntityData, TEntityRepository> 
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyManager&lt;TEntity, TEntityData, TEntityRepository&gt;"/> class.
        /// </summary>
        /// <param name="entityRepository">The entity repository.</param>
        /// <param name="excludedPropertyNames">The excluded property names.</param>
        public ConcurrencyManager(TEntityRepository entityRepository, IEnumerable<string> excludedPropertyNames) : base(entityRepository, excludedPropertyNames)
        {
        }
    }

    /// <summary>
    /// Manages concurrency of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public class ConcurrencyManager<TEntity, TId, TEntityData, TEntityRepository> : 
        BaseLogger,
        IConcurrencyManager<TEntity, TId, TEntityData, TEntityRepository> 
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {

        /// <summary>
        /// Property names to excluded when comparing changes by default.
        /// </summary>
        public static readonly IEnumerable<string> DefaultExcludedPropertyNames = new[]
                                                                                      {
                                                                                          "Id", "CreatedAt", "UpdatedAt",
                                                                                          "Version", "VersionId"
                                                                                      };

        private readonly IEnumerable<string> _excludedPropertyNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyManager&lt;TEntity, TId, TEntityData, TEntityRepository&gt;"/> class.
        /// </summary>
        /// <param name="entityRepository">The entity repository.</param>
        /// <param name="excludedPropertyNames">The excluded property names.</param>
        public ConcurrencyManager(TEntityRepository entityRepository, IEnumerable<string> excludedPropertyNames = null)
        {
            using (Logger.Scope())
            {
                EntityRepository = entityRepository;
                _excludedPropertyNames = excludedPropertyNames ?? DefaultExcludedPropertyNames;
            }
        }

        /// <summary>
        /// Gets or sets the entity repository.
        /// </summary>
        /// <value>
        /// The entity repository.
        /// </value>
        public TEntityRepository EntityRepository { get; set; }

        /// <summary>
        /// Verifies the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Verify(TEntity entity)
        {
            using (Logger.Scope())
            {
                // we need to determine the concurrency mode for this entity.
                // concurrency mode should be assigned via an attribute on the entity data interface.

                var assignedConcurrencyModeAttributes =
                    typeof(TEntityData).GetCustomAttributes(typeof(ConcurrencyModeAttribute), true).OfType
                        <ConcurrencyModeAttribute>();

                var distinctConcurrencyModes = assignedConcurrencyModeAttributes.Select(x => x.ConcurrencyMode).Distinct();
                if (distinctConcurrencyModes.Count() > 1)
                {
                    throw new ConflictingConcurrencyModesException(typeof(TEntityData), distinctConcurrencyModes);
                }

                // above, we will throw an exception if there are more than one distinct concurrnecy modes assigned.
                // now, we either need that distinct concurrency mode or the default concurrency mode in order to proceed.
                var concurrencyMode = distinctConcurrencyModes.FirstOrDefault();

                Logger.InfoFormat("Concurrency Mode [{0}].", concurrencyMode);

                if (concurrencyMode == ConcurrencyMode.Overwrite)
                {
                    return;
                }

                TEntity retrievedEntity;

                if (!HasEntityChanged(entity, out retrievedEntity))
                {
                    Logger.InfoFormat("Entity [{0}] hasn't changed.", entity);
                    return;
                }

                Logger.InfoFormat("Entity [{0}] has changed.", entity);

                // entity has changed.
                if (concurrencyMode == ConcurrencyMode.Fail)
                {
                    throw new ConcurrencyConflictException(typeof(TEntityData), concurrencyMode);
                }

                if (concurrencyMode == ConcurrencyMode.Merge)
                {
                    IList<string> conflictingPropertyNames;
                    if (!TryMergeChanges(entity, retrievedEntity, out conflictingPropertyNames))
                    {
                        Logger.InfoFormat("Merge failed for entity [{0}]. Conflicting property names [{1}].", entity, string.Join(",", conflictingPropertyNames));

                        throw new ConcurrencyConflictException(typeof(TEntityData), concurrencyMode, conflictingPropertyNames);
                    }

                    Logger.InfoFormat("Merge succeeded for entity [{0}].", entity);

                    // merge was successful so we should update the current version to that of the retrieved entity.
                    // it will then be incremented by 1 by the base entity before persisting.
                    entity.Version = retrievedEntity.Version;
                }
            }
        }

        private bool HasEntityChanged(TEntity entity, out TEntity retrievedEntity)
        {
            using (Logger.Scope())
            {
                retrievedEntity = EntityRepository.GetById(entity.Id, true);

                if (retrievedEntity == null)
                {
                    Logger.InfoFormat("No entity of type [{0}] returned for id [{1}]", typeof(TEntity), entity.Id);
                    return false;
                }

                var versionChanged = entity.Version != retrievedEntity.Version;
                var lastUpdatedAtChanged = entity.LastUpdatedAt != retrievedEntity.LastUpdatedAt;
                var entityChanged = versionChanged || lastUpdatedAtChanged;

                Logger.InfoFormat("Version Changed [{0}]. Last Updated At Changed [{1}]. Entity Changed [{2}].",
                    versionChanged, lastUpdatedAtChanged, entityChanged);

                return entityChanged;
            }
        }

        private bool TryMergeChanges(TEntity currentEntity, TEntity retrievedEntity, out IList<string> conflictingPropertyNames)
        {
            using (Logger.Scope())
            {
                // we need to try to merge the changes to the current entity into the retrieved entity
                // we will do this by going through each property in the current entity
                // if that property is changed we need to check to see if the value of that property in the 
                // retrieved entity is the same as the original value in the current entity
                // - if it is, we can write the new value from the current entity to the retrieved entity 
                // - if it is not, we must throw a ConcurrencyConflictException

                conflictingPropertyNames = new List<string>();
                
                foreach (var propertyInfo in typeof(TEntityData).GetProperties().Where(IsPropertyIncluded))
                {
                    object originalValue;
                    var currentPropertyChanged = currentEntity.HasPropertyChanged(propertyInfo.Name, out originalValue);

                    object currentValue;
                    currentValue = propertyInfo.GetValue(currentEntity, null);

                    var retrievedValue = propertyInfo.GetValue(retrievedEntity, null);
                    var retrievedValueChangedFromOriginal = originalValue != retrievedValue ||
                                                             (originalValue != null && !originalValue.Equals(retrievedValue));

                    if (currentPropertyChanged)
                    {
                        if (retrievedValueChangedFromOriginal)
                        {
                            Logger.InfoFormat("Property [{0}] has changed in both current value [{1}] and retrieved value [{2}] is different from the original value of the current entity [{3}].",
                                propertyInfo.Name, currentValue, retrievedValue, originalValue);

                            // property has changed so we cannot merge
                            conflictingPropertyNames.Add(propertyInfo.Name);
                        }
                    }
                    else if (retrievedValueChangedFromOriginal)
                    {
                        Logger.InfoFormat("Property [{0}] hasn't changed in the current entity but the recieved entity value [{1}] changed from the current entity original value [{2}].",
                            propertyInfo.Name, retrievedValue, originalValue);

                        // this means the current property hasn't changed since before so go ahead and merge new value
                        propertyInfo.SetValue(currentEntity, propertyInfo.GetValue(retrievedEntity, null), null);
                    }
                }

                return conflictingPropertyNames.Count == 0;
            }
        }

        private bool IsPropertyIncluded(PropertyInfo x)
        {
            using (Logger.Scope())
            {
                return !_excludedPropertyNames.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase);
            }
        }
    }
}