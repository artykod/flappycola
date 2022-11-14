using System;
using DataBinding;

public interface IUiViewModel : IDataSource, IDisposable
{
    event Action Disposed;
}
