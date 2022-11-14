public class ShowPreloaderState : BaseGameState
{
    private readonly Session _session;

    public ShowPreloaderState(Session session)
    {
        _session = session;
    }

    public override void Enter()
    {
        base.Enter();

        _session.Managers.UiManager.Open(new UiPreloaderViewModel(), new UiView(UiCommonNames.Preloader), UiCommonNames.Preloader);

        _session.GameStateFsm.GoToState(new LoadGameConfigState(_session));
    }
}
