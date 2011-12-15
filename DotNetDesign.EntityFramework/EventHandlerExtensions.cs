using System;
using Common.Logging;
using DotNetDesign.EntityFramework;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Event Handler Extension Methods
    /// </summary>
    public static class EventHandlerExtensions
    {
        private static readonly ILog Logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            using (Logger.Scope())
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

                var weh = (IWeakEventHandler<TEventArgs>)wehConstructor.Invoke(new object[] { eventHandler, unregister });

                return weh.Handler;
            }
        }
    }
}