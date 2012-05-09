using System;
using Common.Logging;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Property change event args
    /// </summary>
    public class PropertyChangeEventArgs : EventArgs
    {
        protected readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="originalValue">The original value.</param>
        /// <param name="newValue">The new value.</param>
        public PropertyChangeEventArgs(string propertyName, object originalValue, object newValue)
        {
            using (Logger.Scope())
            {
                PropertyName = propertyName;
                OriginalValue = originalValue;
                NewValue = newValue;
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the original value.
        /// </summary>
        /// <value>
        /// The original value.
        /// </value>
        public object OriginalValue { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>
        /// The new value.
        /// </value>
        public object NewValue { get; private set; }

        /// <summary>
        /// Converts <see cref="DotNetDesign.Substrate.PropertyChangeEventArgs"/> to <see cref="System.EventArgs"/>.
        /// </summary>
        /// <param name="propertyChangeEventArgs">The <see cref="DotNetDesign.Substrate.PropertyChangeEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static EventArgs ToEventArgs(PropertyChangeEventArgs propertyChangeEventArgs)
        {
            return propertyChangeEventArgs;
        }
    }
}