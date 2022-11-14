using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfo
{
	public readonly string Guid;
	public readonly string CharacterId;
	public readonly string PlayerName;
	public readonly int JumpKey;

	public event Action OnJump;
	public event Action<int> OnLifeChange;
	public event Action<PlayerInfo> OnFinish;

	public int Scores { get; private set; }
	public int TotalScores { get; private set; }
	public bool IsFinished { get; private set; }
	public int Lifes { get; private set; }

	public void AddScores(int scores, int maxScore)
	{
		if (IsFinished)
		{
			return;
		}

		Scores += scores;
		TotalScores += scores;

		if (Scores > maxScore)
		{
			Scores = 0;
			Lifes++;

			OnLifeChange?.Invoke(+1);
		}
	}

	public void Jump()
	{
		if (!IsFinished)
		{
			OnJump?.Invoke();
		}
	}

	public bool Finish()
	{
		if (!IsFinished)
		{
			Lifes--;

			OnLifeChange?.Invoke(-1);

			if (Lifes < 1)
			{
				IsFinished = true;

				OnFinish?.Invoke(this);

				return true;
			}
		}

		return false;
	}

	public PlayerInfo(string guid, string characterId, string playerName, int jumpKey) 
	{
		Guid = guid;
		CharacterId = characterId;
		PlayerName = playerName;
		JumpKey = jumpKey;
		Lifes = 1;
	}
}

public struct PlayersCollection : IEnumerable<PlayerInfo>
{
	private readonly Dictionary<string, PlayerInfo> _players;

	public PlayersCollection(Dictionary<string, PlayerInfo> players)
	{
		_players = players;
	}

	public IEnumerator<PlayerInfo> GetEnumerator()
	{
		foreach (var i in _players)
		{
			yield return i.Value;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}