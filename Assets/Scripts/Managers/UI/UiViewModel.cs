using System;
using DataBinding;

public class UiViewModel : DataSource, IUiViewModel
{
    private bool _disposing;

    public event Action Disposed;

    public UiViewModel() : base()
    {
    }

    public void Dispose()
    {
        if (_disposing)
        {
            return;
        }

        _disposing = true;

        Disposed?.Invoke();
    }

    protected virtual void OnDispose() {}
}
