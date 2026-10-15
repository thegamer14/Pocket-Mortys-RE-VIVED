public class BattleSwitchMove : BattleMove
{
	public BattleSwitchMove(MortyData switchMortyData)
		: base(eBattleMoveType.SWITCH)
	{
		mortyData = switchMortyData;
	}
}
