using System;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Property change event args
    /// </summary>
    public class PropertyChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="originalValue">The original value.</param>
        /// <param name="newValue">The new value.</param>
        public PropertyChangeEventArgs(string propertyName, object originalValue, object newValue)
        {
            PropertyName = propertyName;
            OriginalValue = originalValue;
            NewValue = newValue;
        }

        #region Implementation of IPropertyChangeEventArgs<TPropertyType>

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

        #endregion
    }
}