using System.Collections.Generic;
using UnityEngine;

public class SegmentedWorldPartTestController : WorldController
{
	public string worldID;

	public int rotation;

	private void Start()
	{
		_availableSignPostIDs = SignPostDefs.GetListOfRandomPooledIDs(true);
		_availableRareMortyID = ((!(Random.value < 0.3f)) ? null : MortyDefs.GetRandomID(new RandomMortyParams
		{
			badgeLevelLock = true,
			rarityLock = eMortyRarityCheck.RARE
		}));
		_availableMortyIDs = MortyDefs.GetIDListsForWorld();
		_availableQuestIDs = PlayerDefs.GetAvailableQuestIDs();
		_availableNPCIDs = NPCDefs.GetListOfRandomNPCsForWorld(worldID, QuestDefs.ConvertGuestIDListToAssetIds(_availableQuestIDs));
		_nodeLimits = new Dictionary<ePlaceholderType, int>();
		_nodeCounts = WorldDefs.GetEmptyLimitDict();
		LoadResources(worldID, worldID);
		CreateTestSegments();
		CreateAllNodesFromAllSegments();
	}

	private void CreateTestSegments()
	{
		int num = 0;
		for (int i = 0; i < _segmentResourcePool.eSegmentTypeCount; i++)
		{
			int num2 = 0;
			eSegmentType eSegmentType2 = (eSegmentType)i;
			for (int j = 0; j < _segmentResourcePool.eSegmentJointTypeCount; j++)
			{
				eSegmentJointType jointType = (eSegmentJointType)j;
				int segmentTypeCount = _segmentResourcePool.GetSegmentTypeCount(eSegmentType2, jointType);
				for (int k = 0; k < segmentTypeCount; k++)
				{
					SegmentedSegmentInfo info = new SegmentedSegmentInfo(eSegmentType2, jointType, rotation);
					CreateSegment(info, num2, num, 17f, 17f);
					num2++;
				}
				if (segmentTypeCount > 0 && eSegmentType2 == eSegmentType.DEFAULT)
				{
					num++;
					num2 = 0;
				}
			}
			if (eSegmentType2 > eSegmentType.DEFAULT)
			{
				num++;
			}
		}
	}
}
