using System;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// An object that uniquely identifies an entity.
    /// </summary>
    public class EntityIdentifier : EntityIdentifier<Guid>, IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityIdentifier"/> class.
        /// </summary>
        public EntityIdentifier()
        {
            Id = Guid.NewGuid();
        }
    }

    /// <summary>
    /// An object that uniquely identifies an entity.
    /// </summary>
    public class EntityIdentifier<TId> : IIdentifiable<TId>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public TId Id { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }
    }
}