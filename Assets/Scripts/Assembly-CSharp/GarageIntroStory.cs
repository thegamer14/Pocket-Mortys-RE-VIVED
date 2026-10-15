using System.Collections;
using UnityEngine;

public class GarageIntroStory : StoryController
{
	private PlayerNode _playerNode;

	private MortyNode _playerMortyNode;

	private TrainerNode _trainerNode;

	private MortyNode _trainerMortyNode;

	private PortalNode _portalNode;

	protected override void Start()
	{
		base.Start();
		_storyCamera.transform.position = new Vector3(20f, 20.5f, 6f);
		_playerNode = base.transform.FindChild("Player").GetComponent<PlayerNode>();
		_playerNode.SetUp(new NodeData(PlayerDefs.AssetID), eNodeInitialDirection.LEFT, false);
		_playerMortyNode = base.transform.FindChild("PlayerMorty").GetComponent<MortyNode>();
		_playerMortyNode.SetUp(PlayerDefs.PlayerPartyGetMorty(0), eNodeInitialDirection.LEFT, false);
		_trainerNode = base.transform.FindChild("Trainer").GetComponent<TrainerNode>();
		_trainerNode.SetUp(TrainerDefs.GetNewTrainerData("TrainerGarageIntro"), eNodeInitialDirection.DOWN, false);
		_trainerNode.ActivateSightInteractionPoint(false);
		_trainerNode.gameObject.SetActive(false);
		_trainerMortyNode = base.transform.FindChild("TrainerMorty").GetComponent<MortyNode>();
		_trainerMortyNode.SetUp(_trainerNode.trainerData.morties[0], eNodeInitialDirection.DOWN, false);
		_trainerMortyNode.gameObject.SetActive(false);
		_portalNode = base.transform.FindChild("Portal").GetComponent<PortalNode>();
		_portalNode.gameObject.SetActive(false);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Garage/Do You Feel It - Tinny", 0.7f);
		App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.GARAGE, "begin_scene");
		StartCoroutine(AnimateIn());
	}

	public IEnumerator AnimateIn()
	{
		yield return StartCoroutine(_playerMortyNode.ForceMoveInDirectionFor(eDirection.LEFT, 2));
		yield return StartCoroutine(_playerMortyNode.ForceMoveInDirectionFor(eDirection.RIGHT, 4));
		yield return StartCoroutine(_playerMortyNode.ForceMoveInDirectionFor(eDirection.LEFT, 4));
		yield return StartCoroutine(_playerMortyNode.ForceMoveInDirectionFor(eDirection.RIGHT, 2));
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[2]
		{
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_1_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_1_TEXT_2, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id), eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateScrewdriver());
		});
	}

	public IEnumerator AnimateScrewdriver()
	{
		yield return StartCoroutine(_playerMortyNode.ForceMoveInDirectionFor(eDirection.RIGHT, 2));
		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(_playerMortyNode.ForceMoveInDirectionFor(eDirection.LEFT, 5));
		_playerMortyNode.SetDirection(eDirection.UP);
		_playerNode.SetDirection(eDirection.DOWN);
		_playerMortyNode.moveModule.movementSpeed = 4.5f;
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[6]
		{
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_2_TEXT_1, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_2_TEXT_2, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_2_TEXT_3, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_2_TEXT_4, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_2_TEXT_5, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_2_TEXT_6, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			AnimatePortal();
		});
	}

	public void AnimatePortal()
	{
		_portalNode.gameObject.SetActive(true);
		StartCoroutine(_portalNode.Open());
		_playerMortyNode.SetDirection(eDirection.RIGHT);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[1]
		{
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_3_TEXT_1, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id), eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateTrainerIn());
		});
	}

	public IEnumerator AnimateTrainerIn()
	{
		base.transform.FindChild("Meshes/mortyDeck").gameObject.SetActive(false);
		_playerNode.SetDirection(eDirection.RIGHT);
		App.sound.PlaySFX("Audio/SFX/PortalUse");
		_trainerNode.gameObject.SetActive(true);
		yield return StartCoroutine(_trainerNode.ForceMoveInDirectionFor(eDirection.DOWN, 1));
		_trainerNode.SetDirection(eDirection.LEFT);
		_trainerMortyNode.gameObject.SetActive(true);
		_trainerMortyNode.SetDirection(eDirection.LEFT);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[1]
		{
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_4_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			AnimateTrainerWantsDevice();
		});
	}

	public void AnimateTrainerWantsDevice()
	{
		StartCoroutine(_trainerNode.ForceMoveInDirectionFor(eDirection.LEFT, 1));
		StartCoroutine(_trainerMortyNode.ForceMoveInDirectionFor(eDirection.DOWN, 1));
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[4]
		{
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_5_TEXT_1, TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_5_TEXT_2, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_5_TEXT_3, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id)),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_5_TEXT_4, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			AnimateTrainerWantsMorty();
		});
	}

	public void AnimateTrainerWantsMorty()
	{
		StartCoroutine(_trainerNode.ForceMoveInDirectionFor(eDirection.LEFT, 1));
		StartCoroutine(_trainerMortyNode.ForceMoveInDirectionFor(eDirection.LEFT, 1));
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[3]
		{
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_6_TEXT_1, TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_6_TEXT_2, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id)),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_6_TEXT_3, TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id), eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateTrainerWantsBattle());
		});
	}

	public IEnumerator AnimateTrainerWantsBattle()
	{
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.DOWN, 1));
		_playerNode.SetDirection(eDirection.RIGHT);
		App.states.Get<BattleState>().battleTutorialStep = eBattleTutorialStep.T_1_0;
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[6]
		{
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_7_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_7_TEXT_2, TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_7_TEXT_3, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id)),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_7_TEXT_4, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_7_TEXT_5, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id)),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_7_TEXT_6, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			AnimatePreBattle();
		});
	}

	public void AnimatePreBattle()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.GARAGE, "begin_rick_battle");
		StartBattle(_trainerNode, true, delegate(eBattleOutcomeType state)
		{
			AnimatePostBattle(state);
		});
	}

	public void AnimatePostBattle(eBattleOutcomeType outcomeType)
	{
		App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.GARAGE, "end_rick_battle");
		App.sound.PlaySoundtrack("Audio/Soundtrack/Garage/Do You Feel It - Tinny", 0.7f);
		_playerMortyNode.RefreshStatus();
		_trainerMortyNode.RefreshStatus();
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[1]
		{
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_8_TEXT_1, TrainerDefs.Name(_trainerNode.data.id), TrainerDefs.AssetID(_trainerNode.data.id), eDirection.RIGHT)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateTrainerOut());
		});
	}

	public IEnumerator AnimateTrainerOut()
	{
		StartCoroutine(_trainerMortyNode.ForceMoveInDirectionFor(eDirection.LEFT, 1));
		yield return StartCoroutine(_trainerNode.ForceMoveInDirectionFor(eDirection.UP, 1));
		StartCoroutine(_trainerMortyNode.ForceMoveInDirectionFor(eDirection.UP, 1));
		yield return StartCoroutine(_trainerNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		StartCoroutine(_trainerMortyNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		yield return StartCoroutine(_trainerNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		App.sound.PlaySFX("Audio/SFX/PortalUse");
		_trainerNode.gameObject.SetActive(false);
		yield return StartCoroutine(_trainerMortyNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		_trainerMortyNode.gameObject.SetActive(false);
		_playerNode.SetDirection(eDirection.LEFT);
		DialogueData dialogueData = new DialogueData(new DialogueDataEntry[7]
		{
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_9_TEXT_1, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_9_TEXT_2, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_9_TEXT_3, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_9_TEXT_4, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_9_TEXT_5, TextDefs.PLAYER_NAME, _playerNode.data.id),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_9_TEXT_6, MortyDefs.Name(_playerMortyNode.data.id), MortyDefs.AssetID(_playerMortyNode.data.id), eDirection.RIGHT),
			new DialogueDataEntry(TextDefs.STORY_GARAGE_INTRO_PHASE_9_TEXT_7, TextDefs.PLAYER_NAME, _playerNode.data.id)
		});
		StartDialogue(dialogueData, delegate
		{
			StartCoroutine(AnimateOut());
		});
	}

	public IEnumerator AnimateOut()
	{
		StartCoroutine(_playerMortyNode.ForceMoveInDirectionFor(eDirection.RIGHT, 3));
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.RIGHT, 3));
		StartCoroutine(_playerMortyNode.ForceMoveInDirectionFor(eDirection.RIGHT, 1));
		yield return StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.UP, 1));
		_playerNode.gameObject.SetActive(false);
		App.sound.PlaySFX("Audio/SFX/PortalUse");
		yield return StartCoroutine(_playerMortyNode.ForceMoveInDirectionFor(eDirection.UP, 1));
		_playerMortyNode.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.6f);
		App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.GARAGE, "end_scene");
		StartWorld("Tutorial", "SegmentedWorldPortal");
	}
}
