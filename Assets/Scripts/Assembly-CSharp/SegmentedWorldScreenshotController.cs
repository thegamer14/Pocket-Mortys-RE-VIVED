using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentedWorldScreenshotController : WorldController
{
	public string worldID;

	private CameraScreenshot _cameraScreenshoot;

	private void Start()
	{
		_segmentedWorldWidth = 9;
		_segmentedWorldDepth = 6;
		_worldAreaSize = _segmentedWorldWidth * _segmentedWorldDepth;
		_cameraScreenshoot = Camera.main.GetComponent<CameraScreenshot>();
		StartCoroutine(ScreenshotLoop("Summer", "Summer", 100));
	}

	private IEnumerator ScreenshotLoop()
	{
		for (int s = 0; s < WorldDefs.SegmentedWorldIDCount; s++)
		{
			string worldID = WorldDefs.GetSegmentedWorldID(s);
			string[] nodeSetIDs = WorldDefs.NodeSetIDs(worldID);
			for (int n = 0; n < nodeSetIDs.Length; n++)
			{
				ResetForNextWorld();
				LoadNewWorldSegment(worldID, nodeSetIDs[n]);
				yield return new WaitForEndOfFrame();
				_cameraScreenshoot.Take(string.Format("{0} - {1}", worldID, nodeSetIDs[n]));
				yield return new WaitForEndOfFrame();
			}
		}
	}

	private IEnumerator ScreenshotLoop(string worldID, string nodeSetID, int times)
	{
		for (int i = 0; i < times; i++)
		{
			ResetForNextWorld();
			LoadNewWorldSegment(worldID, nodeSetID);
			yield return new WaitForEndOfFrame();
			_cameraScreenshoot.Take(string.Format("{0} - {1} ({2})", worldID, nodeSetID, i));
			yield return new WaitForEndOfFrame();
		}
	}

	private void LoadNewWorldSegment(string worldID, string nodeSetID)
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
		LoadResources(worldID, nodeSetID);
		CreateSegments(worldID);
		CreateAllNodesFromAllSegments();
	}

	private void ResetForNextWorld()
	{
		_nodeLimits = WorldDefs.GetLimits(worldID, _worldAreaSize);
		_nodeCounts = WorldDefs.GetEmptyLimitDict();
		_usedTrainerIDs = new List<string>();
		for (int i = 0; i < _segments.Count; i++)
		{
			Object.Destroy(_segments[i].gameObject);
		}
		_segments.Clear();
	}
}
