using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MPWorldState : StateBase
{
	private MPWorldUI _worldUI;

	private MPWorldController _worldController;

	private bool _transitionedIntoPopup;

	public bool leaveMusicPlayingOnCache;

	private float _stopSoundtrackPos;

	private string _soundtrackName;

	private List<string> _soundtrackQueue;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPWorld/{0}", "MPWorld");
		base.showLoadingScreen = true;
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (App.net.room.isPaused)
		{
			UnpauseRoomAndReactivate(uiContainerTrans, transInInfo);
			return;
		}
		if (MPSharedStateInfo.worldInfo.isRevive)
		{
			Coord playerSpawnCoord = MPWorldDefs.GetPlayerSpawnCoord(MPSharedStateInfo.worldInfo.worldId, MPSharedStateInfo.worldInfo.isRevive);
			App.net.player.data.placementData = new MPNodePlacementData(playerSpawnCoord, eDirection.UP, new Coord[0], new eDirection[0]);
		}
		base.Activate(uiContainerTrans, transInInfo);
		App.net.player.SetState(eMPOpponentState.WORLD);
		SoundTrackFinishedHandler();
		_worldUI.RefreshForBattleInvites(App.net.battleInvite.GetReceivedInvites(), App.net.battleInvite.friendInvitePending);
		App.net.battle.SetActiveAvailableHandlers(true);
		StartCoroutine(ActivateComplete());
	}

	protected override IEnumerator ActivateComplete()
	{
		if (MPSharedStateInfo.worldInfo.isRevive)
		{
			_transInInfo = null;
		}
		yield return StartCoroutine(_003CActivateComplete_003E__BaseCallProxy0());
		if (MPSharedStateInfo.worldInfo.isRevive)
		{
			_worldController.SendPlayerPlacementData(true);
			ShowWorld(false);
			App.states.Set<MPBlackedOutState>(null, new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		MPSharedStateInfo.worldInfo.isRevive = false;
	}

	private void UnpauseRoomAndReactivate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		App.net.player.GetDetails(ePlayerDataParams.POST_BATTLE, delegate(eGetPlayerDataError playerDataError)
		{
			if (playerDataError != eGetPlayerDataError.NONE)
			{
				Activate(uiContainerTrans, transInInfo);
				App.net.session.Disconnect(eNetSessionDisconnectType.FAILED_TO_UNPAUSE_ROOM);
			}
			else
			{
				App.net.room.Unpause(delegate
				{
					Activate(uiContainerTrans, transInInfo);
				});
			}
		});
	}

	public override void Deactivate(TransInfo transInfo)
	{
		base.Deactivate(transInfo);
		App.net.player.SetState(eMPOpponentState.OCCUPIED);
		float fadeOutVal = ((transInfo == null) ? 0.5f : transInfo.duration);
		App.sound.StopSoundtrack(fadeOutVal);
		_transitionedIntoPopup = false;
	}

	public override void Cache(TransInfo transOutInfo)
	{
		base.Cache(transOutInfo);
		App.net.player.SetState(eMPOpponentState.OCCUPIED);
		if (!leaveMusicPlayingOnCache)
		{
			PauseWorldSoundtrack();
		}
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		App.net.player.SetState(eMPOpponentState.WORLD);
		leaveMusicPlayingOnCache = false;
		_worldController.ShowWorld(true);
		_worldController.RefreshFromCache();
		AudioSource audioSource = App.sound.PlaySoundtrack(_soundtrackName, 0.2f, 0f, false);
		if ((bool)audioSource)
		{
			audioSource.time = _stopSoundtrackPos;
		}
		_worldUI.RefreshFromCache();
		_worldUI.RefreshForBattleInvites(App.net.battleInvite.GetReceivedInvites(), App.net.battleInvite.friendInvitePending);
		_transitionedIntoPopup = false;
	}

	protected override void Create()
	{
		base.Create();
		GameObject gameObject = new GameObject("WorldController");
		_worldController = gameObject.AddComponent<MPWorldController>();
		_worldController.SetUp(MPSharedStateInfo.worldInfo.worldId);
		_worldUI.SetUp();
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_worldUI = _ui as MPWorldUI;
	}

	protected override void Destroy()
	{
		Object.Destroy(_worldController.gameObject);
		base.Destroy();
		App.net.battle.SetActiveAvailableHandlers(false);
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
		_worldUI.WorldTouchEvent += UIWorldTouchHandler;
		_worldUI.InputDPadChangedEvent += UIInputDPadChangedHandler;
		_worldUI.InputFaceBtnAPressedEvent += UIInputFaceBtnAPressedHandler;
		_worldUI.PausePressedEvent += UIPausePressedHandler;
		_worldUI.MapPressedEvent += UIMapPressedHandler;
		_worldUI.DialogueStartedEvent += UIDialogueStartedHandler;
		_worldUI.DialogueFinishedEvent += UIDialogueFinishedHandler;
		_worldUI.InviteResponseEvent += UIInviteResponseHandler;
		_worldUI.InviteInfoEvent += UIInviteInfoHandler;
		_worldUI.UserBoxStartedEvent += UIUserBoxStartedHandler;
		_worldUI.UserBoxFinishedEvent += UIUserBoxFinishedHandler;
		_worldUI.UserBoxClosedEvent += UIUserBoxClosedHandler;
		_worldUI.UserBoxBattleInviteEvent += UIUserBoxBattleHandler;
		_worldUI.UserBoxAddFriendEvent += UIUserBoxAddFriendHandler;
		_worldUI.UserBoxAcceptInviteEvent += UIUserBoxAcceptHandler;
		_worldUI.EmoteBarBtnPressedEvent += UIEmoteBarBtnPressedHandler;
		_worldController.PlayerInteractAvailableEvent += WorldPlayerInteractAvailableHandler;
		_worldController.PlayerInteractUnavailableEvent += WorldPlayerInteractUnavailableHandler;
		_worldController.PlayerInteractStartRequestEvent += WorldPlayerInteractStartRequestHandler;
		_worldController.PlayerInteractEndRequestEvent += WorldPlayerInteractEndRequestHandler;
		App.sound.SoundTrackFinishedEvent += SoundTrackFinishedHandler;
		App.net.session.EmoteEvent += NetSessionEmoteToRoomHandler;
		App.net.room.UserAddedEvent += NetRoomUserAddedHandler;
		App.net.room.UserRemovedEvent += NetRoomUserRemovedHandler;
		App.net.room.UserStateChangedEvent += NetRoomUserStateChangedHandler;
		App.net.room.UserModifiedEvent += NetRoomUserModifiedHandler;
		App.net.room.WildMortyAddedEvent += NetRoomWildMortyAddedHandler;
		App.net.room.WildMortyRemovedEvent += NetRoomWildMortyRemovedHandler;
		App.net.room.WildMortyStateChangedEvent += NetRoomWildMortyStateChangedHandler;
		App.net.room.PickupAddedEvent += NetRoomPickupAddedHandler;
		App.net.room.PickupRemovedEvent += NetRoomPickupRemovedHandler;
		App.net.room.nodeReceiver.NodePlacementReceivedEvent += NetRoomNodePlacementReceivedHandler;
		App.net.player.MortiesChangedEvent += NetPlayerMortiesChangedHandler;
		App.net.player.AvatarChangedEvent += NetPlayerAvatarChangedHandler;
		App.net.battle.StartEvent += NetBattleStartHandler;
		App.net.battleInvite.InviteQueueChangeEvent += InviteQueueChangeHandler;
		App.net.battleInvite.SentInviteExpiredEvent += SentInviteExpiredHandler;
		App.net.battleInvite.SentInviteDeclinedEvent += SentInviteDeclinedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_worldUI.WorldTouchEvent -= UIWorldTouchHandler;
		_worldUI.InputDPadChangedEvent -= UIInputDPadChangedHandler;
		_worldUI.InputFaceBtnAPressedEvent -= UIInputFaceBtnAPressedHandler;
		_worldUI.PausePressedEvent -= UIPausePressedHandler;
		_worldUI.MapPressedEvent -= UIMapPressedHandler;
		_worldUI.DialogueStartedEvent -= UIDialogueStartedHandler;
		_worldUI.DialogueFinishedEvent -= UIDialogueFinishedHandler;
		_worldUI.InviteResponseEvent -= UIInviteResponseHandler;
		_worldUI.InviteInfoEvent -= UIInviteInfoHandler;
		_worldUI.UserBoxStartedEvent -= UIUserBoxStartedHandler;
		_worldUI.UserBoxFinishedEvent -= UIUserBoxFinishedHandler;
		_worldUI.UserBoxClosedEvent -= UIUserBoxClosedHandler;
		_worldUI.UserBoxBattleInviteEvent -= UIUserBoxBattleHandler;
		_worldUI.UserBoxAddFriendEvent -= UIUserBoxAddFriendHandler;
		_worldUI.UserBoxAcceptInviteEvent -= UIUserBoxAcceptHandler;
		_worldUI.EmoteBarBtnPressedEvent -= UIEmoteBarBtnPressedHandler;
		_worldController.PlayerInteractAvailableEvent -= WorldPlayerInteractAvailableHandler;
		_worldController.PlayerInteractUnavailableEvent -= WorldPlayerInteractUnavailableHandler;
		_worldController.PlayerInteractStartRequestEvent -= WorldPlayerInteractStartRequestHandler;
		_worldController.PlayerInteractEndRequestEvent -= WorldPlayerInteractEndRequestHandler;
		App.sound.SoundTrackFinishedEvent -= SoundTrackFinishedHandler;
		App.net.session.EmoteEvent -= NetSessionEmoteToRoomHandler;
		App.net.room.UserAddedEvent -= NetRoomUserAddedHandler;
		App.net.room.UserRemovedEvent -= NetRoomUserRemovedHandler;
		App.net.room.UserStateChangedEvent -= NetRoomUserStateChangedHandler;
		App.net.room.UserModifiedEvent -= NetRoomUserModifiedHandler;
		App.net.room.WildMortyAddedEvent -= NetRoomWildMortyAddedHandler;
		App.net.room.WildMortyRemovedEvent -= NetRoomWildMortyRemovedHandler;
		App.net.room.WildMortyStateChangedEvent -= NetRoomWildMortyStateChangedHandler;
		App.net.room.PickupAddedEvent -= NetRoomPickupAddedHandler;
		App.net.room.PickupRemovedEvent -= NetRoomPickupRemovedHandler;
		if (App.net.room.nodeReceiver != null)
		{
			App.net.room.nodeReceiver.NodePlacementReceivedEvent -= NetRoomNodePlacementReceivedHandler;
		}
		App.net.player.MortiesChangedEvent -= NetPlayerMortiesChangedHandler;
		App.net.player.AvatarChangedEvent -= NetPlayerAvatarChangedHandler;
		App.net.battle.StartEvent -= NetBattleStartHandler;
		App.net.battleInvite.InviteQueueChangeEvent -= InviteQueueChangeHandler;
		App.net.battleInvite.SentInviteExpiredEvent -= SentInviteExpiredHandler;
		App.net.battleInvite.SentInviteDeclinedEvent -= SentInviteDeclinedHandler;
	}

	private void InteractRequestPlayerWithUser(MPUserNode playerNode, MPUserNode otherUserNode)
	{
		_worldUI.DisplayUserBoxToChallenge(otherUserNode.userData);
	}

	private void InteractRequestPlayerWithWildMorty(MPUserNode playerNode, MPWildMortyNode mortyNode)
	{
		App.net.battleInvite.SendInviteWildMorty(mortyNode.data.netId);
	}

	private void InteractRequestPlayerWithNPC(MPUserNode playerNode, MPNPCNode npcNode)
	{
		DialogueData dialogueData = new DialogueData(NPCDefs.Dialogue(npcNode.id), NPCDefs.Name(npcNode.id), NPCDefs.AssetID(npcNode.id));
		_worldUI.DisplayDialogueSpeech(dialogueData);
	}

	private void InteractRequestPlayerWithItem(MPUserNode playerNode, MPPickupNode pickupNode)
	{
		MPSharedStateInfo.rewardInfo = new MPRewardInfo(eMPStateOrigin.WORLD, pickupNode.data.rewardDatas, "item_crate", eMPRewardConfirmation.WAITING);
		App.net.room.RequestPickupCollect(pickupNode.data);
		App.states.Set<MPRewardState>(new FadeTransInfo
		{
			duration = 0.05f,
			color = Color.white
		}, new FadeTransInfo
		{
			duration = 0.2f,
			color = Color.white
		}, true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void InteractRequestPlayerWithSignPost(MPUserNode playerNode, MPSignPostNode signPostNode)
	{
		_worldUI.DisplayDialogueAction("Signpost coming soon!");
	}

	private void InteractRequestPlayerWithPortal(MPUserNode playerNode, MPPortalNode portalNode)
	{
		ActivateInput(false);
		App.net.room.Leave(delegate
		{
			App.states.Set<MPWorldSelectState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
		});
	}

	private void InteractRequestPlayerWithHealingCenter(MPUserNode playerNode, MPHealingCenterNode healingCenterNode)
	{
		MPSharedStateInfo.healingCenterInfo = new MPHealingCenterInfo(eMPStateOrigin.WORLD, false);
		App.states.Set<MPHealingCenterState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void InteractRequestPlayerWithSalesmanRicks(MPUserNode playerNode, MPSalesmanRicksNode itemNode)
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.WORLD, "store_menu");
		App.states.Set<MPSalesmanRickState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void InteractRequestPlayerWithStorage(MPUserNode playerNode, MPStorageNode storageNode)
	{
		App.states.Set<MPStorageState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void InteractRequestPlayerWithMortyLab(MPUserNode playerNode, MPMortyLabNode mortyLabNode)
	{
		MPSharedStateInfo.labsInfo = new MPLabsInfo(eMPStateOrigin.WORLD);
		App.states.Set<MPMortyLabState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void InteractRequestPlayerWithCraftTable(MPUserNode playerNode, MPCraftTableNode craftTableNode)
	{
		MPSharedStateInfo.craftInfo = new MPCraftInfo(eMPStateOrigin.WORLD, new Dictionary<string, int>(App.net.player.data.items));
		App.states.Set<MPCraftState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void InteractRequestPlayerWithGachaMachine(MPUserNode playerNode, MPGachaMachineNode gatchaNode)
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.COUPON_MACHINE, "visit_coupon_machine");
		App.states.Set<MPCouponUseState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void InteractRequestPlayerWithMeeseekAd(MPUserNode playerNode, MPVideoRewardMeeseekNode meeseekAdNode)
	{
		DialogueData dialogueData = new DialogueData(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_WORLD_DIALOGUE, MPTextDefs.MP_VIDEO_REWARD_MEESEEK_NAME, "NPCVideoRewardMeeseek");
		_worldUI.DisplayDialogueSpeech(dialogueData, null, delegate
		{
			MPSharedStateInfo.videoRewardMeeseekInfo = new MPVideoRewardMeeseekInfo(eMPStateOrigin.WORLD, App.net.room.videoRewardMeeseeksData);
			App.states.Set<MPVideoRewardMeeseekState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		});
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

	private void UIWorldTouchHandler(Vector2 viewportPoint)
	{
		_worldController.WorldTouched(viewportPoint);
	}

	private void UIInputDPadChangedHandler(eDirection direction)
	{
		_worldController.DPadChanged(direction);
	}

	private void UIInputFaceBtnAPressedHandler()
	{
		_worldController.FaceBtnAPressed();
	}

	private void UIPausePressedHandler()
	{
		_transitionedIntoPopup = true;
		leaveMusicPlayingOnCache = true;
		App.states.Set<MPPauseState>(null, new SlideTransInfo
		{
			direction = eDirection.DOWN,
			duration = 0.3f
		}, true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIMapPressedHandler()
	{
		MPSharedStateInfo.worldMapInfo = new MPWorldMapInfo(eMPStateOrigin.WORLD, MPSharedStateInfo.worldInfo.worldId);
		App.states.Set<MPWorldMapState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
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

	private void UIInviteResponseHandler(MPBattleInviteData data, bool accept)
	{
		App.net.battleInvite.RespondToInvite(data, accept);
		if (!accept)
		{
			_worldUI.RefreshForBattleInvites(App.net.battleInvite.GetReceivedInvites(), App.net.battleInvite.friendInvitePending);
		}
	}

	private void UIInviteInfoHandler(MPBattleInviteData data)
	{
		_worldUI.HideChallengeNotification();
		_worldUI.DisplayUserBoxForInviteResponse(data);
	}

	private void UIUserBoxStartedHandler()
	{
		_worldUI.TweenOut();
	}

	private void UIUserBoxFinishedHandler()
	{
		_worldUI.TweenIn();
		_worldUI.RefreshForBattleInvites(App.net.battleInvite.GetReceivedInvites(), App.net.battleInvite.friendInvitePending);
	}

	private void UIUserBoxClosedHandler(MPBattleInviteData inviteData = null)
	{
		if (inviteData != null)
		{
			App.net.battleInvite.RespondToInvite(inviteData, false);
		}
		else if (App.net.battleInvite.sentInvitePending)
		{
			App.net.battleInvite.CancelPendingSentInvite();
		}
		_worldUI.CloseUserBox();
	}

	private void UIUserBoxBattleHandler(string userId)
	{
		_worldUI.DisplayUserBoxWaiting();
		App.net.battleInvite.SendInvite(userId, "PvP", delegate(eSendInviteError error)
		{
			switch (error)
			{
			case eSendInviteError.OPPONENT_STATE_INVALID:
				_worldUI.DisplayUserBoxDeclined();
				break;
			default:
				_worldUI.DisplayUserBoxFailed();
				break;
			case eSendInviteError.NONE:
				break;
			}
		});
	}

	private void UIUserBoxAddFriendHandler(string userId)
	{
		if (!App.net.friend.IsFriendListFull())
		{
			_worldUI.DisplayUserBoxWaiting();
			App.net.friend.SendRequest(userId, delegate(eFriendSendRequestError sendError)
			{
				if (sendError == eFriendSendRequestError.NONE)
				{
					App.net.friend.RefreshData(delegate(eFriendRefreshDataError refreshError)
					{
						if (refreshError == eFriendRefreshDataError.NONE)
						{
							_worldUI.DisplayUserBoxFriendAdded(sendError);
						}
						else
						{
							App.net.session.Disconnect(eNetSessionDisconnectType.REFRESH_FRIEND_LIST_FAILED);
						}
					});
				}
				else
				{
					_worldUI.DisplayUserBoxFriendAdded(sendError);
				}
			});
		}
		else
		{
			_worldUI.DisplayUserBoxFriendAdded(eFriendSendRequestError.FRIEND_LIMIT_SELF);
		}
	}

	private void UIUserBoxAcceptHandler(MPBattleInviteData inviteData = null)
	{
		App.net.battleInvite.RespondToInvite(inviteData, true, delegate(eRespondToInviteError error)
		{
			if (error != eRespondToInviteError.NONE)
			{
				_worldUI.DisplayUserBoxFailed();
				App.net.player.SetState(eMPOpponentState.WORLD);
			}
		});
		App.net.player.SetState(eMPOpponentState.OCCUPIED);
	}

	private void UIEmoteBarBtnPressedHandler(string emoteId)
	{
		App.net.session.EmoteToRoom(emoteId);
	}

	private void WorldPlayerInteractAvailableHandler()
	{
		_worldUI.SetInputForPlayerInteract(true);
	}

	private void WorldPlayerInteractUnavailableHandler()
	{
		_worldUI.SetInputForPlayerInteract(false);
	}

	private void WorldPlayerInteractStartRequestHandler(MPUserNode playerNode, MPNode node)
	{
		eDirection direction = playerNode.GetDirection();
		eDirection oppositeDirection = HelperMethods.GetOppositeDirection(direction);
		playerNode.InteractionStarted(node, direction);
		node.InteractionStarted(playerNode, oppositeDirection);
		switch (node.interactionType)
		{
		case eMPNodeInteractionType.USER:
			InteractRequestPlayerWithUser(playerNode, (MPUserNode)node);
			break;
		case eMPNodeInteractionType.WILD_MORTY:
			InteractRequestPlayerWithWildMorty(playerNode, (MPWildMortyNode)node);
			break;
		case eMPNodeInteractionType.NPC:
			InteractRequestPlayerWithNPC(playerNode, (MPNPCNode)node);
			break;
		case eMPNodeInteractionType.ITEM:
			InteractRequestPlayerWithItem(playerNode, (MPPickupNode)node);
			break;
		case eMPNodeInteractionType.SIGN_POST:
			InteractRequestPlayerWithSignPost(playerNode, (MPSignPostNode)node);
			break;
		case eMPNodeInteractionType.PORTAL:
			InteractRequestPlayerWithPortal(playerNode, (MPPortalNode)node);
			break;
		case eMPNodeInteractionType.HEALING_CENTER:
			InteractRequestPlayerWithHealingCenter(playerNode, (MPHealingCenterNode)node);
			break;
		case eMPNodeInteractionType.SALESMAN_RICK:
			InteractRequestPlayerWithSalesmanRicks(playerNode, (MPSalesmanRicksNode)node);
			break;
		case eMPNodeInteractionType.STORAGE:
			InteractRequestPlayerWithStorage(playerNode, (MPStorageNode)node);
			break;
		case eMPNodeInteractionType.MORTY_LAB:
			InteractRequestPlayerWithMortyLab(playerNode, (MPMortyLabNode)node);
			break;
		case eMPNodeInteractionType.CRAFT_TABLE:
			InteractRequestPlayerWithCraftTable(playerNode, (MPCraftTableNode)node);
			break;
		case eMPNodeInteractionType.GACHA_MACHINE:
			InteractRequestPlayerWithGachaMachine(playerNode, (MPGachaMachineNode)node);
			break;
		case eMPNodeInteractionType.VIDEO_REWARD_MEESEEK:
			InteractRequestPlayerWithMeeseekAd(playerNode, (MPVideoRewardMeeseekNode)node);
			break;
		case eMPNodeInteractionType.TRAINER:
			break;
		}
	}

	private void WorldPlayerInteractEndRequestHandler(MPUserNode playerNode, MPNode node)
	{
		playerNode.InteractionFinished(node);
		node.InteractionFinished(playerNode);
	}

	private void WorldDisplayDialogueHandler(DialogueData dialogueData)
	{
		_worldUI.DisplayDialogueSpeech(dialogueData);
	}

	public void NetSessionEmoteToRoomHandler(string emoteId, string userId)
	{
		_worldController.DisplayEmoteOnUser(emoteId, userId);
	}

	private void NetRoomUserAddedHandler(MPUserData userData)
	{
		_worldController.CreateUserNode(userData);
		_worldController.SendPlayerPlacementData();
	}

	private void NetRoomUserRemovedHandler(MPUserData userData)
	{
		_worldController.RemoveNetNode(userData.netId);
		_worldUI.RefreshForOpponentsRemoved(userData);
	}

	private void NetRoomUserStateChangedHandler(MPUserData userData)
	{
		_worldController.RefreshOpponentNodeStateWithData(userData);
		_worldUI.RefreshUserBoxForOpponentStateChanges(userData);
	}

	private void NetRoomUserModifiedHandler(MPUserData userData)
	{
		_worldController.RefreshOpponentWithModifiedData(userData);
	}

	private void NetRoomNodePlacementReceivedHandler(string netId, MPNodePlacementData placementData, bool fromNewNode)
	{
		_worldController.UpdateNetNodeWithUdpPlacementData(netId, placementData);
		if (fromNewNode)
		{
			_worldController.SendPlayerPlacementData();
		}
	}

	private void NetRoomWildMortyAddedHandler(MPOpponentData wildMortyData)
	{
		_worldController.CreateWildMortyNode(wildMortyData);
	}

	private void NetRoomWildMortyRemovedHandler(MPOpponentData wildMortyData)
	{
		_worldController.RemoveNetNode(wildMortyData.netId);
	}

	private void NetRoomWildMortyStateChangedHandler(MPOpponentData wildMortyData)
	{
		_worldController.RefreshOpponentNodeStateWithData(wildMortyData);
	}

	private void NetRoomPickupAddedHandler(MPPickupData pickup)
	{
		_worldController.CreatePickupNode(pickup);
	}

	private void NetRoomPickupRemovedHandler(MPPickupData pickup)
	{
		_worldController.RemoveNetNode(pickup.netId);
	}

	private void NetBattleStartHandler(string battleType, MPOpponentData enemyData)
	{
		MPBattleOpponent player = new MPBattleOpponent(App.net.player.data, eBattleOpponentSide.LEFT);
		MPBattleOpponent enemy = new MPBattleOpponent(enemyData, eBattleOpponentSide.RIGHT);
		MPSharedStateInfo.battleInfo = new MPBattleInfo(battleType, player, enemy, MPSharedStateInfo.worldInfo.worldId);
		App.net.room.Pause();
		App.states.Set<MPBattleState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}

	private void InviteQueueChangeHandler(List<MPBattleInviteData> invites, bool friendInvitePending)
	{
		if (base.status == eStateStatus.ACTIVE)
		{
			_worldUI.RefreshForBattleInvites(invites, friendInvitePending);
		}
	}

	private void SentInviteExpiredHandler()
	{
		if (base.status == eStateStatus.ACTIVE)
		{
			_worldUI.DisplayUserBoxDeclined();
		}
	}

	private void SentInviteDeclinedHandler()
	{
		if (base.status == eStateStatus.ACTIVE)
		{
			_worldUI.DisplayUserBoxDeclined();
		}
	}

	private void NetPlayerMortiesChangedHandler()
	{
		_worldController.RefreshOpponentWithModifiedData(App.net.player.data);
	}

	private void NetPlayerAvatarChangedHandler()
	{
		_worldController.RefreshOpponentWithModifiedData(App.net.player.data);
	}

	private void SoundTrackFinishedHandler()
	{
		if (base.status != eStateStatus.DEACTIVE && !IsTransitioning() && (base.status != eStateStatus.CACHED || leaveMusicPlayingOnCache))
		{
			if (_soundtrackQueue == null || _soundtrackQueue.Count == 0)
			{
				_soundtrackQueue = new List<string>(AudioDefs.ST_WORLD_ARR);
			}
			_soundtrackName = _soundtrackQueue[0];
			_soundtrackQueue.RemoveAt(0);
			App.sound.PlaySoundtrack(_soundtrackName, 0.2f, 0f, false);
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CActivateComplete_003E__BaseCallProxy0()
	{
		return base.ActivateComplete();
	}
}
