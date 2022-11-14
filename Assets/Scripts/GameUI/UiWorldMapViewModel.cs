using DataBinding;

public class UiWorldMapViewModel : UiViewModel
{
    private readonly Session _session;

    public UiWorldMapViewModel(Session session)
    {
        _session = session;

        AddNode(new DataProperty<string>("level", "Level 1"));
        AddNode(new CommandProperty("levelClick", OnLevelClick));
    }

    private void OnLevelClick(IDataSource _)
    {
        _session.GameStateFsm.GoToState(new FlappyGameState(_session));
    }
}
