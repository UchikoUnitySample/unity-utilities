using UnityEngine;

public abstract class State<T> where T : class
{
    abstract public void Enter(T t);
    abstract public void Execute(T t);
    abstract public void Exit(T t);
}

public class TStateMachine<T,S>
    where T : class
    where S : State<T>
{
    T owner;

    S currentState;
    public S CurrentState
    {
        get { return currentState; }
    }

    S previousState;
    public S PreviousState
    {
        get { return previousState; }
    }

    S globalState;
    public S GlobalState
    {
        get { return globalState; }
    }

    public TStateMachine(T owner)
    {
        this.owner = owner;
        this.currentState = null;
        this.previousState = null;
        this.globalState = null;
    }

    public void Update()
    {
        if (globalState != null)
            globalState.Execute(owner);
        if (currentState != null)
            currentState.Execute(owner);
    }

    public void ChangeState(S newState)
    {
        if (currentState != null)
        {
            previousState = currentState;
            currentState.Exit(owner);
        }
        currentState = newState;
        currentState.Enter(owner);
    }

    public void RevertToPreviousState()
    {
        ChangeState(previousState);
    }
}

public class StateMachine<T> : TStateMachine<T, State<T>>
    where T : class
{
    public StateMachine(T owner) : base(owner)
    {
    }
}