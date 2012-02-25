using System;
using System.Collections.Generic;
using Common.Logging;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Extension methods for WebHttpEntityContainers
    /// </summary>
    public static class WebHttpEntityContainerExtensions
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(WebHttpEntityContainerExtensions));

        /// <summary>
        /// Returns a successful container.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="message">The message.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <returns></returns>
        public static WebHttpEntityContainer<TEntity> AsSuccessContainer<TEntity>(this TEntity entity, string message = null, IEnumerable<ValidationResult> validationResults = null)
        {
            using (Logger.Scope())
            {
                return new WebHttpEntityContainer<TEntity>
                {
                    Success = true,
                    Message = message,
                    ValidationResults = validationResults,
                    Body = entity
                };
            }
        }

        /// <summary>
        /// Ases the failure container.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="message">The message.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <returns></returns>
        public static WebHttpEntityContainer<TEntity> AsFailureContainer<TEntity>(this TEntity entity, string message = null, IEnumerable<ValidationResult> validationResults = null)
        {
            using (Logger.Scope())
            {
                return new WebHttpEntityContainer<TEntity>
                {
                    Success = false,
                    Message = message,
                    ValidationResults = validationResults,
                    Body = entity
                };
            }
        }

        /// <summary>
        /// Returns a failed container.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <returns></returns>
        public static WebHttpEntityContainer<TEntity> AsFailureContainer<TEntity>(this Exception exception, string message = null, IEnumerable<ValidationResult> validationResults = null)
        {
            using (Logger.Scope())
            {
                return new WebHttpEntityContainer<TEntity>
                {
                    Success = false,
                    Message = exception.Message,
                    ValidationResults = validationResults,
                    Body = default(TEntity)
                };
            }
        }
    }
}