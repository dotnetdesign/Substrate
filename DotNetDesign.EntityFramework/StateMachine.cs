using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Implementation of state machine.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    public class StateMachine<TState> : 
        BaseLogger,
        IStateMachine<TState>
    {
        private readonly IDictionary<TState, IEnumerable<TState>> _validStateChangeMap;
        private TState _currentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachine&lt;TState&gt;"/> class.
        /// </summary>
        /// <param name="validStateChangeMap">The valid state change map.</param>
        public StateMachine(IDictionary<TState, IEnumerable<TState>> validStateChangeMap)
            : this(validStateChangeMap, default(TState))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachine&lt;TState&gt;"/> class.
        /// </summary>
        /// <param name="validStateChangeMap">The valid state change map.</param>
        /// <param name="initialState">The initial state.</param>
        public StateMachine(IDictionary<TState, IEnumerable<TState>> validStateChangeMap, TState initialState)
        {
            using (Logger.Scope())
            {
                _validStateChangeMap = validStateChangeMap;
                _currentState = initialState;
            }
        }

        /// <summary>
        /// Called when [state changing].
        /// </summary>
        /// <param name="originalState">State of the original.</param>
        /// <param name="newState">The new state.</param>
        protected virtual void OnStateChanging(TState originalState, TState newState)
        {
            using (Logger.Scope())
            {
                StateChanging.Invoke(this, new StateChangeEventArgs<TState>(originalState, newState));
            }
        }

        /// <summary>
        /// Occurs when [state changing].
        /// </summary>
        public event EventHandler<StateChangeEventArgs<TState>> StateChanging = delegate { };

        /// <summary>
        /// Called when [state changed].
        /// </summary>
        /// <param name="originalState">State of the original.</param>
        /// <param name="newState">The new state.</param>
        protected virtual void OnStateChanged(TState originalState, TState newState)
        {
            using (Logger.Scope())
            {
                StateChanged.Invoke(this, new StateChangeEventArgs<TState>(originalState, newState));
            }
        }

        /// <summary>
        /// Occurs when [state changed].
        /// </summary>
        public event EventHandler<StateChangeEventArgs<TState>> StateChanged = delegate { };

        /// <summary>
        /// Gets or sets the state of the current.
        /// </summary>
        /// <value>
        /// The state of the current.
        /// </value>
        public TState CurrentState
        {
            get
            {
                using (Logger.Scope())
                {
                    return _currentState;
                }
            }
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        /// <param name="targetState">State of the target.</param>
        public void ChangeState(TState targetState)
        {
            using (Logger.Scope())
            {
                if (!GetValidNextStates().Contains(targetState))
                {
                    throw new InvalidStateException<TState>(_currentState, targetState, GetValidNextStates());
                }

                var originalState = _currentState;
                OnStateChanging(originalState, targetState);
                _currentState = targetState;
                OnStateChanged(originalState, targetState);
            }
        }

        /// <summary>
        /// Gets the valid next states.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TState> GetValidNextStates()
        {
            using (Logger.Scope())
            {
                return _validStateChangeMap[CurrentState];
            }
        }
    }
}