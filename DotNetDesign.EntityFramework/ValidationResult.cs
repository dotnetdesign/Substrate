using System.Collections.Generic;

namespace DotNetDesign.EntityFramework
{
    ///<summary>
    /// Validation result
    ///</summary>
    public class ValidationResult : 
        BaseLogger,
        IValidationResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the member names.
        /// </summary>
        /// <value>
        /// The member names.
        /// </value>
        public IEnumerable<string> MemberNames { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the type of the status.
        /// </summary>
        /// <value>
        /// The type of the status.
        /// </value>
        public ValidationResultStatusType StatusType { get; set; }

        #endregion

        #region Static Factory Methods

        /// <summary>
        /// Builds an validation result with status type Info.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public static ValidationResult Info(string errorMessage)
        {
            return new ValidationResult { StatusType = ValidationResultStatusType.Info, ErrorMessage = errorMessage };
        }

        /// <summary>
        /// Builds an validation result with status type Info.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="memberNames">The member names.</param>
        /// <returns></returns>
        public static ValidationResult Info(string errorMessage, IEnumerable<string> memberNames)
        {
            return new ValidationResult { StatusType = ValidationResultStatusType.Info, ErrorMessage = errorMessage, MemberNames = memberNames };
        }

        /// <summary>
        /// Builds an validation result with status type Warning.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public static ValidationResult Warning(string errorMessage)
        {
            return new ValidationResult { StatusType = ValidationResultStatusType.Warning, ErrorMessage = errorMessage };
        }

        /// <summary>
        /// Builds an validation result with status type Warning.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="memberNames">The member names.</param>
        /// <returns></returns>
        public static ValidationResult Warning(string errorMessage, IEnumerable<string> memberNames)
        {
            return new ValidationResult { StatusType = ValidationResultStatusType.Warning, ErrorMessage = errorMessage, MemberNames = memberNames };
        }

        /// <summary>
        /// Builds an validation result with status type Error.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public static ValidationResult Error(string errorMessage)
        {
            return new ValidationResult { StatusType = ValidationResultStatusType.Error, ErrorMessage = errorMessage };
        }

        /// <summary>
        /// Builds an validation result with status type Error.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="memberNames">The member names.</param>
        /// <returns></returns>
        public static ValidationResult Error(string errorMessage, IEnumerable<string> memberNames)
        {
            return new ValidationResult { StatusType = ValidationResultStatusType.Error, ErrorMessage = errorMessage, MemberNames = memberNames };
        }

        #endregion
    }
}