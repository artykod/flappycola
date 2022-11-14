using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DataBinding;

public class FlappyGameState : BaseGameState
{
    private readonly Session _session;
    private readonly FlappyConfig _flappyConfig;
    private FlappyGame _game;
    private UiFlappyGameViewModel _viewModel;

    public FlappyGameState(Session session, IDataSource flappyConfigDataSource)
    {
        _session = session;
        _flappyConfig = new FlappyConfig(flappyConfigDataSource);
    }

    public override void Enter()
    {
        base.Enter();

        _viewModel = new UiFlappyGameViewModel(_session);
        var view = new UiView("UI/FlappyGame");

		_session.Managers.UiManager.Open(_viewModel, view);

        Addressables.LoadAssetAsync<GameObject>("FlappyGame").Completed += OnGameFieldLoaded;
    }

    private void OnGameFieldLoaded(AsyncOperationHandle<GameObject> op)
    {
        op.Completed -= OnGameFieldLoaded;

        var players = new[]
        {
            new PlayerInfo("player_1_guid", "1", "Test1", 0),
        };

        _game = GameObject.Instantiate(op.Result).GetComponent<FlappyGame>();
        _game.Initialize(_session, _flappyConfig, players);

        _game.OnGameFinish += OnFlappyGameFinish;

        _viewModel.AssignGame(_game);
    }

    private void OnFlappyGameFinish(FlappyGame game)
    {
        _game.OnGameFinish -= OnFlappyGameFinish;

        _session.GameStateFsm.GoToState(new FlappyResultState(_session, _flappyConfig, game.CurrentPlayers));
    }

    public override void Exit()
    {
        base.Exit();

        GameObject.Destroy(_game.gameObject);

        _session.Managers.UiManager.Close(_viewModel);
    }
}
