using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DataBinding;

public class UiView : IUiView
{
    private GameObject _viewInstance;
    private DataContext _dataContext;
    private bool _disposing;
    private IUiViewModel _viewModel;

    public event Action<IUiView> Disposed;
    public event Action<IUiView> Initialized;

    public UiView(string prefabName)
    {
        var assetRequest = Addressables.LoadAssetAsync<GameObject>(prefabName);

        assetRequest.Completed += HandleLoadComplete;

        void HandleLoadComplete(AsyncOperationHandle<GameObject> op)
        {
            assetRequest.Completed -= HandleLoadComplete;

            _viewInstance = GameObject.Instantiate(op.Result);

            _dataContext = _viewInstance.GetComponent<DataContext>();

            BindToViewModel();

            OnInitialize(_viewInstance);

            Initialized?.Invoke(this);
        }
    }

    public void Dispose()
    {
        if (_disposing)
        {
            return;
        }

        _disposing = true;

        OnDispose();

        if (_viewInstance != null)
        {
            GameObject.Destroy(_viewInstance);

            _viewInstance = null;
        }

        Disposed?.Invoke(this);
    }

    public void AssignViewModel(IUiViewModel viewModel)
    {
        if (_viewModel == viewModel)
        {
            return;
        }

        if (_viewModel != null)
        {
            _viewModel.Disposed -= Dispose;
        }

        _viewModel = viewModel;

        if (_viewModel != null)
        {
            _viewModel.Disposed += Dispose;
        }

        BindToViewModel();
    }

    private void BindToViewModel()
    {
        if (_dataContext != null)
        {
            _dataContext.SetDataSource(_viewModel);
        }
    }

    protected virtual void OnInitialize(GameObject viewInstance) { }
    protected virtual void OnDispose() { }
}
