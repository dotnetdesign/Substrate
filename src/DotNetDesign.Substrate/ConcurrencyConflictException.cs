using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common.Logging;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Thrown when a concurrency conflict is encountered.
    /// </summary>
    [Serializable]
    public class ConcurrencyConflictException : Exception
    {
        private const string ERROR_MESSAGE_FORMAT =
            "One or more concurrency conflicts were encountered and prevented this entity from saving. Entity data type {0}. Assigned concurrency mode {1}. Conflicting property name(s) [{2}].";

        protected readonly ILog Logger = Common.Logging.LogManager.GetLogger(typeof(ConcurrencyConflictException));

        /// <summary>
        /// Gets or sets the type of the entity data.
        /// </summary>
        /// <value>
        /// The type of the entity data.
        /// </value>
        public Type EntityDataType { get; set; }

        /// <summary>
        /// Gets or sets the concurrency mode.
        /// </summary>
        /// <value>
        /// The concurrency mode.
        /// </value>
        public ConcurrencyMode ConcurrencyMode { get; set; }

        /// <summary>
        /// Gets or sets the conflicting property names.
        /// </summary>
        /// <value>
        /// The conflicting property names.
        /// </value>
        public IEnumerable<string> ConflictingPropertyNames { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class.
        /// </summary>
        /// <param name="entityDataType">Type of the entity data.</param>
        /// <param name="concurrencyMode">The concurrency mode.</param>
        /// <param name="conflictingPropertyNames">The conflicting property names.</param>
        public ConcurrencyConflictException(Type entityDataType, ConcurrencyMode concurrencyMode, IEnumerable<string> conflictingPropertyNames = null)
            : base(string.Format(ERROR_MESSAGE_FORMAT, entityDataType, concurrencyMode, (conflictingPropertyNames == null) ? "" : string.Join(", ", conflictingPropertyNames)))
        {
            using (Logger.Scope())
            {
                EntityDataType = entityDataType;
                ConcurrencyMode = concurrencyMode;
                ConflictingPropertyNames = conflictingPropertyNames;
            }
        }

        protected ConcurrencyConflictException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}