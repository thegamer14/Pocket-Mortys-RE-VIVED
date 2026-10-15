using System.Collections;
using UnityEngine;

public class CouncilChambersIntroStory : StoryController
{
	private PlayerNode _playerNode;

	private PortalNode _portalNode;

	private NPCNode _guard1Node;

	private NPCNode _guard2Node;

	private string _councilMember6ID;

	protected override void Start()
	{
		base.Start();
		_storyCamera = Camera.main.GetComponent<StoryCamera>();
		_storyCamera.transform.position = new Vector3(20.5f, 20.5f, 3.5f);
		_playerNode = base.transform.FindChild("Player").GetComponent<PlayerNode>();
		_playerNode.SetUp(new NodeData(PlayerDefs.AssetID), eNodeInitialDirection.UP, false);
		_playerNode.gameObject.SetActive(false);
		_portalNode = base.transform.FindChild("Portal").GetComponent<PortalNode>();
		_guard1Node = base.transform.FindChild("Guard1").GetComponent<NPCNode>();
		_guard2Node = base.transform.FindChild("Guard2").GetComponent<NPCNode>();
		_guard1Node.SetUp(new NodeData("NPCRickChambersGuard"), eNodeInitialDirection.DOWN, false);
		_guard2Node.SetUp(new NodeData("NPCRickChambersGuard"), eNodeInitialDirection.DOWN, false);
		_councilMember6ID = string.Format("TrainerCouncil{0}", 6);
		App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.COUNCIL, "begin_scene");
		StartCoroutine(AnimateIn());
	}

	public IEnumerator AnimateIn()
	{
		yield return new WaitForSeconds(1f);
		_playerNode.gameObject.SetActive(true);
		yield return StartCoroutine(_playerNode.SetUpMortiesWithAnimation(Resources.Load(string.Format("World/Nodes/{0}/", "Default") + "Morty"), eDirection.DOWN));
		yield return new WaitForSeconds(0.4f);
		yield return StartCoroutine(_portalNode.Close());
		_portalNode.gameObject.SetActive(false);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Chambers/RaM203m02.DerelictShip.01.00.33.05.Ver1.0.0");
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[3]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_1_TEXT_1, MortyDefs.Name("MortyDefault"), MortyDefs.AssetID("MortyDefault"), eDirection.RIGHT),
			new DialogueDataEntry(string.Format(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_1_TEXT_2, TrainerDefs.Name("TrainerTutorialGymLeader")), TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_1_TEXT_3, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateToCouncil());
		});
	}

	public IEnumerator AnimateToCouncil()
	{
		StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.UP, 5));
		StartCoroutine(_guard1Node.ForceMoveInDirectionFor(eDirection.LEFT, 3));
		StartCoroutine(_guard2Node.ForceMoveInDirectionFor(eDirection.RIGHT, 3));
		yield return StartCoroutine(_storyCamera.MoveToZ(8.5f, 1.5f));
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[1]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_2_TEXT_1, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateTakeGunAway());
		});
	}

	public IEnumerator AnimateTakeGunAway()
	{
		yield return StartCoroutine(_guard1Node.ForceMoveInDirectionFor(eDirection.LEFT, 3));
		_playerNode.SetDirection(eDirection.RIGHT);
		yield return new WaitForSeconds(0.2f);
		yield return StartCoroutine(_guard1Node.ForceMoveInDirectionFor(eDirection.RIGHT, 3));
		_guard1Node.SetDirection(eDirection.DOWN);
		_guard2Node.SetDirection(eDirection.DOWN);
		_playerNode.SetDirection(eDirection.UP);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[1]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_3_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateExplain());
		});
	}

	public IEnumerator AnimateExplain()
	{
		yield return new WaitForSeconds(0.2f);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[10]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_1, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_2, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_3, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_4, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_5, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_6, MortyDefs.Name("MortyDefault"), MortyDefs.AssetID("MortyDefault")),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_7, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_8, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_9, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_4_TEXT_10, TrainerDefs.Name(_councilMember6ID), TrainerDefs.AssetID(_councilMember6ID), eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateLetsGo());
		});
	}

	public IEnumerator AnimateLetsGo()
	{
		StartCoroutine(_storyCamera.MoveToZ(1.5f, 1.5f));
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.DOWN, 6));
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.LEFT, 1));
		_playerNode.SetDirection(eDirection.RIGHT);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[1]
		{
			new DialogueDataEntry(TextDefs.STORY_COUNCIL_CHAMBERS_INTRO_PHASE_5_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateOut());
		});
	}

	public IEnumerator AnimateOut()
	{
		StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.DOWN, 8));
		StartCoroutine(_storyCamera.MoveToZ(-5.5f, 1.5f));
		App.sound.StopSoundtrack(1f);
		yield return new WaitForSeconds(1f);
		App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.COUNCIL, "end_scene");
		StartWorld("Citadel", "CitadelCouncilChambers");
	}
}
