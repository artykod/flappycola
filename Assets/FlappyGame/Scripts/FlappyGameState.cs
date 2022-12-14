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

		_session.Managers.Ui.Open(_viewModel, view);

        Addressables.LoadAssetAsync<GameObject>("FlappyGame").Completed += OnGameFieldLoaded;
    }

    private void OnGameFieldLoaded(AsyncOperationHandle<GameObject> op)
    {
        op.Completed -= OnGameFieldLoaded;

        var playerGuid = "player_1_guid";
        var playerName = "Player 1";
        var playerJumpKey = 0;
        var skinId = _session.Managers.Progress.SkinId ?? "1";
        var startLives = _flappyConfig.PlayerStartLives;
        var maxLives = _flappyConfig.MaxPlayerLives;

        var players = new[]
        {
            new PlayerInfo(playerGuid, skinId, playerName, playerJumpKey, startLives, maxLives),
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

        _session.Managers.Ui.Close(_viewModel);
    }
}
