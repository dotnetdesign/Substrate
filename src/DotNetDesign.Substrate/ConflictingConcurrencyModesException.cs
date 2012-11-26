using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Exception thrown when multiple concurrency mode attributes with different concurrency modes are assigned to a single entity data type.
    /// </summary>
    [Serializable]
    public class ConflictingConcurrencyModesException : Exception
    {
        private const string ERROR_MESSAGE_FORMAT =
            "A entity data type cannot be assigned multiple and differing concurrency modes. Entity data type {0}. Assigned concurrency modes [{1}].";

        /// <summary>
        /// Gets or sets the type of the entity data.
        /// </summary>
        /// <value>
        /// The type of the entity data.
        /// </value>
        public Type EntityDataType { get; set; }

        /// <summary>
        /// Gets or sets the conflicting concurrency modes.
        /// </summary>
        /// <value>
        /// The conflicting concurrency modes.
        /// </value>
        public IEnumerable<ConcurrencyMode> ConflictingConcurrencyModes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictingConcurrencyModesException"/> class.
        /// </summary>
        /// <param name="entityDataType">Type of the entity data.</param>
        /// <param name="conflictingConcurrencyModes">The conflicting concurrency modes.</param>
        // ReSharper disable PossibleMultipleEnumeration
        public ConflictingConcurrencyModesException(Type entityDataType,
                                                    IEnumerable<ConcurrencyMode> conflictingConcurrencyModes)
            : base(string.Format(ERROR_MESSAGE_FORMAT, entityDataType, string.Join(", ", conflictingConcurrencyModes)))
        {
            using (Logger.Assembly.Scope())
            {
                Guard.ArgumentNotNull(entityDataType, "entityDataType");
                Guard.ArgumentNotNull(conflictingConcurrencyModes, "conflictingConcurrencyModes");

                EntityDataType = entityDataType;
                ConflictingConcurrencyModes = conflictingConcurrencyModes;
            }
        }

        // ReSharper restore PossibleMultipleEnumeration

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictingConcurrencyModesException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ConflictingConcurrencyModesException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}