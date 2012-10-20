using System;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// An object that uniquely identifies an entity.
    /// </summary>
    [Serializable]
    public class EntityIdentifier : EntityIdentifier<Guid>, IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityIdentifier"/> class.
        /// </summary>
        public EntityIdentifier()
            : this(Guid.NewGuid())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityIdentifier"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public EntityIdentifier(Guid id)
        {
            using (Logger.Assembly.Scope())
            {
                Id = id;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Guid"/> to <see cref="DotNetDesign.Substrate.EntityIdentifier"/>.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator EntityIdentifier(Guid guid)
        {
            using (Logger.Assembly.Scope())
            {
                return new EntityIdentifier(guid);
            }
        }
    }

    /// <summary>
    /// An object that uniquely identifies an entity.
    /// </summary>
    [Serializable]
    public class EntityIdentifier<TId> :
        IIdentifiable<TId>
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