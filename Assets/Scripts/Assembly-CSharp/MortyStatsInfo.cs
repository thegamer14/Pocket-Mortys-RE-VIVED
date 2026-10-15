public class MortyStatsInfo
{
	public MortyData selectedMortyData;

	public bool hideReleaseBtn;

	public MortyStatsInfo(MortyData selectedMortyData, bool hideReleaseBtn = false)
	{
		this.selectedMortyData = selectedMortyData;
		this.hideReleaseBtn = hideReleaseBtn;
	}
}
