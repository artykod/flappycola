using System.Collections.Generic;
using DataBinding;

public class UiFlappyResultViewModel : UiViewModel
{
    public UiFlappyResultViewModel(PlayersCollection players)
    {
        var sortedPlayers = new List<PlayerInfo>(players);

		sortedPlayers.Sort((p1, p2) => p1.TotalScores.CompareTo(p2.TotalScores));

        var playersDataSource = new DataSource("players");

        for (int i = 0, l = sortedPlayers.Count; i < l; ++i)
        {
            var playerDataSource = new DataSource($"{i}");

            playerDataSource.AddNode(new DataProperty<string>("name", sortedPlayers[i].PlayerName));
            playerDataSource.AddNode(new DataProperty<int>("score", sortedPlayers[i].TotalScores));

            playersDataSource.AddNode(playerDataSource);
        }

        AddNode(playersDataSource);
    }
}
