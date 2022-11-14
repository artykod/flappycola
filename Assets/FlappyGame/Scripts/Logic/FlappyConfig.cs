using DataBinding;

public class FlappyConfig
{
	public readonly float Gravity;
	public readonly float Jump;
	public readonly float LevelSpeed;
	public readonly float LevelSpeedAcceleration;
	public readonly float LevelSpeedAccelerationTimeInterval;
	public readonly float ScoreAddIntervalSeconds;
	public readonly float ObstaclesDistance;
	public readonly float ObstaclesHeightMin;
	public readonly float ObstaclesHeightMax;
	public readonly int MaxPlayerScore;
	public readonly float ImmortalTimeAfterGroundDeath;
	public readonly float StartGameTimer;

    public FlappyConfig(IDataSource config)
    {
		Gravity = config.GetDataValueOrDefault<float>("gravity");
		Jump = config.GetDataValueOrDefault<float>("jump");
		LevelSpeed = config.GetDataValueOrDefault<float>("levelSpeed");
		LevelSpeedAcceleration = config.GetDataValueOrDefault<float>("levelSpeedAcceleration");
		LevelSpeedAccelerationTimeInterval = config.GetDataValueOrDefault<float>("levelSpeedAccelerationTimeInterval");
		ScoreAddIntervalSeconds = config.GetDataValueOrDefault<float>("scoreAddIntervalSeconds");
		ObstaclesDistance = config.GetDataValueOrDefault<float>("obstaclesDistance");
		ObstaclesHeightMin = config.GetDataValueOrDefault<float>("obstaclesHeightMin");
		ObstaclesHeightMax = config.GetDataValueOrDefault<float>("obstaclesHeightMax");
		MaxPlayerScore = config.GetDataValueOrDefault<int>("maxPlayerScore");
		ImmortalTimeAfterGroundDeath = config.GetDataValueOrDefault<float>("immortalTimeAfterGroundDeath");
		StartGameTimer = config.GetDataValueOrDefault<float>("startGameTimer");
    }
}