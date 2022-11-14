using DataBinding;

public class UiSkinSelectViewModel : UiViewModel
{
    private readonly Session _session;

    [AutoCreate] private readonly DataSource Skins;
    [AutoCreate] private readonly CommandProperty SaveClick;
    [AutoCreate] private readonly CommandProperty CloseClick;

    public UiSkinSelectViewModel(Session session)
    {
        _session = session;

        SaveClick.SetAction(OnSaveClick);
        CloseClick.SetAction(OnCloseClick);

        Skins.AddNode(new SkinItemViewModel(0, "1", _session.Managers.Progress));
        Skins.AddNode(new SkinItemViewModel(1, "2", _session.Managers.Progress));
        Skins.AddNode(new SkinItemViewModel(2, "3", _session.Managers.Progress));
        Skins.AddNode(new SkinItemViewModel(3, "4", _session.Managers.Progress));
    }

    private void OnSaveClick(IDataSource _)
    {
        _session.Managers.Progress.Save();

        OnCloseClick(_);
    }

    private void OnCloseClick(IDataSource _)
    {
        _session.Managers.Ui.Close(this);
    }

    private class SkinItemViewModel : UiViewModel
    {
        private readonly IProgressManager _progress;

        [AutoCreate] private readonly DataProperty<string> Id;
        [AutoCreate] private readonly CommandProperty Click;

        public SkinItemViewModel(int idx, string id, IProgressManager progress) : base($"{idx}")
        {
            _progress = progress;

            Id.SetValue(id);

            Click.SetAction(OnClick);
        }

        private void OnClick(IDataSource _)
        {
            _progress.SkinId = Id.GetValue<string>();
        }
    }
}
