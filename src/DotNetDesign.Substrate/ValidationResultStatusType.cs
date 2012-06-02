namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Validation Result Status Types
    /// </summary>
    public enum ValidationResultStatusType
    {
        /// <summary>
        /// Info - Validation failed with non-blocking status type Info.
        /// </summary>
        Info,

        /// <summary>
        /// Warning - Validation failed with non-blocking status type Warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Error - Validation failed with blocking status type Error
        /// </summary>
        Error
    }
}