using DataBinding;

public class UiWorldMapViewModel : UiViewModel
{
    private readonly Session _session;

    [AutoCreate] private readonly DataSource FlappyLevels;

    public UiWorldMapViewModel(Session session)
    {
        _session = session;

        if (session.GameConfig.Data.TryGetNode<IDataSource>("flappyConfigs", out var flappyConfigs))
        {
            flappyConfigs.ForEach<IDataSource>(levelConfig =>
            {
                FlappyLevels.AddNode(new UiFlappyLevelViewModel(_session, levelConfig));
            });
        }
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
