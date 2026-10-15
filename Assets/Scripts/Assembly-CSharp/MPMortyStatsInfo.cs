public class MPMortyStatsInfo : MPStateInfo
{
	public MPOpponentMortyData mortyData;

	public bool allowRelease;

	public MPMortyStatsInfo(eMPStateOrigin origin, MPOpponentMortyData selectedMortyData, bool allowRelease)
		: base(origin)
	{
		mortyData = selectedMortyData;
		this.allowRelease = allowRelease;
	}
}
