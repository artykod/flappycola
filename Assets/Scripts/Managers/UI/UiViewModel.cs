using System;

public class UiViewModel : IUiViewModel
{
    private bool _disposing;

    public event Action Disposed;

    public void Dispose()
    {
        if (_disposing)
        {
            return;
        }

        _disposing = true;

        Disposed?.Invoke();
    }
}
