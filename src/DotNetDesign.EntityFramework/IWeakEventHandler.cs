using System;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines methods of a weak event handler.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    public interface IWeakEventHandler<TEventArgs>
      where TEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the handler.
        /// </summary>
        EventHandler<TEventArgs> Handler { get; }
    }
}