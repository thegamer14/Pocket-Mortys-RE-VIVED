using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentArenaState : StateBase
{
	private WorldUI _worldUI;

	private TournamentArenaController _arenaController;

	private bool _transitionedIntoPopup;

	private bool _startWaitingForTransitionIn;

	private bool _eventRewardsActive;

	public bool leaveMusicPlayingOnCache;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/World/{0}", "World");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		if (!leaveMusicPlayingOnCache)
		{
			App.sound.PlaySoundtrack("Audio/Soundtrack/Tournament/Crowd", 1f, (transInInfo != null) ? transInInfo.duration : 0f);
		}
		leaveMusicPlayingOnCache = false;
		StartCoroutine(ActivateComplete());
	}

	public override void Cache(TransInfo transOutInfo)
	{
		base.Cache(transOutInfo);
		if (!leaveMusicPlayingOnCache)
		{
			App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0f);
		}
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		_arenaController.ShowWorld(true);
		if (!leaveMusicPlayingOnCache)
		{
			App.sound.PlaySoundtrack("Audio/Soundtrack/Tournament/Crowd", 1f, (transInInfo != null) ? transInInfo.duration : 0f);
		}
		_transitionedIntoPopup = false;
		leaveMusicPlayingOnCache = false;
		_arenaController.RefreshFromCache();
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		if (!leaveMusicPlayingOnCache)
		{
			App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0f);
		}
	}

	protected override void Create()
	{
		base.Create();
		GameObject gameObject = Object.Instantiate(Resources.Load(string.Format("Tournament/{0}", "TournamentArena"))) as GameObject;
		_arenaController = gameObject.GetComponent<TournamentArenaController>();
		_arenaController.SetUp(TournamentDefs.currentTrainerID);
		StartCoroutine(StartEntrance());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_worldUI = _ui as WorldUI;
		_worldUI.SetUp(false, false, false, false, false);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_worldUI = null;
	}

	protected override void DestroyCache()
	{
		base.DestroyCache();
		_arenaController.RefreshIntoCache();
		if (!_transitionedIntoPopup)
		{
			_arenaController.ShowWorld(false);
			App.sound.StopAllSFX();
		}
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_arenaController.PlayerInteractAvailableEvent += WorldPlayerInteractAvailableHandler;
		_arenaController.PlayerInteractUnavailableEvent += WorldPlayerInteractUnavailableHandler;
		_arenaController.PlayerInteractStartEvent += WorldPlayerInteractStartHandler;
		_arenaController.PlayerInteractEndEvent += WorldPlayerInteractEndHandler;
		_worldUI.PausePressedEvent += UIPausePressedHandler;
		_worldUI.InputDPadChangedEvent += UIInputDPadChangedHandler;
		_worldUI.InputFaceBtnAPressedEvent += UIInputFaceBtnAPressedHandler;
		_worldUI.DialogueStartedEvent += UIDialogueStartedHandler;
		_worldUI.DialogueFinishedEvent += UIDialogueFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_arenaController.PlayerInteractAvailableEvent -= WorldPlayerInteractAvailableHandler;
		_arenaController.PlayerInteractUnavailableEvent -= WorldPlayerInteractUnavailableHandler;
		_arenaController.PlayerInteractStartEvent -= WorldPlayerInteractStartHandler;
		_arenaController.PlayerInteractEndEvent -= WorldPlayerInteractEndHandler;
		_worldUI.PausePressedEvent -= UIPausePressedHandler;
		_worldUI.InputDPadChangedEvent -= UIInputDPadChangedHandler;
		_worldUI.InputFaceBtnAPressedEvent -= UIInputFaceBtnAPressedHandler;
		_worldUI.DialogueStartedEvent -= UIDialogueStartedHandler;
		_worldUI.DialogueFinishedEvent -= UIDialogueFinishedHandler;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (_startWaitingForTransitionIn)
		{
			StartCoroutine(InteractAntagonistPreBattle());
		}
		else if (SharedStatInfo.battleInfo != null)
		{
			TournamentTrainerNode tournamentTrainerNode = (TournamentTrainerNode)SharedStatInfo.battleInfo.node;
			tournamentTrainerNode.RefreshStatus();
			StartCoroutine(InteractTrainerPostBattle(tournamentTrainerNode));
		}
		else if (_eventRewardsActive)
		{
			StartCoroutine(EventFinished(true));
		}
		_startWaitingForTransitionIn = false;
		SharedStatInfo.battleInfo = null;
		_eventRewardsActive = false;
	}

	public IEnumerator StartEntrance()
	{
		if (TournamentDefs.DialogueAntag(TournamentDefs.currentTrainerID) != null)
		{
			_startWaitingForTransitionIn = true;
			StartCoroutine(_arenaController.MoveStoryCameraTo(new Vector3(10.5f, 20f, 2.5f), 0f));
		}
		else
		{
			yield return StartCoroutine(_arenaController.ForceWalkPlayer(eDirection.UP, 4));
			_worldUI.TweenIn();
			_worldUI.tweenInputOnActivate = true;
		}
	}

	public void InteractDefault(AdvancedNode node)
	{
		string id = node.data.id;
		if (id != null && id == "Exit")
		{
			InteractExit();
		}
	}

	public void InteractExit()
	{
		leaveMusicPlayingOnCache = true;
		_transitionedIntoPopup = true;
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(TextDefs.TOURNAMENT_ARENA_EXIT_DESC, new string[1] { TextDefs.TOURNAMENT_ARENA_EXIT_BTN });
		genericPopupState.ButtonClickedEvent += ExitPopupHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private IEnumerator InteractTrainerPreBattle(PlayerNode playerNode, TournamentTrainerNode trainerNode)
	{
		trainerNode.ActivateInteractionPoint(false);
		_worldUI.tweenInputOnActivate = false;
		_worldUI.TweenOut();
		StartCoroutine(trainerNode.TweenAttackIcon());
		yield return new WaitForSeconds(0.1f);
		if (playerNode.transform.position.x < trainerNode.transform.position.x)
		{
			yield return StartCoroutine(_arenaController.ForceWalkPlayer(eDirection.RIGHT, 1));
		}
		else if (playerNode.transform.position.x > trainerNode.transform.position.x)
		{
			yield return StartCoroutine(_arenaController.ForceWalkPlayer(eDirection.LEFT, 1));
		}
		yield return StartCoroutine(_arenaController.ForceWalkPlayer(eDirection.UP, 3));
		DialogueData preDialogueData = TournamentDefs.DialogueTrainerPre(TournamentDefs.currentTrainerID);
		if (preDialogueData != null)
		{
			_worldUI.DisplayDialogueSpeech(preDialogueData, null, delegate
			{
				InteractTrainerPreBattleComplete(trainerNode);
			});
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
			InteractTrainerPreBattleComplete(trainerNode);
		}
	}

	private void InteractTrainerPreBattleComplete(TournamentTrainerNode trainerNode)
	{
		BattleOpponent player = new BattleOpponent(eBattleOpponentType.PLAYER, eBattleOpponentSide.LEFT, PlayerDefs.AssetID, null, PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems());
		BattleOpponentAI enemy = new BattleOpponentAI(eBattleOpponentType.AI_TRAINER, eBattleOpponentSide.RIGHT, trainerNode.trainerData.morties, trainerNode.trainerData.items, trainerNode.trainerData);
		SharedStatInfo.battleInfo = new BattleInfo(player, enemy, trainerNode, SharedStatInfo.worldInfo.worldID == "Tutorial");
		SharedStatInfo.battleInfo.isLevelingActive = false;
		App.states.Set<BattleState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private IEnumerator InteractTrainerPostBattle(TournamentTrainerNode trainerNode)
	{
		DialogueData postDialogueData = TournamentDefs.DialogueTrainerPost(TournamentDefs.currentTrainerID);
		if (postDialogueData != null)
		{
			_worldUI.DisplayDialogueSpeech(postDialogueData, null, delegate
			{
				InteractTrainerPostBattleComplete(trainerNode);
			});
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
			InteractTrainerPostBattleComplete(trainerNode);
		}
	}

	private void InteractTrainerPostBattleComplete(TournamentTrainerNode trainerNode)
	{
		BattleFinished();
	}

	private IEnumerator InteractAntagonistPreBattle()
	{
		yield return StartCoroutine(_arenaController.MoveStoryCameraTo(new Vector3(10.5f, 20f, 9.5f), 1f));
		DialogueData antagDialogueData = TournamentDefs.DialogueAntag(TournamentDefs.currentTrainerID);
		if (antagDialogueData != null)
		{
			_worldUI.DisplayDialogueSpeech(antagDialogueData, null, delegate
			{
				StartCoroutine(InteractAntagonistPreBattleComplete());
			});
		}
		else
		{
			StartCoroutine(InteractAntagonistPreBattleComplete());
		}
	}

	private IEnumerator InteractAntagonistPreBattleComplete()
	{
		StartCoroutine(_arenaController.MoveStoryCameraTo(new Vector3(10.5f, 20f, -7.5f), 3f));
		yield return new WaitForSeconds(2.8f);
		yield return StartCoroutine(_arenaController.ForceWalkPlayer(eDirection.UP, 4));
		_arenaController.FollowCameraEnabled(true);
		_worldUI.TweenIn();
		_worldUI.tweenInputOnActivate = true;
	}

	private void InteractWithItemCrateNode(ItemCrateNode node)
	{
		Object.Destroy(node.gameObject);
		SharedStatInfo.rewardInfo = new RewardInfo(TournamentDefs.EventStashedItems(TournamentDefs.currentEventID), "tournament_item_crate");
		App.states.Set<RewardState>(new FadeTransInfo
		{
			duration = 0.05f,
			color = Color.white
		}, new FadeTransInfo
		{
			duration = 0.2f,
			color = Color.white
		}, true, eStateAction.ADD_CURRENT_TO_CACHE);
		TournamentDefs.SetItemStashActive(false);
		App.saveData.Save(eSaveDataType.TOURNAMENT);
		node.InteractionFinished();
	}

	private void BattleFinished()
	{
		string nextTrainerID = TournamentDefs.GetNextTrainerID(TournamentDefs.currentEventID, TournamentDefs.currentTrainerID);
		if (!string.IsNullOrEmpty(nextTrainerID))
		{
			if (TournamentDefs.DialogueAnnouncer(TournamentDefs.currentTrainerID) != null)
			{
				leaveMusicPlayingOnCache = true;
				App.states.Set<TournamentAnnouncerTableState>(new FadeTransInfo(), new FadeTransInfo());
			}
			else
			{
				TournamentDefs.SetNextTrainer(nextTrainerID);
				App.states.Set<TournamentArenaState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
			}
			return;
		}
		TournamentDefs.CompleteEvent();
		List<RewardData> list = TournamentDefs.EventRewards(TournamentDefs.currentEventID);
		if (list.Count > 0)
		{
			_eventRewardsActive = true;
			SharedStatInfo.rewardInfo = new RewardInfo(list, "tournament_event", PlayerDefs.playerItems);
			App.states.Set<RewardState>(new FadeTransInfo
			{
				duration = 0.05f,
				color = Color.white
			}, new FadeTransInfo
			{
				duration = 0.2f,
				color = Color.white
			}, true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else
		{
			StartCoroutine(EventFinished(true));
		}
	}

	private IEnumerator EventFinished(bool isSuccess)
	{
		StartCoroutine(_arenaController.ForceWalkPlayerOut(isSuccess));
		yield return new WaitForSeconds(1f);
		TournamentDefs.EndEvent();
		SharedStatInfo.worldInfo.worldID = "TournamentLobby";
		SharedStatInfo.worldInfo.entranceID = "TournamentLobbyOrganiserPortal";
		App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo());
	}

	public void RefreshPlayerMorties()
	{
		if (_arenaController != null)
		{
			_arenaController.RefreshPlayerMorties();
		}
	}

	public void ShowWorld(bool value)
	{
		if (_arenaController != null)
		{
			_arenaController.ShowWorld(value);
		}
	}

	private void UIPausePressedHandler()
	{
		_transitionedIntoPopup = true;
		leaveMusicPlayingOnCache = true;
		App.states.Set<PauseState>(null, new SlideTransInfo
		{
			direction = eDirection.DOWN,
			duration = 0.3f
		}, true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIInputDPadChangedHandler(eDirection direction)
	{
		_arenaController.DPadChanged(direction);
	}

	private void UIInputFaceBtnAPressedHandler()
	{
		_arenaController.FaceBtnAPressed();
	}

	private void UIDialogueStartedHandler()
	{
	}

	private void UIDialogueFinishedHandler()
	{
	}

	private void WorldPlayerInteractAvailableHandler()
	{
		_worldUI.SetInputForPlayerInteract(true);
	}

	private void WorldPlayerInteractUnavailableHandler()
	{
		_worldUI.SetInputForPlayerInteract(false);
	}

	private void WorldPlayerInteractStartHandler(PlayerNode playerNode, AdvancedNode node, bool isTrigger)
	{
		float angle = Mathf.Atan2(node.transform.position.z - playerNode.transform.position.z, playerNode.transform.position.x - node.transform.position.x) * 57.29578f;
		eDirection directionFromAngle = HelperMethods.GetDirectionFromAngle(angle);
		node.InteractionStarted(directionFromAngle, isTrigger);
		switch (node.type)
		{
		default:
			InteractDefault(node);
			break;
		case eNodeType.TOURNAMENT_TRAINER:
			StartCoroutine(InteractTrainerPreBattle(playerNode, (TournamentTrainerNode)node));
			break;
		case eNodeType.ITEM_CRATE:
			InteractWithItemCrateNode((ItemCrateNode)node);
			break;
		}
	}

	private void WorldPlayerInteractEndHandler(PlayerNode playerNode, AdvancedNode node)
	{
		node.InteractionFinished();
	}

	private void WorldDisplayDialogueHandler(DialogueData dialogueData)
	{
		_worldUI.DisplayDialogueSpeech(dialogueData);
	}

	private void ExitPopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= ExitPopupHandler;
		App.states.SetBack(new PopTransInfo());
		if (buttonIndex == 1)
		{
			_worldUI.tweenInputOnActivate = false;
			_arenaController.FollowCameraEnabled(false);
			StartCoroutine(EventFinished(false));
		}
	}
}
