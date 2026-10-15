public class BattleItemMove : BattleMove
{
	public BattleItemMove(string itemID, MortyData mortyData)
		: base(eBattleMoveType.ITEM)
	{
		id = itemID;
		base.mortyData = mortyData;
	}
}
