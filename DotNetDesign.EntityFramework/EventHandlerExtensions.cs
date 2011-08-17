using System;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Event Handler Extension Methods
    /// </summary>
    public static class EventHandlerExtensions
    {
        /// <summary>
        /// Makes the weak.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="unregister">The unregister.</param>
        /// <returns></returns>
        public static EventHandler<TEventArgs> MakeWeak<TEventArgs>(this EventHandler<TEventArgs> eventHandler, UnregisterCallback<TEventArgs> unregister)
          where TEventArgs : EventArgs
        {
            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler");

            if (eventHandler.Method.IsStatic || eventHandler.Target == null)
                throw new ArgumentException("Only instance methods are supported.", "eventHandler");

            var wehType = typeof(WeakEventHandler<,>).MakeGenericType(eventHandler.Method.DeclaringType, typeof(TEventArgs));

            var wehConstructor = wehType.GetConstructor(
                new[]
                    {
                        typeof (EventHandler<TEventArgs>),
                        typeof (UnregisterCallback<TEventArgs>)
                    });

            var weh = (IWeakEventHandler<TEventArgs>) wehConstructor.Invoke(new object[] {eventHandler, unregister});

            return weh.Handler;
        }
    }
}