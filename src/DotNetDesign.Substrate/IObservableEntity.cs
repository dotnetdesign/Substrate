using System;
using System.ComponentModel;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Defines functionality for observable entity.
    /// </summary>
    public interface IObservableEntity : IObservableEntity<Guid>
    {}

    /// <summary>
    /// Defines functionality for observable entity.
    /// </summary>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public interface IObservableEntity<TId> : IIdentifiable<TId>
    {
        /// <summary>
        /// Occurs before the property changes.
        /// </summary>
        event EventHandler<PropertyChangeEventArgs> PropertyChanging;

        /// <summary>
        /// Occurs after the property has changed.
        /// </summary>
        event EventHandler<PropertyChangeEventArgs> PropertyChanged;

        /// <summary>
        /// Occurs when the entity has been saving.
        /// </summary>
        event EventHandler<EventArgs> Saving;

        /// <summary>
        /// Occurs when the entity has been saved.
        /// </summary>
        event EventHandler<EventArgs> Saved;

        /// <summary>
        /// Occurs when the entity has been deleting.
        /// </summary>
        event EventHandler<EventArgs> Deleting;

        /// <summary>
        /// Occurs when the entity has been deleted.
        /// </summary>
        event EventHandler<EventArgs> Deleted;

        /// <summary>
        /// Occurs when the entity has been initializing.
        /// </summary>
        event EventHandler<EventArgs> Initializing;

        /// <summary>
        /// Occurs when the entity has been initialized.
        /// </summary>
        event EventHandler<EventArgs> Initialized;
    }
}