public class RewardData
{
	public eRewardType type;

	public string id;

	public float chance;

	public int value;

	public bool swappableForCoin;

	public RewardData(eRewardType type, string id, float chance, int value = 1, bool swappableForCoin = false)
	{
		this.type = type;
		this.id = id;
		this.chance = chance;
		this.value = value;
		this.swappableForCoin = swappableForCoin;
	}
}
