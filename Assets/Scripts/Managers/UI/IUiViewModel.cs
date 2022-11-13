using System;

public interface IUiViewModel : IDisposable
{
    event Action Disposed;
}
