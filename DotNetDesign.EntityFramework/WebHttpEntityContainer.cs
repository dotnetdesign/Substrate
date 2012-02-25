using System.Collections.Generic;
using System.Runtime.Serialization;
using Common.Logging;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Object that wraps the response for a IWebHttpEntityRepsotiryService with metadata
    /// </summary>
    [DataContract]
    public class WebHttpEntityContainer<TBody>
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(WebHttpEntityContainer<TBody>));

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WebHttpEntityContainer&lt;TBody&gt;"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the validation results.
        /// </summary>
        /// <value>
        /// The validation results.
        /// </value>
        [DataMember]
        public IEnumerable<ValidationResult> ValidationResults { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [DataMember]
        public TBody Body { get; set; }

        /// <summary>
        /// Generates a success container with a null body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <returns></returns>
        public static WebHttpEntityContainer<TBody> NullSuccessContainer(string message = null, IEnumerable<ValidationResult> validationResults = null)
        {
            using (Logger.Scope())
            {
                return new WebHttpEntityContainer<TBody>
                           {
                               Success = true,
                               ValidationResults = validationResults,
                               Message = message
                           };
            }
        }

        /// <summary>
        /// Generates a failed container with a null body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <returns></returns>
        public static WebHttpEntityContainer<TBody> NullFailureContainer(string message = null, IEnumerable<ValidationResult> validationResults = null)
        {
            using (Logger.Scope())
            {
                return new WebHttpEntityContainer<TBody>
                {
                    Success = false,
                    ValidationResults = validationResults,
                    Message = message
                };
            }
        }
    }
}