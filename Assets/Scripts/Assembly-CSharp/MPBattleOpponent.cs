public class MPBattleOpponent
{
	public MPOpponentData opponentData;

	public string pronounYou;

	public string pronounYours;

	public eBattleOpponentSide side;

	public BattleTrainer trainer;

	public BattleMorty morty;

	public MPBattleMortyInfoBar mortyInfoBar;

	public MPOpponentMortyData currentMortyData;

	public MPBattleOpponent(MPOpponentData opponentData, eBattleOpponentSide side)
	{
		this.opponentData = opponentData;
		this.side = side;
		SetPronoun();
	}

	private void SetPronoun()
	{
		switch (opponentData.type)
		{
		default:
			pronounYou = string.Empty;
			pronounYours = string.Empty;
			break;
		case eMPOpponentType.PLAYER:
			pronounYou = TextDefs.PRONOUN_PLAYER;
			pronounYours = TextDefs.PRONOUN_PLAYERS;
			break;
		case eMPOpponentType.USER:
			pronounYou = TextDefs.PRONOUN_TRAINER;
			pronounYours = TextDefs.PRONOUN_TRAINERS;
			break;
		case eMPOpponentType.WILD_MORTY:
			pronounYou = TextDefs.PRONOUN_WILD_MORTY;
			pronounYours = TextDefs.PRONOUN_WILD_MORTY;
			break;
		}
	}

	public MPOpponentMortyData GetMortyData(string ownedId)
	{
		return opponentData.GetMortyData(ownedId);
	}

	public bool IsTrainer()
	{
		return opponentData.type != eMPOpponentType.WILD_MORTY;
	}
}
