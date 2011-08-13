using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines properties for identifiable objects.
    /// </summary>
    public interface IIdentifiable : IIdentifiable<Guid>
    {
    }

    /// <summary>
    /// Defines properties for identifiable objects.
    /// </summary>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public interface IIdentifiable<TId>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        TId Id { get; set; }
    }
}
