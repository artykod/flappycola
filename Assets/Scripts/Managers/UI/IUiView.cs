using System;

public interface IUiView : IDisposable
{
    event Action<IUiView> Disposed;
    event Action<IUiView> Initialized;

    void AssignViewModel(IUiViewModel viewModel);
}
