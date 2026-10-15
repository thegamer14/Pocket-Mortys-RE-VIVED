using System.Collections;
using UnityEngine;

public class CouncilChambersStory : StoryController
{
	private TrainerNode _trainerNode;

	private int _councilMemberNumber;

	private PlayerNode _playerNode;

	private NPCNode _guard1Node;

	private NPCNode _guard2Node;

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
		_councilMemberNumber = PlayerDefs.NextCouncilMember;
		_trainerNode = base.transform.FindChild("Trainer").GetComponent<TrainerNode>();
		_trainerNode.SetUp(TrainerDefs.GetNewTrainerData(PlayerDefs.NextCouncilMemberID), eNodeInitialDirection.FIXED_DOWN, false);
		_trainerNode.ActivateSightInteractionPoint(false);
		base.transform.FindChild("CouncilMembers/" + PlayerDefs.NextCouncilMember).gameObject.SetActive(false);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Chambers/RaM203m02.DerelictShip.01.00.33.05.Ver1.0.0");
		StartCoroutine(AnimateIn());
	}

	public IEnumerator AnimateIn()
	{
		StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.UP, 12));
		yield return StartCoroutine(_storyCamera.MoveToZ(8.5f, 3f));
		if (PlayerDefs.IsNextCouncilMemberReadyToBattle())
		{
			DialogueData dialogueData = new DialogueData(TrainerDefs.GetRandomPreBattleDialogue(_trainerNode.data.id), TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id));
			StartDialogue(dialogueData, delegate
			{
				PreFightSuccessDialogueComplete();
			});
		}
		else
		{
			DialogueData dialogueData2 = new DialogueData(string.Format(TextDefs.STORY_COUNCIL_CHAMBERS_BATTLE_FAIL, TrainerDefs.GetCouncilMemberBadgeRequirement(_councilMemberNumber)), TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id));
			StartDialogue(dialogueData2, delegate
			{
				PreFightFailDialogueComplete();
			});
		}
	}

	public IEnumerator AnimateOut()
	{
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.DOWN, 8));
		App.sound.StopSoundtrack(1f);
		if (_trainerNode.trainerData.isDefeated && App.saveData.player.councilMembersDefeated == 1)
		{
			PlayerDefs.DisplayNotification(eNotificationType.INFO, "InfoCouponKey");
			PlayerDefs.DisplayNotification(eNotificationType.INFO, "InfoUnlockMortyLabsKey");
		}
		else if (_trainerNode.trainerData.isDefeated && App.saveData.player.councilMembersDefeated == 2)
		{
			PlayerDefs.DisplayNotification(eNotificationType.INFO, "InfoUnlockTournamentsKey");
		}
		StartWorld("Citadel", "CitadelCouncilChambers");
	}

	private void PreFightSuccessDialogueComplete()
	{
		StartBattle(_trainerNode, false, delegate(eBattleOutcomeType state)
		{
			BattleComplete(state);
		});
	}

	private void PreFightFailDialogueComplete()
	{
		StartCoroutine(AnimateOut());
	}

	private void BattleComplete(eBattleOutcomeType outcomeType)
	{
		App.sound.PlaySoundtrack("Audio/Soundtrack/Chambers/RaM203m02.DerelictShip.01.00.33.05.Ver1.0.0");
		string text = string.Format(TrainerDefs.DialoguePostBattle(_trainerNode.data.id), TrainerDefs.GetCouncilMemberBadgeRequirement(_councilMemberNumber + 1));
		DialogueData dialogueData = new DialogueData(text, TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id));
		StartDialogue(dialogueData, delegate
		{
			PostFightDialogueComplete();
		});
	}

	private void PostFightDialogueComplete()
	{
		StartCoroutine(AnimateOut());
	}
}
