using System.Collections.Generic;
using DataBinding;

public class UiFlappyResultViewModel : UiViewModel
{
    private readonly Session _session;

    [AutoCreate] private readonly DataSource Players;
    [AutoCreate] private readonly CommandProperty ContinueClick;

    public UiFlappyResultViewModel(Session session, PlayersCollection players)
    {
        _session = session;

        var sortedPlayers = new List<PlayerInfo>(players);

		sortedPlayers.Sort((p1, p2) => p1.TotalScores.CompareTo(p2.TotalScores));

        for (int i = 0, l = sortedPlayers.Count; i < l; ++i)
        {
            var player = new DataSource($"{i}");
            var playerName = new DataProperty<string>("Name", sortedPlayers[i].PlayerName);
            var playerScore = new DataProperty<int>("Score", sortedPlayers[i].TotalScores);

            Players.AddNode(player);
            player.AddNode(playerName);
            player.AddNode(playerScore);
        }

        ContinueClick.SetAction(OnContinueClick);
    }

    private void OnContinueClick(IDataSource _)
    {
        _session.GameStateFsm.GoToState(new WorldMapState(_session));
    }
}
