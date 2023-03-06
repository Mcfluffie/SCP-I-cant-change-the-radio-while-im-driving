/// This script was written by Joshua Ferguson.
/// Version 1.0.0
/// Last updated - 02.12.22

using UnityEngine;

/// <summary>
/// Inherit from this class to create a finite state machine.
/// </summary>
public class FiniteStateMachine : MonoBehaviour
{
    /// <summary>
    /// Returns a reference to the currently active state.
    /// </summary>
    public IState CurrentState { get; private set; }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    protected virtual void Update()
    {
        CurrentState?.Update(); //Update the currently active state
    }

    /// <summary>
    /// Sets the state machine's current state to the passed state.
    /// </summary>
    /// <param name="state">The state being transitioned to.</param>
    protected void SetState(IState state)
    {
        CurrentState?.Exit(); //Exits the current state (if there is one)
        CurrentState = state; //Sets the new state as the current state
        CurrentState?.Enter(); //Enter the newly active state (if there is one)
    }
}

/// <summary>
/// Use this interface to make an object a state.
/// </summary>
public interface IState
{
    /// <summary>
    /// Use this to call functionality on the first frame the state is active.
    /// </summary>
    void Enter();
    /// <summary>
    /// Use this to call functionality every frame the state is active.
    /// </summary>
    void Update();
    /// <summary>
    /// Use this to call functionality on the last frame the state is active.
    /// </summary>
    void Exit();
}
