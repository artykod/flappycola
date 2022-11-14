using DataBinding;

public class GameConfig
{
    public readonly IDataSource Data;

    public GameConfig(string jsonString)
    {
        Data = DataSourceFactory.FromJson("gameConfig", jsonString);

        AddDebugDataContext(Data);
    }

    private void AddDebugDataContext(IDataSource dataSource)
    {
        var go = new UnityEngine.GameObject("[Debug]GameConfigDataContext");
        var dataContext = go.AddComponent<DataContext>();

        dataContext.SetDataSource(dataSource);

        UnityEngine.GameObject.DontDestroyOnLoad(go);
    }
}