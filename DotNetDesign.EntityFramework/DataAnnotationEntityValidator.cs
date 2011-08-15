using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Validates property data annotation validation attributes on TEntity and TEntityData.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public class DataAnnotationEntityValidator<TEntity, TEntityData, TEntityRepository> :
        DataAnnotationEntityValidator<TEntity, TEntityData, Guid, TEntityRepository>, IEntityValidator<TEntity, TEntityData, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TEntityData>
    {
    }

    /// <summary>
    /// Validates property data annotation validation attributes on TEntity and TEntityData.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityData">The type of the entity data.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
    public class DataAnnotationEntityValidator<TEntity, TEntityData, TId, TEntityRepository> :
        IEntityValidator<TEntity, TEntityData, TId, TEntityRepository>
        where TEntity : class, IEntity<TEntity, TId, TEntityData, TEntityRepository>, TEntityData
        where TEntityData : class, IEntityData<TEntityData, TEntity, TId, TEntityRepository>
        where TEntityRepository : class, IEntityRepository<TEntityRepository, TEntity, TId, TEntityData>
    {
        #region IEntityValidator<TEntity,TEntityData,TId,TEntityRepository> Members

        /// <summary>
        /// Validates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public IEnumerable<IValidationResult> Validate(TEntity entity)
        {
            var validationResults = new List<IValidationResult>();

            validationResults.AddRange(ValidateType<TEntityData>(entity));
            validationResults.AddRange(ValidateType<TEntity>(entity));

            return validationResults;
        }

        #endregion

        /// <summary>
        /// Validates the type.
        /// </summary>
        /// <typeparam name="TValidatableType">The type of the validatable type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private IEnumerable<IValidationResult> ValidateType<TValidatableType>(TEntity entity)
        {
            return from property in typeof (TValidatableType).GetProperties()
                   from validationAttribute in
                       property.GetCustomAttributes(typeof (ValidationAttribute), true).OfType<ValidationAttribute>()
                   where !validationAttribute.IsValid(property.GetValue(entity, null))
                   select
                       ValidationResult.Error(
                           validationAttribute.FormatErrorMessage(GetDisplayName(property)),
                           new[] {property.Name});

        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        protected virtual string GetDisplayName(PropertyInfo propertyInfo)
        {
            return
                ((DisplayNameAttribute)
                 propertyInfo.GetCustomAttributes(typeof (DisplayNameAttribute), true).SingleOrDefault()).DisplayName;
        }
    }
}