using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WorldTutorialController : WorldController
{
	private const string _kDialogueIDTutorialStart = "DialogueIDTutorialStart";

	private const string _kDialogueIDTutorialItemSee = "DialogueIDTutorialItemSee";

	private const string _kDialogueIDTutorialMortySee = "DialogueIDTutorialMortySee";

	private const string _kDialogueIDTutorialMortyCapture = "DialogueIDTutorialMortyCapture";

	private const string _kDialogueIDTutorialUseMpad = "DialogueIDTutorialUseMpad";

	private Node _mortySignPost;

	public override void SetUp(string worldID, string nodeSetID, string entranceID)
	{
		base.SetUp(worldID, nodeSetID, entranceID);
		App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.DIMENSION, "begin_scene");
	}

	protected override void CreateSegments(string worldID)
	{
		SegmentedWorldGenerator segmentedWorldGenerator = new SegmentedWorldGenerator();
		SegmentedSegmentInfo[,] array = segmentedWorldGenerator.Generate(2, 2, true);
		for (int i = 0; i < array.GetLength(1); i++)
		{
			for (int j = 0; j < array.GetLength(0); j++)
			{
				if (array[j, i].jointType == eSegmentJointType.EDGE || array[j, i].jointType == eSegmentJointType.EDGE_CORNER)
				{
					CreateSegment(array[j, i], j, array.GetLength(1) - 1 - i, WorldDefs.SegmentWidth(worldID), WorldDefs.SegmentDepth(worldID));
				}
			}
		}
		SegmentedSegmentInfo info = new SegmentedSegmentInfo(eSegmentType.SPAWN, eSegmentJointType.ONE_WAY, 270f);
		CreateSegment(info, 1, 1, WorldDefs.SegmentWidth(worldID), WorldDefs.SegmentDepth(worldID));
		SegmentedSegmentInfo info2 = new SegmentedSegmentInfo("CornerMorty", eSegmentType.DEFAULT, eSegmentJointType.CORNER, 0f);
		CreateSegment(info2, 1, 2, WorldDefs.SegmentWidth(worldID), WorldDefs.SegmentDepth(worldID));
		SegmentedSegmentInfo info3 = new SegmentedSegmentInfo("CornerTrainer", eSegmentType.DEFAULT, eSegmentJointType.CORNER, 90f);
		CreateSegment(info3, 2, 2, WorldDefs.SegmentWidth(worldID), WorldDefs.SegmentDepth(worldID));
		SegmentedSegmentInfo info4 = new SegmentedSegmentInfo(eSegmentType.GYM, eSegmentJointType.ONE_WAY, 270f);
		CreateSegment(info4, 2, 1, WorldDefs.SegmentWidth(worldID), WorldDefs.SegmentDepth(worldID));
	}

	protected override Node CreateNode(string resourceName, NodeData data, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		Node node = base.CreateNode(resourceName, data, parent, position, initialDirection, applyRotation);
		if (data != null && data.id.Equals("SignPostTutorialMorty"))
		{
			_mortySignPost = node;
		}
		return node;
	}

	protected override IEnumerator CreatePlayerNodeCo(Vector3 position, eDirection direction, int distance, Vector3[] mortyPositions = null, eDirection[] mortyDirections = null)
	{
		yield return StartCoroutine(_003CCreatePlayerNodeCo_003E__BaseCallProxy0(position, direction, distance, mortyPositions, mortyDirections));
		if (distance > -1)
		{
			DisplayDialogue("DialogueIDTutorialStart");
		}
	}

	protected override Node CreateGymNodeFromPlaceholder(PlaceholderNode placeholder)
	{
		TrainerData newTrainerData = TrainerDefs.GetNewTrainerData("TrainerTutorialGymLeader");
		newTrainerData.type = eTrainerType.GYM_LEADER;
		return CreateGymNode(placeholder.data, newTrainerData, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
	}

	public void WildMortyCaught()
	{
		if (_mortySignPost != null)
		{
			Object.Destroy(_mortySignPost.gameObject);
			_mortySignPost = null;
		}
		App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.DIMENSION, "end_morty_battle");
	}

	public void WildMortyCaughtShowDialogue()
	{
		DisplayDialogue("DialogueIDTutorialMortyCapture");
	}

	public override void RefreshGymLeaderDefeated()
	{
		base.RefreshGymLeaderDefeated();
		PlayerDefs.DisplayNotification(eNotificationType.ALERT, "AlertTutorialGoThroughPortalIDKey");
	}

	public override void DisplayDialogue(string dialogueID)
	{
		base.DisplayDialogue(dialogueID);
		if (dialogueID.Equals("DialogueIDTutorialUseMpad"))
		{
			PlayerDefs.DisplayNotification(eNotificationType.ALERT, "AlertTutorialUseMPadID");
		}
	}

	public override DialogueData GetDialogueData(string id)
	{
		if (id.Equals("DialogueIDTutorialStart"))
		{
			return new DialogueData(new DialogueDataEntry[1]
			{
				new DialogueDataEntry(string.Format(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_1_TEXT_1, TrainerDefs.Name("TrainerTutorialGymLeader")), TextDefs.PLAYER_NAME, _player.data.id)
			});
		}
		if (id.Equals("DialogueIDTutorialItemSee"))
		{
			return new DialogueData(new DialogueDataEntry[2]
			{
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_2_TEXT_1, MortyDefs.Name("MortyDefault"), "MortyDefault", eDirection.RIGHT),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_2_TEXT_2, TextDefs.PLAYER_NAME, _player.data.id)
			});
		}
		if (id.Equals("DialogueIDTutorialMortySee"))
		{
			return new DialogueData(new DialogueDataEntry[4]
			{
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_3_TEXT_1, MortyDefs.Name("MortyDefault"), "MortyDefault", eDirection.RIGHT),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_3_TEXT_2, TextDefs.PLAYER_NAME, _player.data.id),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_3_TEXT_3, MortyDefs.Name("MortyDefault"), "MortyDefault", eDirection.RIGHT),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_3_TEXT_4, TextDefs.PLAYER_NAME, _player.data.id)
			});
		}
		if (id.Equals("DialogueIDTutorialMortyCapture"))
		{
			return new DialogueData(new DialogueDataEntry[6]
			{
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_4_TEXT_1, MortyDefs.Name("MortyDefault"), "MortyDefault", eDirection.RIGHT),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_4_TEXT_2, TextDefs.PLAYER_NAME, _player.data.id),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_4_TEXT_3, MortyDefs.Name("MortyDefault"), "MortyDefault", eDirection.RIGHT),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_4_TEXT_4, TextDefs.PLAYER_NAME, _player.data.id),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_4_TEXT_5, MortyDefs.Name("MortyDefault"), "MortyDefault", eDirection.RIGHT),
				new DialogueDataEntry(string.Format(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_4_TEXT_6, TrainerDefs.Name("TrainerTutorialGymLeader")), TextDefs.PLAYER_NAME, _player.data.id)
			});
		}
		if (id.Equals("DialogueIDTutorialUseMpad"))
		{
			return new DialogueData(new DialogueDataEntry[3]
			{
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_5_TEXT_1, TextDefs.PLAYER_NAME, _player.data.id),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_5_TEXT_2, MortyDefs.Name("MortyDefault"), "MortyDefault", eDirection.RIGHT),
				new DialogueDataEntry(TextDefs.WORLD_DIALOGUE_TUTORIAL_PHASE_5_TEXT_3, TextDefs.PLAYER_NAME, _player.data.id)
			});
		}
		return null;
	}

	protected override void PlayerInteractStartHandler(AdvancedNode node, bool isTrigger)
	{
		switch (node.type)
		{
		case eNodeType.ITEM_CRATE:
			App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.DIMENSION, "pick_up_chip");
			break;
		case eNodeType.PORTAL:
			App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.DIMENSION, "end_scene");
			break;
		}
		switch (node.data.id)
		{
		case "MortyScruffy":
			if (!isTrigger)
			{
				App.states.Get<BattleState>().battleTutorialStep = eBattleTutorialStep.T_2_0;
				App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.DIMENSION, "begin_morty_battle");
			}
			break;
		case "TrainerTutorial":
			App.states.Get<BattleState>().battleTutorialStep = eBattleTutorialStep.T_3_0;
			App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.DIMENSION, "begin_trainer_battle");
			break;
		case "TrainerTutorialGymLeader":
			App.states.Get<BattleState>().battleTutorialStep = eBattleTutorialStep.T_4_0;
			App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.DIMENSION, "begin_rick_battle");
			break;
		}
		base.PlayerInteractStartHandler(node, isTrigger);
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CCreatePlayerNodeCo_003E__BaseCallProxy0(Vector3 position, eDirection direction, int distance, Vector3[] mortyPositions, eDirection[] mortyDirections)
	{
		return base.CreatePlayerNodeCo(position, direction, distance, mortyPositions, mortyDirections);
	}
}
