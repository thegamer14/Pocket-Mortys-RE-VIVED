public class MPMortydexEntryInfo : MPStateInfo
{
	public string selectedMortyId;

	public MPMortydexEntryInfo(eMPStateOrigin origin, string selectedMortyID)
		: base(origin)
	{
		selectedMortyId = selectedMortyID;
	}
}
