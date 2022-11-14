using System;
using System.Collections;

public class Session : IDisposable
{
    public interface IAsyncProvider
    {
        void StartAsync(IEnumerator async);
        void StopAsync(IEnumerator async);
    }

    private readonly FSM _gameStateFsm;
    private readonly Managers _managers;
    private readonly IAsyncProvider _asyncProvider;
    private readonly IEnumerator _frameUpdateCoroutine;

    public IAsyncProvider AsyncProvider => _asyncProvider;
    public FSM GameStateFsm => _gameStateFsm;
    public Managers Managers => _managers;
    public GameConfig GameConfig { get; private set; }

    public Session(IAsyncProvider asyncProvider)
    {
        _gameStateFsm = new FSM();
        _managers = new Managers(this);
        _asyncProvider = asyncProvider;

        _asyncProvider.StartAsync(_frameUpdateCoroutine = FrameUpdate());

        _gameStateFsm.GoToState(new ShowPreloaderState(this));
    }

    private IEnumerator FrameUpdate()
    {
        while (true)
        {
            _gameStateFsm.Update();

            yield return null;
        }
    }

    public void Dispose()
    {
        _gameStateFsm.Dispose();

        _managers.Dispose();
    }

    public void AssignGameConfig(GameConfig gameConfig)
    {
        GameConfig = gameConfig;
    }
}
