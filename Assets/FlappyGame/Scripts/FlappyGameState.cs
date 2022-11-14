using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DataBinding;

public class FlappyGameState : BaseGameState
{
    private readonly Session _session;
    private FlappyConfig _flappyConfig;
    private FlappyGame _game;
    private UiFlappyGameViewModel _viewModel;

    public FlappyGameState(Session session)
    {
        _session = session;
    }

    public override void Enter()
    {
        base.Enter();

        _viewModel = new UiFlappyGameViewModel(_session);
        var view = new UiView("UI/FlappyGame");

		_session.Managers.UiManager.Open(_viewModel, view);

        if (_session.GameConfig.Data.TryGetNodeByPath<IDataSource>("flappyConfig", out var flappyConfigDataSource))
        {
            _flappyConfig = new FlappyConfig(flappyConfigDataSource);

            Addressables.LoadAssetAsync<GameObject>("FlappyGame").Completed += OnGameFieldLoaded;
        }
        else
        {
            UnityEngine.Debug.LogError("No 'flappyConfig' in session.Config for FlappyGame");
        }
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

        _viewModel.AssignGame(_game);
    }

    public override void Exit()
    {
        base.Exit();

        GameObject.Destroy(_game.gameObject);

        _session.Managers.UiManager.Close(_viewModel);
    }
}
