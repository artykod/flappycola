public static class SingleCoreStarter
{
    private static Core _coreInstance;

    [UnityEngine.RuntimeInitializeOnLoadMethod]
    private static void Main()
    {
        StartSession();
    }

    private static void StartSession()
    {
        _coreInstance = new Core();
    }

    public static void RestartSession()
    {
        _coreInstance?.Dispose();

        StartSession();
    }
}