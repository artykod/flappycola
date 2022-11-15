using SimpleJSON;
using System;
using System.IO;
using UnityEngine;

public class PersistentProgressManager : IProgressManager
{
    private const string PersistentProgressFileName = "progress";

    private readonly Session _session;

    public string SkinId { get; set; }

    public PersistentProgressManager(Session session)
    {
        _session = session;

        Load();
    }

    public void Dispose()
    {
    }

    private void SetDefaultValues()
    {
        SkinId = _session.GameConfig.Data.GetDataValueOrDefault<string>("defaultFlappySkin");
    }

    private void Load()
    {
        var persistentPath = Path.Combine(Application.persistentDataPath, PersistentProgressFileName);

        try
        {
            var persistentFileExists = File.Exists(persistentPath);
            var jsonString = persistentFileExists ? File.ReadAllText(persistentPath) : "{}";
            var json = JSON.Parse(jsonString);

            SkinId = json.HasKey("skinId") ? json["skinId"] : SkinId;

            Debug.Log($"Progress from persistent: {json}");
        }
        catch (Exception e)
        {
            Debug.LogError("Load progress from persistent failed");
            Debug.LogException(e);
        }
    }

    public void Save()
    {
        var persistentPath = Path.Combine(Application.persistentDataPath, PersistentProgressFileName);
        var json = new JSONObject();

        try
        {
            json["skinId"] = SkinId;

            Debug.Log($"Save progress to persistent: {json}");

            File.WriteAllText(persistentPath, json.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError("Save progress to persistent failed");
            Debug.LogException(e);
        }
    }
}
