using System;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Entity permissions
    /// </summary>
    [Flags]
    public enum EntityPermissions
    {
        /// <summary>
        /// No permissions to interact with the entity.
        /// </summary>
        None = 0,
        /// <summary>
        /// Permission to view the entity.
        /// </summary>
        View = 1,
        /// <summary>
        /// Permission to insert/create the entity.
        /// </summary>
        Insert = 2,
        /// <summary>
        /// Permission to update the entity.
        /// </summary>
        Update = 4,
        /// <summary>
        /// Permission to delete the entity.
        /// </summary>
        Delete = 8
    }
}