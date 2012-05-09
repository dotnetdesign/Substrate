using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines functionality of a validatable object.
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        bool IsValid { get; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ValidationResult> Validate();
    }
}