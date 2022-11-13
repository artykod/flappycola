public class GameConfig
{
    public readonly bool Debug;
	public readonly float ResultsShowTime;
	public readonly float Gravity;
	public readonly float Jump;
	public readonly float LevelSpeed;
	public readonly float LevelSpeedAcceleration;
	public readonly float LevelSpeedAccelerationTimeInterval;
	public readonly float ScoreAddIntervalSeconds;
	public readonly float ObstaclesDistance;
	public readonly float ObstaclesHeightMin;
	public readonly float ObstaclesHeightMax;
	public readonly float MaxPlayerScore;
	public readonly float ImmortalTimeAfterGroundDeath;
	public readonly float StartGameTimer;

    public GameConfig(string jsonString)
    {
        var json = SimpleJSON.JSON.Parse(jsonString);

		Debug = json["debug"];
		ResultsShowTime = json["resultsShowTime"];
		Gravity = json["gravity"];
		Jump = json["jump"];
		LevelSpeed = json["levelSpeed"];
		LevelSpeedAcceleration = json["levelSpeedAcceleration"];
		LevelSpeedAccelerationTimeInterval = json["levelSpeedAccelerationTimeInterval"];
		ScoreAddIntervalSeconds = json["scoreAddIntervalSeconds"];
		ObstaclesDistance = json["obstaclesDistance"];
		ObstaclesHeightMin = json["obstaclesHeightMin"];
		ObstaclesHeightMax = json["obstaclesHeightMax"];
		MaxPlayerScore = json["maxPlayerScore"];
		ImmortalTimeAfterGroundDeath = json["immortalTimeAfterGroundDeath"];
		StartGameTimer = json["startGameTimer"];
    }
}