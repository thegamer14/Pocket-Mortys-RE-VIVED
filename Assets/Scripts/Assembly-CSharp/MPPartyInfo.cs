public class MPPartyInfo : MPStateInfo
{
	public MPOpponentMortyData battleOutMortyData;

	public MPOpponentMortyData selectedMortyData;

	public string selectedItemId;

	public bool forceSwitch;

	public eUseItemError serverError;

	public PlayerUseItemResponse serverResponse;

	public MPPartyInfo(eMPStateOrigin origin, MPOpponentMortyData battleOutMortyData = null, string selectedItemId = null, bool forceSwitch = false)
		: base(origin)
	{
		this.battleOutMortyData = battleOutMortyData;
		this.selectedItemId = selectedItemId;
		this.forceSwitch = forceSwitch;
	}
}
