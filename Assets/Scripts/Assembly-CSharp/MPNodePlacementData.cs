using System.Collections.Generic;

public class MPNodePlacementData
{
	public const string kDictKeyCoord = "coord";

	public const string kDictKeyDirection = "direction";

	public const string kDictKeyCoordHistory = "coord_history";

	public const string kDictKeyDirectionHistory = "direction_history";

	public bool walkIn;

	public Coord coord;

	public eDirection direction;

	public Coord[] coordHistory;

	public eDirection[] directionHistory;

	public MPNodePlacementData()
	{
		walkIn = false;
		coord = new Coord(0, 0);
		direction = eDirection.DOWN;
		coordHistory = new Coord[0];
		directionHistory = new eDirection[0];
	}

	public MPNodePlacementData(Coord coord, eDirection direction, Coord[] coordHistory, eDirection[] directionHistory)
		: this()
	{
		this.coord = coord;
		this.direction = ((direction == eDirection.NONE) ? eDirection.DOWN : direction);
		this.coordHistory = ((coordHistory == null) ? new Coord[0] : coordHistory);
		this.directionHistory = ((directionHistory == null) ? new eDirection[0] : directionHistory);
	}

	public MPNodePlacementData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict);
	}

	public void SetFromDict(Dictionary<string, object> dict)
	{
		if (dict == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dict)
		{
			switch (item.Key)
			{
			case "coord":
			{
				int[] array = (int[])item.Value;
				coord = new Coord(array[0], array[1]);
				break;
			}
			}
		}
	}
}
