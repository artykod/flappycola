using DataBinding;

public class UiWorldMapViewModel : UiViewModel
{
    private readonly Session _session;

    [AutoCreate] private readonly DataProperty<string> Level;
    [AutoCreate] private readonly CommandProperty LevelClick;

    public UiWorldMapViewModel(Session session)
    {
        _session = session;

        Level.SetValue<string>("Level 1");
        LevelClick.SetAction(OnLevelClick);
    }

    private void OnLevelClick(IDataSource _)
    {
        _session.GameStateFsm.GoToState(new FlappyGameState(_session));
    }
}
