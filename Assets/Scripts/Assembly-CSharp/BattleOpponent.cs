using System.Collections.Generic;

public class BattleOpponent
{
	public string assetID;

	public string name;

	public string pronounYou;

	public string pronounYours;

	public eBattleOpponentType type;

	public eBattleOpponentSide side;

	public BattleTrainer trainer;

	public BattleMorty morty;

	public BattleMortyInfoBar mortyInfoBar;

	public List<MortyData> mortyDatas;

	public Dictionary<string, int> items;

	public MortyData currentMortyData;

	public List<MortyData> mortiesToRewardXP;

	public BattleMove move;

	public BattleOpponent(eBattleOpponentType type, eBattleOpponentSide side, string assetID, string name, List<MortyData> mortyDatas, Dictionary<string, int> items)
	{
		this.type = type;
		this.side = side;
		this.assetID = assetID;
		this.name = name;
		this.mortyDatas = mortyDatas;
		this.items = items;
		mortiesToRewardXP = new List<MortyData>();
		SetPronoun();
	}

	private void SetPronoun()
	{
		switch (type)
		{
		default:
			pronounYou = string.Empty;
			pronounYours = string.Empty;
			break;
		case eBattleOpponentType.PLAYER:
			pronounYou = TextDefs.PRONOUN_PLAYER;
			pronounYours = TextDefs.PRONOUN_PLAYERS;
			break;
		case eBattleOpponentType.AI_TRAINER:
			pronounYou = TextDefs.PRONOUN_TRAINER;
			pronounYours = TextDefs.PRONOUN_TRAINERS;
			break;
		case eBattleOpponentType.AI_WILD_MORTY:
			pronounYou = TextDefs.PRONOUN_WILD_MORTY;
			pronounYours = TextDefs.PRONOUN_WILD_MORTY;
			break;
		}
	}

	public virtual void SetEnemyMorty(MortyData mortyData)
	{
	}

	public void DebugLog()
	{
		currentMortyData.DebugLog();
	}
}
