public interface IUiManager : IManager
{
    void Open(IUiViewModel viewModel, IUiView view, string tag = null);
    void Close(IUiViewModel viewModel);
    void CloseByTag(string tag);
    bool TryGetByTag(string tag, out IUiViewModel viewModel);
}
