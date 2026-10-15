public class MPBattleInfo : MPStateInfo
{
	public string battleType;

	public string backgroundId;

	public MPBattleOpponent player;

	public MPBattleOpponent enemy;

	public MPBattleInfo(string battleType, MPBattleOpponent player, MPBattleOpponent enemy, string backgroundId)
		: base(eMPStateOrigin.NONE)
	{
		this.battleType = battleType;
		this.backgroundId = backgroundId;
		this.player = player;
		this.enemy = enemy;
	}
}
