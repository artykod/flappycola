using System.Collections.Generic;

public class UiManager : IUiManager
{
    private struct ViewEntry
    {
        public IUiViewModel ViewModel;
        public IUiView View;
        public string Tag;
    }

    private readonly List<ViewEntry> _viewInstances = new List<ViewEntry>(32);

    public void Dispose()
    {
        foreach (var entry in _viewInstances)
        {
            entry.ViewModel?.Dispose();
            entry.View?.Dispose();
        }

        _viewInstances.Clear();
    }

    public void Open(IUiViewModel viewModel, IUiView view, string tag = null)
    {
        _viewInstances.Add(new ViewEntry
        {
            ViewModel = viewModel,
            View = view,
            Tag = tag,
        });

        view.AssignViewModel(viewModel);
    }

    public void Close(IUiViewModel viewModel)
    {
        _viewInstances.RemoveAll(i => i.ViewModel == viewModel);

        viewModel?.Dispose();
    }

    public void CloseByTag(string tag)
    {
        if (TryGetByTag(tag, out var viewModel))
        {
            Close(viewModel);
        }
    }

    public bool TryGetByTag(string tag, out IUiViewModel viewModel)
    {
        viewModel = _viewInstances.Find(i => i.Tag == tag).ViewModel;

        return viewModel != null;
    }
}
