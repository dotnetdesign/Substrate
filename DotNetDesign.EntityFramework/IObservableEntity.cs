using System;
using System.ComponentModel;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines functionality for observable entity.
    /// </summary>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public interface IObservableEntity<TId> : INotifyPropertyChanged, INotifyPropertyChanging, IIdentifiable<TId>
    {
        /// <summary>
        /// Occurs when the entity has been saving.
        /// </summary>
        event EventHandler Saving;

        /// <summary>
        /// Occurs when the entity has been saved.
        /// </summary>
        event EventHandler Saved;

        /// <summary>
        /// Occurs when the entity has been deleting.
        /// </summary>
        event EventHandler Deleting;

        /// <summary>
        /// Occurs when the entity has been deleted.
        /// </summary>
        event EventHandler Deleted;

        /// <summary>
        /// Occurs when the entity has been initializing.
        /// </summary>
        event EventHandler Initializing;

        /// <summary>
        /// Occurs when the entity has been initialized.
        /// </summary>
        event EventHandler Initialized;
    }
}