public class FlappyResultState : BaseGameState
{
    private readonly Session _session;
    private readonly FlappyConfig _config;
    private readonly PlayersCollection _players;

    private UiFlappyResultViewModel _viewModel;

    public FlappyResultState(Session session, FlappyConfig config, PlayersCollection players)
    {
        _session = session;
        _config = config;
        _players = players;
    }

    public override void Enter()
    {
        base.Enter();

        _viewModel = new UiFlappyResultViewModel(_session, _players);
        var view = new UiView("UI/FlappyResult");

        _session.Managers.UiManager.Open(_viewModel, view);
    }

    public override void Exit()
    {
        base.Exit();

        _session.Managers.UiManager.Close(_viewModel);
    }
}
