using System.Collections;
using UnityEngine;

public class CouncilChambersFinalBattleStory : StoryController
{
	private const string _kCouncilMember1BoundAssetID = "RickCouncil1Bound";

	private const string _kMysteriousRickCaughtAssetID = "RickMysteriousCaught";

	private PlayerNode _playerNode;

	private TrainerNode _trainerNode;

	private NPCNode _guard1Node;

	private NPCNode _guard2Node;

	private GameObject _councilMembers;

	private GameObject _councilMembersBound;

	private string _councilMember1ID;

	private string _councilMember6ID;

	protected override void Start()
	{
		base.Start();
		_storyCamera = Camera.main.GetComponent<StoryCamera>();
		_storyCamera.transform.position = new Vector3(20.5f, 20.5f, -3.5f);
		_playerNode = base.transform.FindChild("Player").GetComponent<PlayerNode>();
		_playerNode.SetUp(new NodeData(PlayerDefs.AssetID), eNodeInitialDirection.UP, false);
		_playerNode.SetUpMorties(Resources.Load(string.Format("World/Nodes/{0}/", "Default") + "Morty"));
		_trainerNode = base.transform.FindChild("Trainer").GetComponent<TrainerNode>();
		_trainerNode.SetUp(TrainerDefs.GetNewTrainerData("TrainerCouncilMysteriousRick"), eNodeInitialDirection.UP, false);
		_trainerNode.ActivateSightInteractionPoint(false);
		_councilMembers = base.transform.FindChild("CouncilMembers").gameObject;
		_councilMembersBound = base.transform.FindChild("CouncilMembersBound").gameObject;
		_councilMembers.SetActive(false);
		_guard1Node = base.transform.FindChild("Guard1").GetComponent<NPCNode>();
		_guard2Node = base.transform.FindChild("Guard2").GetComponent<NPCNode>();
		_guard1Node.SetUp(new NodeData("NPCRickChambersGuard"), eNodeInitialDirection.DOWN, false);
		_guard2Node.SetUp(new NodeData("NPCRickChambersGuard"), eNodeInitialDirection.DOWN, false);
		_guard1Node.gameObject.SetActive(false);
		_guard2Node.gameObject.SetActive(false);
		_councilMember1ID = string.Format("TrainerCouncil{0}", 1);
		_councilMember6ID = string.Format("TrainerCouncil{0}", 6);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Chambers/RaM203m02.DerelictShip.01.00.33.05.Ver1.0.0");
		StartCoroutine(AnimateIn());
	}

	private IEnumerator AnimateIn()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.UP, 12));
		yield return StartCoroutine(_storyCamera.MoveToZ(7f, 3f));
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[1]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_0_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateMysteriousRickBattle());
		});
	}

	private IEnumerator AnimateMysteriousRickBattle()
	{
		_trainerNode.SetDirection(eDirection.DOWN);
		yield return new WaitForSeconds(1f);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[1]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_1_TEXT_1, TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id), eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			AnimatePreBattle();
		});
	}

	private void AnimatePreBattle()
	{
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[7]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_2_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_2_TEXT_2, TrainerDefs.Name(_councilMember1ID), "RickCouncil1Bound", eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_2_TEXT_3, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_2_TEXT_4, TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_2_TEXT_5, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_2_TEXT_6, MortyDefs.Name("MortyDefault"), MortyDefs.AssetID("MortyDefault"), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_2_TEXT_7, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			StartBattle(_trainerNode, false, delegate(eBattleOutcomeType state)
			{
				AnimatePostBattle(state);
			});
		});
	}

	private void AnimatePostBattle(eBattleOutcomeType outcomeType)
	{
		_councilMembersBound.SetActive(false);
		_councilMembers.SetActive(true);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Chambers/RaM203m07.GoSonGo.01.05.36.00.Ver1.0", 1f, 1f);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[3]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_1, TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_2, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_3, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateGuardsIn());
		});
	}

	private IEnumerator AnimateGuardsIn()
	{
		_guard1Node.gameObject.SetActive(true);
		_guard2Node.gameObject.SetActive(true);
		StartCoroutine(_guard1Node.ForceMoveInDirectionFor(eDirection.UP, 7));
		yield return StartCoroutine(_guard2Node.ForceMoveInDirectionFor(eDirection.UP, 7));
		_guard1Node.SetDirection(eDirection.LEFT);
		_guard2Node.SetDirection(eDirection.RIGHT);
		_trainerNode.SetNewAssetID("RickMysteriousCaught");
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[7]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_4, TrainerDefs.Name(_councilMember1ID), TrainerDefs.AssetID(_councilMember1ID), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_5, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_6, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_7, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_8, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_9, TrainerDefs.Name(_trainerNode.data.id), "RickMysteriousCaught", eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_3_TEXT_10, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateTakeCape());
		});
	}

	private IEnumerator AnimateTakeCape()
	{
		NodeAIFollowModule firstMortyFollowModule = _playerNode.GetFollowingMorties()[0].GetComponent<NodeAIFollowModule>();
		firstMortyFollowModule.enabled = false;
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.UP, 1));
		yield return new WaitForSeconds(0.5f);
		PlayerDefs.AvatarID = "AvatarRickSashAndCape";
		_playerNode.SetUp(new NodeData(PlayerDefs.AssetID), eNodeInitialDirection.UP, false);
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.DOWN, 1));
		firstMortyFollowModule.enabled = true;
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[5]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_2, MortyDefs.Name("MortyDefault"), MortyDefs.AssetID("MortyDefault"), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_3, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_4, MortyDefs.Name("MortyDefault"), MortyDefs.AssetID("MortyDefault"), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_5, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateOut());
		});
	}

	private void AnimateMorty()
	{
		PlayerDefs.AvatarID = "AvatarRickSashAndCape";
		_playerNode.SetUp(new NodeData(PlayerDefs.AssetID), eNodeInitialDirection.DOWN, false);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[5]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_2, MortyDefs.Name("MortyDefault"), MortyDefs.AssetID("MortyDefault"), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_3, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_4, MortyDefs.Name("MortyDefault"), MortyDefs.AssetID("MortyDefault"), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_ENDING_PHASE_4_TEXT_5, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateOut());
		});
	}

	private IEnumerator AnimateOut()
	{
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.DOWN, 8));
		App.sound.StopSoundtrack(1f);
		StartWorld("Citadel", "CitadelCouncilChambers");
	}
}
