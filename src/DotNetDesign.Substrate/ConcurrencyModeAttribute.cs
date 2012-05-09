using System;
using Common.Logging;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Sets the concurrency mode for the associated type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ConcurrencyModeAttribute : Attribute
    {
        protected readonly ILog Logger = LogManager.GetLogger(typeof(ConcurrencyModeAttribute));

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyModeAttribute"/> class.
        /// </summary>
        /// <param name="concurrencyMode">The concurrency mode.</param>
        public ConcurrencyModeAttribute(ConcurrencyMode concurrencyMode)
        {
            using (Logger.Scope())
            {
                ConcurrencyMode = concurrencyMode;
            }
        }

        /// <summary>
        /// Gets or sets the concurrency mode.
        /// </summary>
        /// <value>
        /// The concurrency mode.
        /// </value>
        public ConcurrencyMode ConcurrencyMode { get; set; }
    }
}