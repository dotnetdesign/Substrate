using System;
using System.Collections.Generic;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Defines functionality for a state machine
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    public interface IStateMachine<TState>
    {
        /// <summary>
        /// Occurs when [state changing].
        /// </summary>
        event EventHandler<StateChangeEventArgs<TState>> StateChanging;

        /// <summary>
        /// Occurs when [state changed].
        /// </summary>
        event EventHandler<StateChangeEventArgs<TState>> StateChanged;

        /// <summary>
        /// Gets or sets the state of the current.
        /// </summary>
        /// <value>
        /// The state of the current.
        /// </value>
        TState CurrentState { get; }

        /// <summary>
        /// Changes the state.
        /// </summary>
        /// <param name="targetState">State of the target.</param>
        void ChangeState(TState targetState);

        /// <summary>
        /// Gets the valid next states.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TState> GetValidNextStates();
    }
}