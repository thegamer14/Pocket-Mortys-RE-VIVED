public class BattleAttackMove : BattleMove
{
	public BattleAttackMove(AttackData attackData)
		: base(eBattleMoveType.ATTACK)
	{
		base.attackData = attackData;
	}
}
