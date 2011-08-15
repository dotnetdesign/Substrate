using System.Collections.Generic;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines properties required of Validation Result types.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Gets or sets the member names.
        /// </summary>
        /// <value>
        /// The member names.
        /// </value>
        IEnumerable<string> MemberNames { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the type of the status.
        /// </summary>
        /// <value>
        /// The type of the status.
        /// </value>
        ValidationResultStatusType StatusType { get; set; }
    }
}