using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections;

public class LoadGameConfigState : BaseGameState
{
    private readonly Session _session;

    public LoadGameConfigState(Session session)
    {
        _session = session;
    }

    public override void Enter()
    {
        base.Enter();

        _session.AsyncProvider.StartAsync(LoadGameConfig());

        IEnumerator LoadGameConfig()
        {
            var loadGameConfigRequest = Addressables.LoadAssetAsync<TextAsset>("GameConfig");

            yield return loadGameConfigRequest;

            var gameConfigJsonAsset = loadGameConfigRequest.Result;
            var gameConfigJson = gameConfigJsonAsset.text;
            var gameConfig = new GameConfig(gameConfigJson);

            _session.AssignGameConfig(gameConfig);

            yield return new WaitForSecondsRealtime(1f);

            _session.GameStateFsm.GoToState(new WorldMapState(_session));
        }
    }
}
