namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Defines methods for versionable objects.
    /// </summary>
    public interface IVersionable
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        int Version { get; set; }
    }

    /// <summary>
    /// Defines methods for versionable objects.
    /// </summary>
    /// <typeparam name="TVersionableObject">The type of the versionable object.</typeparam>
    public interface IVersionable<out TVersionableObject> : IVersionable
        where TVersionableObject : IVersionable<TVersionableObject>
    {
        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns></returns>
        TVersionableObject GetPreviousVersion();

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        TVersionableObject GetVersion(int version);
    }
}
