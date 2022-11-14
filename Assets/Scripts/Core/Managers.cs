using System;

public class Managers : IDisposable
{
    private readonly Session _session;

    public IUiManager UiManager { get; private set; }

    public Managers(Session session)
    {
        _session = session;

        CreateInitialManagers();
    }

    public void Dispose()
    {
        UiManager?.Dispose();
    }

    private void CreateInitialManagers()
    {
        UiManager = new UiManager();
    }
}
