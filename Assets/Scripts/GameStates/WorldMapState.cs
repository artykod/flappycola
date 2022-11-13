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

        var view = new UiWorldMapView(_session, UiNames.WorldMap);
        
        _worldMapViewModel = new UiWorldMapViewModel();

        _session.Managers.UiManager.Open(_worldMapViewModel, view);

        view.Initialized += WorldMapInitialized;
    }

    private void WorldMapInitialized(IUiView view)
    {
        view.Initialized -= WorldMapInitialized;

        _session.Managers.UiManager.CloseByTag(UiNames.Preloader);
    }

    public override void Exit()
    {
        base.Exit();

        _session.Managers.UiManager.Close(_worldMapViewModel);
    }
}
