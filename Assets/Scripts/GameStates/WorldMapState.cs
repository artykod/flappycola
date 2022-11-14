public class WorldMapState : BaseGameState
{
    private readonly Session _session;

    private IUiViewModel _worldMapViewModel;

    public WorldMapState(Session session)
    {
        _session = session;
    }

    public override void Enter()
    {
        base.Enter();

        var view = new UiView("UI/WorldMap");
        
        _worldMapViewModel = new UiWorldMapViewModel(_session);

        _session.Managers.Ui.Open(_worldMapViewModel, view);

        view.Initialized += WorldMapInitialized;
    }

    private void WorldMapInitialized(IUiView view)
    {
        view.Initialized -= WorldMapInitialized;

        _session.Managers.Ui.CloseByTag(UiCommonNames.Preloader);
    }

    public override void Exit()
    {
        base.Exit();

        _session.Managers.Ui.Close(_worldMapViewModel);
    }
}
