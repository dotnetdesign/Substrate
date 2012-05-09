using System;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Defines the method signature of a method to handle the unregister callback.
    /// </summary>
    /// <typeparam name="TEventHandler"></typeparam>
    /// <param name="eventHandler">The event handler.</param>
    public delegate void UnregisterCallback<TEventHandler>(EventHandler<TEventHandler> eventHandler) where TEventHandler : EventArgs;



    /// <summary>
    /// Represents a weak referenced event handler.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    public class WeakEventHandler<TTarget, TEventArgs> :
        BaseLogger<WeakEventHandler<TTarget, TEventArgs>>,
        IWeakEventHandler<TEventArgs>
        where TTarget : class
        where TEventArgs : EventArgs
    {
        private delegate void OpenEventHandler(TTarget @this, object sender, TEventArgs e);

        private readonly WeakReference _targetRef;
        private readonly OpenEventHandler _openHandler;
        private readonly EventHandler<TEventArgs> _handler;
        private UnregisterCallback<TEventArgs> _unregister;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventHandler&lt;TTarget, TEventArgs&gt;"/> class.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="unregister">The unregister.</param>
        public WeakEventHandler(EventHandler<TEventArgs> eventHandler, UnregisterCallback<TEventArgs> unregister)
        {
            using (Logger.Scope())
            {
                _targetRef = new WeakReference(eventHandler.Target);
                _openHandler =
                    (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);
                _handler = Invoke;
                _unregister = unregister;
            }
        }

        /// <summary>
        /// Invokes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TEventArgs"/> instance containing the event data.</param>
        public void Invoke(object sender, TEventArgs e)
        {
            using (Logger.Scope())
            {
                var target = (TTarget)_targetRef.Target;

                if (target != null)
                    _openHandler.Invoke(target, sender, e);
                else if (_unregister != null)
                {
                    _unregister(_handler);
                    _unregister = null;
                }
            }
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        public EventHandler<TEventArgs> Handler
        {
            get
            {
                using (Logger.Scope())
                {
                    return _handler;
                }
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DotNetDesign.Substrate.WeakEventHandler&lt;TTarget,TEventArgs&gt;"/> to <see cref="System.EventHandler&lt;TEventArgs&gt;"/>.
        /// </summary>
        /// <param name="weh">The weh.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator EventHandler<TEventArgs>(WeakEventHandler<TTarget, TEventArgs> weh)
        {
            return weh._handler;
        }
    }

}