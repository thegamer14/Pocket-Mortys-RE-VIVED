public class MPWorldMapInfo : MPStateInfo
{
	public string worldId;

	public MPWorldMapInfo(eMPStateOrigin origin, string worldId)
		: base(origin)
	{
		this.worldId = worldId;
	}
}
