using DataBinding;

public class UiWorldMapViewModel : UiViewModel
{
    private readonly Session _session;

    [AutoCreate] private readonly DataSource FlappyLevels;
    [AutoCreate] private readonly CommandProperty SkinsClick;

    public UiWorldMapViewModel(Session session)
    {
        _session = session;

        if (session.GameConfig.Data.TryGetNode<IDataSource>("flappyLevels", out var flappyLevels))
        {
            flappyLevels.ForEach<IDataSource>(levelConfig =>
            {
                FlappyLevels.AddNode(new UiFlappyLevelViewModel(_session, levelConfig));
            });
        }

        SkinsClick.SetAction(OnSkinsClick);
    }

    private void OnSkinsClick(IDataSource _)
    {
        var viewModel = new UiSkinSelectViewModel(_session);
        var view = new UiView("UI/SkinSelect");

        _session.Managers.Ui.Open(viewModel, view);
    }

    private class UiFlappyLevelViewModel : UiViewModel
    {
        private readonly Session _session;
        private readonly IDataSource _flappyConfig;

        [AutoCreate] private readonly CommandProperty Click;

        public UiFlappyLevelViewModel(Session session, IDataSource flappyConfig) : base(flappyConfig)
        {
            _session = session;
            _flappyConfig = flappyConfig;

            Click.SetAction(OnClick);
        }

        private void OnClick(IDataSource _)
        {
            _session.GameStateFsm.GoToState(new FlappyGameState(_session, _flappyConfig));
        }
    }
}
