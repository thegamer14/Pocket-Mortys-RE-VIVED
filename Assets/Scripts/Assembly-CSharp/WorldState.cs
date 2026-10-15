using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldState : StateBase
{
	private WorldUI _worldUI;

	private WorldController _worldController;

	public bool leaveMusicPlayingOnCache;

	private bool _transitionedIntoPopup;

	private float _stopSoundtrackPos;

	private string _soundtrackName;

	private List<string> _soundtrackQueue;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/World/{0}", "World");
		base.showLoadingScreen = true;
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		PlayerDefs.CheckForNextAlert();
		if (PlayerDefs.GetMortyCombineCount() > 0)
		{
			PlayerDefs.DisplayNotification(eNotificationType.INFO, "InfoCombineKey");
		}
		SoundTrackFinishedHandler();
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transInfo)
	{
		base.Deactivate(transInfo);
		float fadeOutVal = ((transInfo == null) ? 0.5f : transInfo.duration);
		App.sound.StopSoundtrack(fadeOutVal);
	}

	public override void Cache(TransInfo transOutInfo)
	{
		base.Cache(transOutInfo);
		if (!leaveMusicPlayingOnCache)
		{
			PauseWorldSoundtrack();
		}
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		leaveMusicPlayingOnCache = false;
		_worldController.ShowWorld(true);
		if (SharedStatInfo.battleInfo != null)
		{
			UncacheFromBattle();
		}
		if (SharedStatInfo.bagInfo != null && SharedStatInfo.bagInfo.questGiverNode != null)
		{
			UncacheFromQuestRequest();
		}
		PlayerDefs.CheckForNextAlert();
		_worldController.RefreshFromCache();
		SharedStatInfo.battleInfo = null;
		AudioSource audioSource = App.sound.PlaySoundtrack(_soundtrackName, 0.2f, 0f, false);
		if ((bool)audioSource)
		{
			audioSource.time = _stopSoundtrackPos;
		}
		_transitionedIntoPopup = false;
	}

	private void UncacheFromBattle()
	{
		if (SharedStatInfo.battleInfo.node is TrainerNode)
		{
			TrainerNode trainerNode = (TrainerNode)SharedStatInfo.battleInfo.node;
			trainerNode.RefreshStatus();
			if (SharedStatInfo.battleInfo.outcomeType == eBattleOutcomeType.PLAYER_WON || SharedStatInfo.battleInfo.outcomeType == eBattleOutcomeType.WILD_MORTY_CAUGHT)
			{
				if (trainerNode.trainerData.type == eTrainerType.DEFAULT || trainerNode.trainerData.id == "TrainerTutorial")
				{
					_worldController.trainersRemainingCount--;
					if (_worldController.trainersRemainingCount <= 0)
					{
						_worldUI.SetHudTrainersObjectTicked();
					}
				}
				else if (trainerNode.trainerData.type == eTrainerType.GYM_LEADER)
				{
					_worldController.gymLeadersRemainingCount--;
					if (_worldController.gymLeadersRemainingCount <= 0)
					{
						_worldUI.SetHudGymLeadersObjectTicked();
					}
				}
			}
			if (SharedStatInfo.worldInfo.worldID.Equals("Tutorial"))
			{
				if (trainerNode.data.id == "TrainerTutorial")
				{
					App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.DIMENSION, "end_trainer_battle");
				}
				else if (trainerNode.data.id == "TrainerTutorialGymLeader")
				{
					App.upsight.CustomAnalyticEvent(eSubType1.TUTORIAL, eSubType2.DIMENSION, "end_rick_battle");
				}
			}
			else if (trainerNode.trainerData.type == eTrainerType.GYM_LEADER)
			{
				App.upsight.CustomAnalyticEvent(eSubType1.LEVEL, eSubType2.LEVEL_PROGRESSION, "badge_win", PlayerDefs.BadgeCount);
				App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.RICK_BATTLE_OVER, "won_rick_battle");
			}
			PlayerInteractPostBattleWithTrainerNode(trainerNode);
		}
		else if (SharedStatInfo.battleInfo.node is MortyNode && _worldController is WorldTutorialController)
		{
			((WorldTutorialController)_worldController).WildMortyCaughtShowDialogue();
		}
	}

	private void UncacheFromQuestRequest()
	{
		QuestGiverNode questGiverNode = SharedStatInfo.bagInfo.questGiverNode;
		string[] source = QuestDefs.SolutionIDs(questGiverNode.data.id);
		if (SharedStatInfo.bagInfo.selectedID == null)
		{
			questGiverNode.InteractionFinished();
		}
		else if (source.Contains(SharedStatInfo.bagInfo.selectedID))
		{
			App.upsight.CustomAnalyticEvent(eSubType1.QUEST, eSubType2.COMPLETE, questGiverNode.data.id);
			ItemDefs.RemoveItem(PlayerDefs.CurrentItems(), SharedStatInfo.bagInfo.selectedID);
			PlayerDefs.SetQuestComplete(questGiverNode.data.id, SharedStatInfo.bagInfo.selectedID);
			questGiverNode.questData.progress = eQuestProgress.FINISHED;
			questGiverNode.RefreshIcon();
			DialogueData dialogueData = new DialogueData(QuestDefs.AcceptDialogue(questGiverNode.data.id), QuestDefs.GiverName(questGiverNode.data.id), QuestDefs.GiverAssetID(questGiverNode.data.id));
			_worldUI.DisplayDialogueSpeech(dialogueData, questGiverNode, delegate
			{
				QuestGiverAcceptedDialogueComplete(questGiverNode);
			});
		}
		else
		{
			App.upsight.CustomAnalyticEvent(eSubType1.QUEST, eSubType2.REJECTED, questGiverNode.data.id);
			DialogueData dialogueData2 = new DialogueData(QuestDefs.RejectDialogue(questGiverNode.data.id), QuestDefs.GiverName(questGiverNode.data.id), QuestDefs.GiverAssetID(questGiverNode.data.id));
			_worldUI.DisplayDialogueSpeech(dialogueData2, questGiverNode);
		}
		SharedStatInfo.bagInfo = null;
	}

	protected override void Create()
	{
		base.Create();
		GameObject gameObject = new GameObject("WorldController");
		if (WorldDefs.isSegmentedWorldSaved)
		{
			SharedStatInfo.worldInfo.worldID = WorldDefs.GetSavedSegmentedWorldID();
			SharedStatInfo.worldInfo.nodeSetID = WorldDefs.GetSavedSegmentedNodeSetID();
		}
		else
		{
			PlayerDefs.SetNormalWorldLocation(SharedStatInfo.worldInfo.worldID, SharedStatInfo.worldInfo.entranceID);
			App.saveData.Save(eSaveDataType.PLAYER);
			SharedStatInfo.worldInfo.nodeSetID = WorldDefs.NodeSetIDs(SharedStatInfo.worldInfo.worldID).PickRandom();
		}
		if (SharedStatInfo.worldInfo.worldID.Equals("Tutorial"))
		{
			_worldController = gameObject.AddComponent<WorldTutorialController>();
		}
		else
		{
			_worldController = gameObject.AddComponent<WorldController>();
		}
		if (WorldDefs.isSegmentedWorldSaved)
		{
			_worldController.SetUpFromSave(SharedStatInfo.worldInfo.worldID, SharedStatInfo.worldInfo.nodeSetID);
		}
		else
		{
			_worldController.SetUp(SharedStatInfo.worldInfo.worldID, SharedStatInfo.worldInfo.nodeSetID, SharedStatInfo.worldInfo.entranceID);
			if (WorldDefs.IsSegmented(SharedStatInfo.worldInfo.worldID))
			{
				PlayerDefs.SetLastSegmentWorldID(SharedStatInfo.worldInfo.worldID);
				PlayerDefs.DimensionCount++;
			}
			SaveCurrentSegmentedWorld();
		}
		_worldUI.SetUp(WorldDefs.IsSegmented(SharedStatInfo.worldInfo.worldID), _worldController.itemCratesRemainingCount == 0, _worldController.trainersRemainingCount == 0, _worldController.gymLeadersRemainingCount == 0, true);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_worldUI = _ui as WorldUI;
	}

	protected override void Destroy()
	{
		UnityEngine.Object.Destroy(_worldController.gameObject);
		base.Destroy();
		_worldUI = null;
	}

	protected override void DestroyCache()
	{
		base.DestroyCache();
		_worldController.RefreshIntoCache();
		if (!_transitionedIntoPopup)
		{
			_worldController.ShowWorld(false);
			App.sound.StopAllSFX();
		}
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_worldController.PlayerInteractAvailableEvent += WorldPlayerInteractAvailableHandler;
		_worldController.PlayerInteractUnavailableEvent += WorldPlayerInteractUnavailableHandler;
		_worldController.PlayerInteractStartEvent += WorldPlayerInteractStartHandler;
		_worldController.PlayerInteractEndEvent += WorldPlayerInteractEndHandler;
		_worldController.DisplayDialogueEvent += WorldDisplayDialogueHandler;
		_worldUI.PausePressedEvent += UIPausePressedHandler;
		_worldUI.PortalGunPressedEvent += UIPortalGunPressedHandler;
		_worldUI.InputDPadChangedEvent += UIInputDPadChangedHandler;
		_worldUI.InputFaceBtnAPressedEvent += UIInputFaceBtnAPressedHandler;
		_worldUI.DialogueStartedEvent += UIDialogueStartedHandler;
		_worldUI.DialogueFinishedEvent += UIDialogueFinishedHandler;
		App.sound.SoundTrackFinishedEvent += SoundTrackFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_worldController.PlayerInteractAvailableEvent -= WorldPlayerInteractAvailableHandler;
		_worldController.PlayerInteractUnavailableEvent -= WorldPlayerInteractUnavailableHandler;
		_worldController.PlayerInteractStartEvent -= WorldPlayerInteractStartHandler;
		_worldController.PlayerInteractEndEvent -= WorldPlayerInteractEndHandler;
		_worldController.DisplayDialogueEvent -= WorldDisplayDialogueHandler;
		_worldUI.PausePressedEvent -= UIPausePressedHandler;
		_worldUI.PortalGunPressedEvent -= UIPortalGunPressedHandler;
		_worldUI.InputDPadChangedEvent -= UIInputDPadChangedHandler;
		_worldUI.InputFaceBtnAPressedEvent -= UIInputFaceBtnAPressedHandler;
		_worldUI.DialogueStartedEvent -= UIDialogueStartedHandler;
		_worldUI.DialogueFinishedEvent -= UIDialogueFinishedHandler;
		App.sound.SoundTrackFinishedEvent -= SoundTrackFinishedHandler;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (SharedStatInfo.worldInfo.entranceID == "TournamentLobbyToiletPortal")
		{
			_worldUI.DisplayDialogueSpeech(new DialogueData(TextDefs.TOURNAMENT_LOBBY_REVIVE, "Birdperson", "NPCBirdperson"));
			SharedStatInfo.worldInfo.entranceID = null;
		}
	}

	public void SaveCurrentSegmentedWorld()
	{
		if (_worldController != null)
		{
			_worldController.SaveWorld();
		}
	}

	private void PlayerInteractWithNode(PlayerNode playerNode, AdvancedNode node, out bool deleteWorldSaveData)
	{
		deleteWorldSaveData = false;
		string text = InteractionDefs.AssetID(node.data.id);
		string text2 = InteractionDefs.TransitionDirection(node.data.id);
		string[] array = ((text2 == null) ? null : text2.Split(':'));
		string entranceID = InteractionDefs.TransitionEntrance(node.data.id);
		string text3 = InteractionDefs.Name(node.data.id);
		string text4 = InteractionDefs.Dialogue(node.data.id);
		string ticketID = TournamentDefs.GetNextTicket();
		switch (InteractionDefs.InteractionType(node.data.id))
		{
		case "InvestmentRick":
			PlayerInteractWithInvestmentRick(node, new DialogueData(text4, text3, text));
			break;
		case "DialogueSpeech":
			_worldUI.DisplayDialogueSpeech(new DialogueData(text4, text3, text), node);
			break;
		case "DialogueAction":
			_worldUI.DisplayDialogueAction(text4, node);
			break;
		case "HealingTransition":
			_worldUI.DisplayDialogueSpeech(new DialogueData(text4, text3, text), node, delegate
			{
				HealingTransitionDialogueComplete((BuildingHealingCenterNode)node);
			});
			break;
		case "SalesmanRickTransition":
			_worldUI.DisplayDialogueSpeech(new DialogueData(text4, text3, text), node, delegate
			{
				SalesmanRickTransitionDialogueComplete((BuildingSalesmanRicksNode)node);
			});
			break;
		case "StorageTransition":
			_worldUI.DisplayDialogueSpeech(new DialogueData(text4, text3, text), node, delegate
			{
				StorageTransitionDialogueComplete((BuildingStorageNode)node);
			});
			break;
		case "MortyLabTransition":
			PlayerInteractWithMortyLabs(node, new DialogueData(text4, text3, text));
			break;
		case "TokenRickTransition":
			if (!string.IsNullOrEmpty(ticketID))
			{
				_worldUI.DisplayDialogueSpeech(new DialogueData(text4, text3, text), node, delegate
				{
					TicketRickTransitionDialogueComplete(node, ticketID);
				});
			}
			else
			{
				_worldUI.DisplayDialogueSpeech(new DialogueData(TextDefs.TOURNAMENT_LOBBY_TICKET_RICK_NONE, text3, text), node);
			}
			break;
		case "DirtyRickTransition":
			if (!TournamentDefs.IsItemStashActive())
			{
				_worldUI.DisplayDialogueSpeech(new DialogueData(text4, text3, text), node, delegate
				{
					DirtyRickTransitionDialogueComplete(node);
				});
			}
			else
			{
				_worldUI.DisplayDialogueSpeech(new DialogueData(TextDefs.TOURNAMENT_DIRTY_RICK_DIAL_ACTIVE, text3, text), node);
			}
			break;
		case "WorldTransition":
		{
			_worldController.RemovePlayerNode((eDirection)Enum.Parse(typeof(eDirection), array[0]), int.Parse(array[1]));
			SharedStatInfo.worldInfo.worldID = ((text == null) ? WorldDefs.GetRandomSegmentedWorldID(PlayerDefs.lastSegmentedWorldIDs) : text);
			SharedStatInfo.worldInfo.entranceID = entranceID;
			InterstitialState interstitialState = App.states.Get<InterstitialState>();
			interstitialState.placementID = eUpsightPlacementID.PORTAL;
			interstitialState.nextStateType = typeof(WorldState);
			App.states.Set<InterstitialState>(new FadeTransInfo());
			deleteWorldSaveData = true;
			break;
		}
		case "StoryTransition":
			_worldController.RemovePlayerNode((eDirection)Enum.Parse(typeof(eDirection), array[0]), int.Parse(array[1]));
			App.states.Get<StoryState>().storyIDToLoad = text;
			App.states.Set<StoryState>(new FadeTransInfo(), new FadeTransInfo());
			deleteWorldSaveData = true;
			break;
		case "TournamentOrganiser":
			PlayerInteractWithTournamentOrganiser(node, new DialogueData(text4, text3, text));
			break;
		case "HPCheck":
			PlayerInteractWithCitadelHPCheck(playerNode, node, new DialogueData(text4, text3, text));
			break;
		}
	}

	private bool PlayerInteractWithItemNode(ItemNode node)
	{
		int resultAmount;
		string resultText;
		bool flag = ItemDefs.ObtainItem(PlayerDefs.CurrentItems(), node.data.id, 1, out resultAmount, out resultText);
		if (flag)
		{
			UnityEngine.Object.Destroy(node.gameObject);
			App.saveData.Save(eSaveDataType.PLAYER);
			_worldUI.DisplayDialogueAction(string.Format(TextDefs.ITEM_FOUND, ItemDefs.Name(node.data.id)));
		}
		else
		{
			_worldUI.DisplayDialogueAction(string.Format(TextDefs.ITEM_FOUND_NO_SPACE, ItemDefs.Name(node.data.id), ItemDefs.Description(node.data.id)));
		}
		node.InteractionFinished();
		return flag;
	}

	private void PlayerInteractWithMortyNode(MortyNode node, bool isTrigger)
	{
		if (!isTrigger)
		{
			BattleOpponent player = new BattleOpponent(eBattleOpponentType.PLAYER, eBattleOpponentSide.LEFT, PlayerDefs.AssetID, null, PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems());
			BattleOpponentAI enemy = new BattleOpponentAI(eBattleOpponentType.AI_WILD_MORTY, eBattleOpponentSide.RIGHT, new List<MortyData> { node.mortyData }, null);
			SharedStatInfo.battleInfo = new BattleInfo(player, enemy, node, SharedStatInfo.worldInfo.worldID == "Tutorial");
			App.states.Set<BattleState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
	}

	private void PlayerInteractWithNPCNode(NPCNode node)
	{
		DialogueData dialogueData = new DialogueData(NPCDefs.Dialogue(node.data.id), NPCDefs.Name(node.data.id), NPCDefs.AssetID(node.data.id));
		_worldUI.DisplayDialogueSpeech(dialogueData, node);
	}

	private IEnumerator PlayerInteractPreBattleWithTrainerNode(PlayerNode playerNode, TrainerNode trainerNode, eDirection directionToPlayer)
	{
		PauseWorldSoundtrack();
		_worldUI.TweenOut();
		yield return StartCoroutine(trainerNode.TweenAttackIcon());
		playerNode.SetDirection(HelperMethods.GetOppositeDirection(directionToPlayer));
		int distanceToPlayer = Mathf.FloorToInt(Vector3.Distance(playerNode.transform.position, trainerNode.transform.position) / 1f) - 1;
		yield return StartCoroutine(trainerNode.ForceMoveInDirectionFor(directionToPlayer, distanceToPlayer));
		DialogueData preDialogueData = new DialogueData(TrainerDefs.GetRandomPreBattleDialogue(trainerNode.data.id), TrainerDefs.Name(trainerNode.data.id), TrainerDefs.AssetID(trainerNode.data.id));
		_worldUI.DisplayDialogueSpeech(preDialogueData, null, delegate
		{
			TrainerPreBattleDialogueComplete(trainerNode);
		});
	}

	private void PlayerInteractPostBattleWithTrainerNode(TrainerNode trainerNode)
	{
		DialogueData dialogueData = new DialogueData(TrainerDefs.DialoguePostBattle(trainerNode.data.id), TrainerDefs.Name(trainerNode.data.id), TrainerDefs.AssetID(trainerNode.data.id));
		_worldUI.DisplayDialogueSpeech(dialogueData, trainerNode, delegate
		{
			TrainerPostBattleDialogueComplete(trainerNode);
		});
	}

	private void PlayerInteractWithSignPostNode(AdvancedNode node)
	{
		_worldUI.DisplayDialogueAction(SignPostDefs.Dialogue(node.data.id), node);
	}

	private void PlayerInteractWithGachaMachineNode(AdvancedNode node)
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.COUPON_MACHINE, "visit_coupon_machine");
		App.states.Set<CouponUseState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private IEnumerator PlayerInteractedWithDialogueTrigger(PlayerNode playerNode, AdvancedNode node)
	{
		while (playerNode.IsMoving())
		{
			yield return null;
		}
		_worldController.DisplayDialogue(node.data.id);
		UnityEngine.Object.Destroy(node.gameObject);
	}

	private void PlayerInteractWithQuestGiverNode(QuestGiverNode questGiverNode)
	{
		if (questGiverNode.questData.progress == eQuestProgress.WAITING)
		{
			if (!PlayerDefs.IsQuestActive(questGiverNode.data.id))
			{
				App.upsight.CustomAnalyticEvent(eSubType1.QUEST, eSubType2.ACTIVE, questGiverNode.data.id);
				PlayerDefs.SetQuestActive(questGiverNode.data.id);
			}
			DialogueData dialogueData = new DialogueData(QuestDefs.ActiveDialogue(questGiverNode.data.id), QuestDefs.GiverName(questGiverNode.data.id), QuestDefs.GiverAssetID(questGiverNode.data.id));
			_worldUI.DisplayDialogueSpeech(dialogueData, null, delegate
			{
				QuestGiverWaitingDialogueComplete(questGiverNode);
			});
		}
		else
		{
			DialogueData dialogueData2 = new DialogueData(QuestDefs.CompleteDialogue(questGiverNode.data.id), QuestDefs.GiverName(questGiverNode.data.id), QuestDefs.GiverAssetID(questGiverNode.data.id));
			_worldUI.DisplayDialogueSpeech(dialogueData2, questGiverNode);
		}
		App.saveData.Save(eSaveDataType.PLAYER);
	}

	private void PlayerInteractWithCraftTableNode(AdvancedNode node)
	{
		App.states.Set<CraftState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void PlayerInteractWithItemCrateNode(ItemCrateNode node)
	{
		UnityEngine.Object.Destroy(node.gameObject);
		SharedStatInfo.rewardInfo = new RewardInfo(RewardsDefs.GetItemCrateReward(), "item_crate");
		App.states.Set<RewardState>(new FadeTransInfo
		{
			duration = 0.05f,
			color = Color.white
		}, new FadeTransInfo
		{
			duration = 0.2f,
			color = Color.white
		}, true, eStateAction.ADD_CURRENT_TO_CACHE);
		_worldController.itemCratesRemainingCount--;
		if (_worldController.itemCratesRemainingCount <= 0)
		{
			_worldUI.SetHudItemsObjectTicked();
		}
		node.InteractionFinished();
	}

	private void PlayerInteractWithMortyLabs(AdvancedNode node, DialogueData dialogueData)
	{
		if (PlayerDefs.DefeatedCouncilMembers < 1)
		{
			dialogueData.entries[0].text = new string[1] { TextDefs.MORTY_LAB_LOCKED_DIAL };
			_worldUI.DisplayDialogueSpeech(dialogueData, node);
		}
		else
		{
			_worldUI.DisplayDialogueSpeech(dialogueData, node, delegate
			{
				MortyLabTransitionDialogueComplete((BuildingMortyLabNode)node);
			});
		}
	}

	private void PlayerInteractWithTournamentOrganiser(AdvancedNode node, DialogueData dialogueData)
	{
		if (SharedStatInfo.tournamentSetUpInfo != null)
		{
			dialogueData = TournamentDefs.DialogueOrganiser(SharedStatInfo.tournamentSetUpInfo.eventID);
			_worldUI.DisplayDialogueSpeech(dialogueData, node, delegate
			{
				TournamentOrganiserPostSelectDialogueComplete();
			});
		}
		else
		{
			_worldUI.DisplayDialogueSpeech(dialogueData, node, delegate
			{
				TournamentOrganiserPreSelectDialogueComplete();
			});
		}
	}

	private void PlayerInteractWithInvestmentRick(AdvancedNode node, DialogueData dialogueData)
	{
		_worldUI.DisplayDialogueSpeech(dialogueData, node, delegate
		{
			App.states.Set<InvestmentState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		});
	}

	private void PlayerInteractWithCitadelHPCheck(PlayerNode playerNode, AdvancedNode node, DialogueData dialogueData)
	{
		if (playerNode.GetDirection() == eDirection.UP && PlayerDefs.GetPartyHPToRegain() > 0)
		{
			_worldUI.DisplayDialogueSpeech(dialogueData, node);
		}
	}

	public void BattleFinished()
	{
		if (!IsActive() || (SharedStatInfo.battleInfo.outcomeType != eBattleOutcomeType.PLAYER_WON && SharedStatInfo.battleInfo.outcomeType != eBattleOutcomeType.WILD_MORTY_CAUGHT))
		{
			return;
		}
		if (SharedStatInfo.battleInfo.node is MortyNode)
		{
			MortyNode mortyNode = (MortyNode)SharedStatInfo.battleInfo.node;
			UnityEngine.Object.Destroy(mortyNode.gameObject);
			if (_worldController is WorldTutorialController)
			{
				((WorldTutorialController)_worldController).WildMortyCaught();
			}
		}
		App.saveData.Save(eSaveDataType.PLAYER);
		SaveCurrentSegmentedWorld();
	}

	public void RefreshPlayerMorties()
	{
		if (_worldController != null)
		{
			_worldController.RefreshPlayerMorties();
		}
	}

	public void RefreshPlayerAsset()
	{
		if (_worldController != null)
		{
			_worldController.RefreshPlayerAsset();
		}
	}

	public void ShowWorld(bool value)
	{
		if (_worldController != null)
		{
			_worldController.ShowWorld(value);
		}
	}

	private void PauseWorldSoundtrack()
	{
		if (App.sound.isSoundtrackPlaying)
		{
			_stopSoundtrackPos = App.sound.GetCurrentSountrackTime();
			App.sound.StopSoundtrack(0.5f);
		}
	}

	private void HealingTransitionDialogueComplete(BuildingHealingCenterNode node)
	{
		App.states.Set<HealingCenterState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void SalesmanRickTransitionDialogueComplete(BuildingSalesmanRicksNode node)
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.WORLD, "store_menu");
		App.states.Get<SalesmanRickState>().playStoreSoundtrack = true;
		App.states.Set<SalesmanRickState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void StorageTransitionDialogueComplete(BuildingStorageNode node)
	{
		App.states.Set<StorageState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void MortyLabTransitionDialogueComplete(BuildingMortyLabNode node)
	{
		App.states.Set<MortyLabState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void TicketRickTransitionDialogueComplete(AdvancedNode node, string ticketID)
	{
		TournamentDefs.RemoveTicket(ticketID);
		RewardData item = new RewardData(eRewardType.MORTY, ticketID, 100f);
		List<RewardData> list = new List<RewardData>();
		list.Add(item);
		SharedStatInfo.rewardInfo = new RewardInfo(list, "tournament_ticket");
		App.states.Set<RewardState>(new FadeTransInfo
		{
			duration = 0.05f,
			color = Color.white
		}, new FadeTransInfo
		{
			duration = 0.2f,
			color = Color.white
		}, true, eStateAction.ADD_CURRENT_TO_CACHE);
		App.saveData.Save(eSaveDataType.TOURNAMENT);
	}

	private void DirtyRickTransitionDialogueComplete(AdvancedNode node)
	{
		App.states.Set<TournamentDirtyRickState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void TournamentOrganiserPreSelectDialogueComplete()
	{
		App.states.Set<TournamentEventSelectState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void TournamentOrganiserPostSelectDialogueComplete()
	{
		App.states.Set<TournamentPartySelectState>(new FadeTransInfo(), new FadeTransInfo());
	}

	private void TrainerPreBattleDialogueComplete(TrainerNode node)
	{
		BattleOpponent player = new BattleOpponent(eBattleOpponentType.PLAYER, eBattleOpponentSide.LEFT, PlayerDefs.AssetID, null, PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems());
		BattleOpponentAI enemy = new BattleOpponentAI(eBattleOpponentType.AI_TRAINER, eBattleOpponentSide.RIGHT, node.trainerData.morties, node.trainerData.items, node.trainerData);
		SharedStatInfo.battleInfo = new BattleInfo(player, enemy, node, SharedStatInfo.worldInfo.worldID == "Tutorial");
		App.states.Set<BattleState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void TrainerPostBattleDialogueComplete(TrainerNode node)
	{
		StartCoroutine(TrainerPostBattleDialogueCompleteCo(node));
	}

	private IEnumerator TrainerPostBattleDialogueCompleteCo(TrainerNode node)
	{
		node.ActivateInteractionPoint(false);
		if (node.trainerData.type == eTrainerType.GYM_LEADER && node.trainerData.isDefeated)
		{
			_worldController.RefreshGymLeaderDefeated();
		}
		int distanceToMove = Mathf.RoundToInt(Vector3.Distance(node.setUpPosition, node.transform.localPosition) / 1f);
		if (distanceToMove > 0)
		{
			yield return StartCoroutine(node.ForceMoveInDirectionFor(HelperMethods.GetOppositeDirection(node.GetDirection()), distanceToMove));
			node.SetDirection(HelperMethods.GetOppositeDirection(node.GetDirection()));
		}
		node.ActivateInteractionPoint(true);
	}

	private void QuestGiverWaitingDialogueComplete(QuestGiverNode questGiverNode)
	{
		SharedStatInfo.bagInfo = new BagInfo(PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems(), eBagFromState.QUEST, questGiverNode);
		App.states.Set<BagState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void QuestGiverAcceptedDialogueComplete(QuestGiverNode questGiverNode)
	{
		List<RewardData> list = QuestDefs.Rewards(questGiverNode.data.id);
		if (list != null && list.Count > 0)
		{
			if (questGiverNode.data.id.Equals("QuestMascotMorty"))
			{
				UnityEngine.Object.Destroy(questGiverNode.gameObject, 0.5f);
			}
			SharedStatInfo.rewardInfo = new RewardInfo(list, "quest_complete");
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
			App.saveData.Save(eSaveDataType.PLAYER);
		}
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
		bool deleteWorldSaveData = false;
		bool flag = true;
		switch (node.type)
		{
		default:
			PlayerInteractWithNode(playerNode, node, out deleteWorldSaveData);
			break;
		case eNodeType.ITEM:
			flag = PlayerInteractWithItemNode((ItemNode)node);
			break;
		case eNodeType.MORTY:
			PlayerInteractWithMortyNode((MortyNode)node, isTrigger);
			if (isTrigger)
			{
				return;
			}
			break;
		case eNodeType.NPC:
			PlayerInteractWithNPCNode((NPCNode)node);
			break;
		case eNodeType.TRAINER:
		{
			TrainerNode trainerNode = (TrainerNode)node;
			if (!trainerNode.trainerData.isDefeated)
			{
				StartCoroutine(PlayerInteractPreBattleWithTrainerNode(playerNode, trainerNode, directionFromAngle));
			}
			else
			{
				PlayerInteractPostBattleWithTrainerNode(trainerNode);
			}
			break;
		}
		case eNodeType.SIGN_POST:
			PlayerInteractWithSignPostNode(node);
			break;
		case eNodeType.GACHA_MACHINE:
			PlayerInteractWithGachaMachineNode(node);
			break;
		case eNodeType.DIALOGUE_TRIGGER:
			StartCoroutine(PlayerInteractedWithDialogueTrigger(playerNode, node));
			break;
		case eNodeType.QUEST_GIVER:
			PlayerInteractWithQuestGiverNode((QuestGiverNode)node);
			break;
		case eNodeType.CRAFT_TABLE:
			PlayerInteractWithCraftTableNode(node);
			break;
		case eNodeType.ITEM_CRATE:
			PlayerInteractWithItemCrateNode((ItemCrateNode)node);
			break;
		}
		if (flag)
		{
			node.InteractionSuccess();
			_worldController.PlayerInteractSuccess(node);
		}
		if (deleteWorldSaveData)
		{
			App.saveData.Delete(eSaveDataType.SEGMENTED_WORLD);
		}
		else
		{
			SaveCurrentSegmentedWorld();
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

	private void UIPausePressedHandler()
	{
		_transitionedIntoPopup = true;
		SaveCurrentSegmentedWorld();
		leaveMusicPlayingOnCache = true;
		App.states.Set<PauseState>(null, new SlideTransInfo
		{
			direction = eDirection.DOWN,
			duration = 0.3f
		}, true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIPortalGunPressedHandler()
	{
		_transitionedIntoPopup = true;
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		string text = string.Format(TextDefs.WORLD_GENERIC_POPUP_PORTAL_GUN_DESC);
		genericPopupState.SetDisplayValues(text, new string[1] { TextDefs.WORLD_GENERIC_POPUP_PORTAL_GUN_BTN });
		genericPopupState.ButtonClickedEvent += GenericPopupPortalGunConfirmationHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIInputDPadChangedHandler(eDirection direction)
	{
		_worldController.DPadChanged(direction);
	}

	private void UIInputFaceBtnAPressedHandler()
	{
		_worldController.FaceBtnAPressed();
	}

	private void UIDialogueStartedHandler()
	{
		_worldUI.TweenOut();
	}

	private void UIDialogueFinishedHandler()
	{
		if (!IsTransitioningOut())
		{
			_worldUI.TweenIn();
		}
	}

	public void GenericPopupPortalGunConfirmationHandler(int buttonIndex)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.ButtonClickedEvent -= GenericPopupPortalGunConfirmationHandler;
		if (buttonIndex == 1)
		{
			App.saveData.Delete(eSaveDataType.SEGMENTED_WORLD);
			SharedStatInfo.worldInfo.worldID = "Citadel";
			SharedStatInfo.worldInfo.nodeSetID = "Citadel";
			SharedStatInfo.worldInfo.entranceID = "CitadelPortal";
			App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
		}
		else
		{
			App.states.SetBack(new PopTransInfo());
		}
	}

	private void SoundTrackFinishedHandler()
	{
		if (base.status == eStateStatus.DEACTIVE || IsTransitioning() || (base.status == eStateStatus.CACHED && !leaveMusicPlayingOnCache))
		{
			return;
		}
		if (_soundtrackQueue == null || _soundtrackQueue.Count == 0)
		{
			_soundtrackQueue = new List<string>(AudioDefs.ST_WORLD_ARR);
			if (!(_worldController is WorldTutorialController))
			{
				_soundtrackQueue.Shuffle();
			}
		}
		_soundtrackName = _soundtrackQueue[0];
		_soundtrackQueue.RemoveAt(0);
		App.sound.PlaySoundtrack(_soundtrackName, 0.2f, 0f, false);
	}
}
