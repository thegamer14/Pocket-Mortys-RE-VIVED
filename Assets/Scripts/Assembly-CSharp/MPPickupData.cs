using System;
using System.Collections.Generic;

public class MPPickupData
{
	public const string kDictKeyNetId = "pickup_id";

	public const string kDictKeyRewards = "contents";

	public const string kDictKeyPlacement = "placement";

	public string netId;

	public MPRewardData[] rewardDatas;

	public MPNodePlacementData placementData;

	public MPPickupData()
	{
		placementData = new MPNodePlacementData();
	}

	public MPPickupData(Dictionary<string, object> dict)
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
			case "pickup_id":
				netId = Convert.ToString(item.Value);
				break;
			case "contents":
				rewardDatas = MPRewardData.GetRewardDatasForDictArray(ConvertExt.ToDictionaryArray(item.Value));
				break;
			case "placement":
			{
				int[] array = (int[])item.Value;
				placementData = new MPNodePlacementData();
				placementData.coord = new Coord(array[0], array[1]);
				break;
			}
			}
		}
	}
}
