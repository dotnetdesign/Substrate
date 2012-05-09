using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common.Logging;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Invalid state exception
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    [Serializable]
    public class InvalidStateException<TState> : Exception
    {
        private const string ERROR_MESSAGE_FORMAT =
            "Change from state {0} to state {1} is not allowed. Allowed states, [{2}].";

        protected readonly ILog Logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStateException&lt;TState&gt;"/> class.
        /// </summary>
        /// <param name="currentState">State of the current.</param>
        /// <param name="targetState">State of the target.</param>
        /// <param name="allowedStates">The allowed states.</param>
        public InvalidStateException(TState currentState, TState targetState, IEnumerable<TState> allowedStates)
            : base(string.Format(ERROR_MESSAGE_FORMAT, currentState, targetState, string.Join(", ", allowedStates)))
        {
            using (Logger.Scope())
            {
                CurrentState = currentState;
                TargetState = targetState;
                AllowedStates = allowedStates;
            }
        }

        /// <summary>
        /// Gets the state of the current.
        /// </summary>
        /// <value>
        /// The state of the current.
        /// </value>
        public TState CurrentState { get; private set; }

        /// <summary>
        /// Gets the state of the target.
        /// </summary>
        /// <value>
        /// The state of the target.
        /// </value>
        public TState TargetState { get; private set; }

        /// <summary>
        /// Gets the allowed states.
        /// </summary>
        public IEnumerable<TState> AllowedStates { get; private set; }
    }
}