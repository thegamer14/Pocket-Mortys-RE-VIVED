using System.Collections;
using UnityEngine;

public class CouncilChambersCompleteStory : StoryController
{
	private const string _kCouncilMember6NoSashAssetID = "RickCouncil6NoSash";

	private PlayerNode _playerNode;

	private NPCNode _guard1Node;

	private NPCNode _guard2Node;

	private string _councilMember6ID;

	protected override void Start()
	{
		base.Start();
		_storyCamera = Camera.main.GetComponent<StoryCamera>();
		_storyCamera.transform.position = new Vector3(20.5f, 20.5f, -3.5f);
		_guard1Node = base.transform.FindChild("Guard1").GetComponent<NPCNode>();
		_guard2Node = base.transform.FindChild("Guard2").GetComponent<NPCNode>();
		_guard1Node.SetUp(new NodeData("NPCRickChambersGuard"), eNodeInitialDirection.DOWN, false);
		_guard2Node.SetUp(new NodeData("NPCRickChambersGuard"), eNodeInitialDirection.DOWN, false);
		_playerNode = base.transform.FindChild("Player").GetComponent<PlayerNode>();
		_playerNode.SetUp(new NodeData(PlayerDefs.AssetID), eNodeInitialDirection.UP, false);
		_playerNode.SetUpMorties(Resources.Load(string.Format("World/Nodes/{0}/", "Default") + "Morty"));
		_councilMember6ID = string.Format("TrainerCouncil{0}", 6);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Chambers/RaM203m07.GoSonGo.01.05.36.00.Ver1.0", 1f, 1f);
		StartCoroutine(AnimateIn());
	}

	public IEnumerator AnimateIn()
	{
		StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.UP, 12));
		yield return StartCoroutine(_storyCamera.MoveToZ(8.5f, 3f));
		string dialogueText = ((PlayerDefs.MortyCaughtCount < MortyDefs.idCount) ? TextDefs.STORY_COUNCIL_CHAMBERS_POST_CRED_PHASE_1_TEXT_1 : TextDefs.STORY_COUNCIL_CHAMBERS_POST_CRED_PHASE_2_TEXT_1);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[1]
		{
			new DialogueDataEntry(dialogueText, TrainerDefs.Name(_councilMember6ID), "RickCouncil6NoSash", eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateOut());
		});
	}

	public IEnumerator AnimateOut()
	{
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.DOWN, 8));
		StartWorld("Citadel", "CitadelCouncilChambers");
	}
}
