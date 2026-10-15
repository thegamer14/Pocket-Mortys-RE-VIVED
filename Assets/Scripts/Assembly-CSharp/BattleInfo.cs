public class BattleInfo
{
	public eBattleOutcomeType outcomeType;

	public bool isTutorial;

	public bool isLevelingActive;

	public Node node;

	public BattleOpponent player;

	public BattleOpponent enemy;

	public BattleInfo(BattleOpponent player, BattleOpponent enemy, Node node = null, bool isTutorial = false)
	{
		outcomeType = eBattleOutcomeType.NONE;
		this.player = player;
		this.enemy = enemy;
		this.node = node;
		this.isTutorial = isTutorial;
		isLevelingActive = true;
	}
}
