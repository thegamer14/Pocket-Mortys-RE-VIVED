using System;
using UnityEngine;

public class StoryState : StateBase
{
	public string storyIDToLoad;

	private StoryUI _storyUI;

	private StoryController _storyController;

	private Action<eBattleOutcomeType> _postBattleCallback;

	public override void Awake()
	{
		base.Awake();
		storyIDToLoad = "GarageIntro";
		_uiResourcePath = string.Format("UI/Story/{0}", "Story");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (PlayerDefs.IsCouncilStoryComplete())
		{
			storyIDToLoad = "CouncilChambersComplete";
		}
		else if (storyIDToLoad.Equals("CouncilChambers") && PlayerDefs.NextCouncilMember == 6 && PlayerDefs.IsNextCouncilMemberReadyToBattle())
		{
			storyIDToLoad = "CouncilChambersFinalBattle";
		}
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	public override void Cache(TransInfo transOutInfo)
	{
		base.Cache(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0.5f);
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		if (SharedStatInfo.battleInfo != null && _postBattleCallback != null)
		{
			_postBattleCallback(SharedStatInfo.battleInfo.outcomeType);
			_postBattleCallback = null;
		}
		PlayerDefs.CheckForNextAlert();
		SharedStatInfo.battleInfo = null;
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0.5f);
	}

	protected override void Create()
	{
		base.Create();
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load(string.Format("Story/{0}", storyIDToLoad))) as GameObject;
		_storyController = gameObject.GetComponent<StoryController>();
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_storyUI = _ui as StoryUI;
	}

	protected override void Destroy()
	{
		UnityEngine.Object.Destroy(_storyController.gameObject);
		base.Destroy();
		_storyUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_storyController.StartDialogueEvent += StoryControllerStartDialogueHandler;
		_storyController.StartBattleEvent += StoryControllerStartBattleHandler;
		_storyController.StartStoryEvent += StoryControllerStartStoryHandler;
		_storyController.StartWorldEvent += StoryControllerStartWorldHandler;
		_storyUI.DialogueStartedEvent += UIDialogueStartedHandler;
		_storyUI.DialogueFinishedEvent += UIDialogueFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storyController.StartDialogueEvent -= StoryControllerStartDialogueHandler;
		_storyController.StartBattleEvent -= StoryControllerStartBattleHandler;
		_storyController.StartStoryEvent -= StoryControllerStartStoryHandler;
		_storyController.StartWorldEvent -= StoryControllerStartWorldHandler;
		_storyUI.DialogueStartedEvent -= UIDialogueStartedHandler;
		_storyUI.DialogueFinishedEvent -= UIDialogueFinishedHandler;
	}

	private void StoryControllerStartStoryHandler(string id)
	{
		PlayerDefs.SetStorySeen(storyIDToLoad);
		App.saveData.Save(eSaveDataType.PLAYER);
		App.states.Get<StoryState>().storyIDToLoad = id;
		App.states.Set<StoryState>(new FadeTransInfo(), new FadeTransInfo());
	}

	private void StoryControllerStartWorldHandler(string id, string entranceID)
	{
		PlayerDefs.SetStorySeen(storyIDToLoad);
		App.saveData.Save(eSaveDataType.PLAYER);
		SharedStatInfo.worldInfo.worldID = id;
		SharedStatInfo.worldInfo.entranceID = entranceID;
		App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo());
	}

	private void StoryControllerStartDialogueHandler(DialogueData dialogueData, Action<AdvancedNode> callback)
	{
		_storyUI.DisplayDialogueSpeech(dialogueData, null, callback);
	}

	private void StoryControllerStartBattleHandler(TrainerNode trainerNode, bool isTutorial, Action<eBattleOutcomeType> callback)
	{
		_postBattleCallback = callback;
		BattleOpponent player = new BattleOpponent(eBattleOpponentType.PLAYER, eBattleOpponentSide.LEFT, PlayerDefs.AssetID, null, PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems());
		BattleOpponentAI enemy = new BattleOpponentAI(eBattleOpponentType.AI_TRAINER, eBattleOpponentSide.RIGHT, trainerNode.trainerData.morties, trainerNode.trainerData.items, trainerNode.trainerData);
		SharedStatInfo.battleInfo = new BattleInfo(player, enemy, trainerNode, isTutorial);
		App.states.Set<BattleState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIDialogueStartedHandler()
	{
	}

	private void UIDialogueFinishedHandler()
	{
	}
}
