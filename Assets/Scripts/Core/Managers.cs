using System;

public class Managers : IDisposable
{
    private readonly Session _session;

    public IUiManager Ui { get; private set; }
    public IProgressManager Progress { get; private set; }

    public Managers(Session session)
    {
        _session = session;

        CreateInitialManagers();
    }

    public void Dispose()
    {
        Ui?.Dispose();
        Progress?.Dispose();
    }

    private void CreateInitialManagers()
    {
        Ui = new UiManager();
        Progress = new PersistentProgressManager();
    }
}
