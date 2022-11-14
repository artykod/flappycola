using System;

public class FSM : IDisposable
{
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
    }

    private IState _currentState;
    private bool _isDisposing;

    public void GoToState(IState state)
    {
        if (_isDisposing)
        {
            return;
        }

        var prevState = _currentState;

        _currentState?.Exit();

        _currentState = state;

        UnityEngine.Debug.Log($"Change state {prevState?.GetType()?.Name} => {_currentState?.GetType()?.Name}");

        state.Enter();
        state.Update();
    }

    public void Update()
    {
        if (_isDisposing)
        {
            return;
        }

        _currentState?.Update();
    }

    public void Dispose()
    {
        _isDisposing = true;

        _currentState?.Exit();

        _currentState = null;
    }
}