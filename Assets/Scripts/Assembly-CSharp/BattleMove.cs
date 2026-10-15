public class BattleMove
{
	public AttackData attackData;

	public string id;

	public MortyData mortyData;

	public int successfulHitsOnDefender;

	public bool isCritical;

	public float elementModifier;

	public int lastEffectDamage;

	public eBattleMoveType type { get; protected set; }

	public BattleMove(eBattleMoveType type)
	{
		this.type = type;
	}
}
