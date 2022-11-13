using System;
using System.Collections;
using UnityEngine;

public class Core : IDisposable, Session.IAsyncProvider
{
    private class CoreMonoBehaviour : MonoBehaviour { }

    [UnityEngine.RuntimeInitializeOnLoadMethod]
    private static void Main()
    {
        StartSession();
    }

    private static Core _coreInstance;

    private static void StartSession()
    {
        _coreInstance = new Core();
    }

    public static void RestartSession()
    {
        _coreInstance?.Dispose();

        StartSession();
    }

    private readonly Session _session;
    private readonly CoreMonoBehaviour _monoBehaviour;

    private Core()
    {
        var gameObject = new GameObject("CoreMonoBehaviour");

        _monoBehaviour = gameObject.AddComponent<CoreMonoBehaviour>();

        GameObject.DontDestroyOnLoad(gameObject);

        _session = new Session(this);
    }

    public void Dispose()
    {
        _session?.Dispose();

        if (_monoBehaviour != null)
        {
            _monoBehaviour.StopAllCoroutines();

            GameObject.Destroy(_monoBehaviour.gameObject);
        }
    }

    public void StartAsync(IEnumerator asyncOp)
    {
        _monoBehaviour.StartCoroutine(asyncOp);
    }

    public void StopAsync(IEnumerator asyncOp)
    {
        _monoBehaviour.StopCoroutine(asyncOp);
    }
}
