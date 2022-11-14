using DataBinding;

public class GameConfig
{
    public readonly IDataSource Data;

    public GameConfig(string jsonString)
    {
		Data = DataSourceFactory.FromJson("gameConfig", jsonString);
    }
}