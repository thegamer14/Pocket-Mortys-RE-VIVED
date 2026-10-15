public class WorldInfo
{
	public string worldID;

	public string nodeSetID;

	public string entranceID;

	public WorldInfo()
	{
		worldID = PlayerDefs.lastNormalWorldID;
		entranceID = PlayerDefs.lastNormalWorldEntranceID;
		nodeSetID = null;
	}
}
