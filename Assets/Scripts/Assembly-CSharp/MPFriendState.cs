using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MPFriendState : StateBase
{
	private MPFriendUI _friendUI;

	private bool _friendDataLoaded;

	private MPFriendData _removeFriendData;

	private MPBattleInviteData _invitedToBattleData;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPFriend/{0}", "MPFriend");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.net.friend.RefreshData();
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		CancelInvoke("RefreshFriendData");
		_friendDataLoaded = false;
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_friendUI = _ui as MPFriendUI;
		_friendUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_friendUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_friendUI.SelectedCellDataEvent += UISelectedCellHandler;
		_friendUI.AddFriendEvent += UIAddFriendHandler;
		_friendUI.RequestMortyEvent += UIRequestMortyHandler;
		_friendUI.BattleEvent += UIBattleHandler;
		_friendUI.DonateEvent += UIDonateHandler;
		_friendUI.AcceptEvent += UIAcceptHandler;
		_friendUI.DenyEvent += UIDenyHandler;
		_friendUI.RemoveEvent += UIRemoveHandler;
		_friendUI.BackEvent += UIBackHandler;
		App.net.friend.RefreshedDataEvent += NetFriendDataRefreshedHandler;
		App.net.battleInvite.InviteQueueChangeEvent += InviteQueueChangeHandler;
		App.net.battleInvite.SentInviteExpiredEvent += SentInviteExpiredHandler;
		App.net.battleInvite.SentInviteDeclinedEvent += SentInviteDeclinedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_friendUI.SelectedCellDataEvent -= UISelectedCellHandler;
		_friendUI.AddFriendEvent -= UIAddFriendHandler;
		_friendUI.RequestMortyEvent -= UIRequestMortyHandler;
		_friendUI.BattleEvent -= UIBattleHandler;
		_friendUI.DonateEvent -= UIDonateHandler;
		_friendUI.AcceptEvent -= UIAcceptHandler;
		_friendUI.DenyEvent -= UIDenyHandler;
		_friendUI.RemoveEvent -= UIRemoveHandler;
		_friendUI.BackEvent -= UIBackHandler;
		App.net.friend.RefreshedDataEvent -= NetFriendDataRefreshedHandler;
		App.net.battleInvite.InviteQueueChangeEvent -= InviteQueueChangeHandler;
		App.net.battleInvite.SentInviteExpiredEvent -= SentInviteExpiredHandler;
		App.net.battleInvite.SentInviteDeclinedEvent -= SentInviteDeclinedHandler;
	}

	public void RefreshFriendData()
	{
		App.net.friend.RefreshData();
	}

	public void DisplayWaitingForBattleInviteReplyPopup(MPFriendData friendData)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(string.Format(MPTextDefs.MP_FRIEND_BATTLE_INVITE_SENT_WAITING_POPUP, friendData.username), null, true, true);
		genericPopupState.ResetButtonClickedEvent(WaitingForBattleInviteReplyPopupHandler);
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void DisplaySentBattleInviteFailedPopup(string dialogue)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(dialogue, null);
		genericPopupState.ResetButtonClickedEvent(BattleSentInviteFailedPopupHandler);
		if (!genericPopupState.IsActive())
		{
			App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
	}

	private bool TryDisplayBattleInvitedPopupIfNeeded(List<MPBattleInviteData> invites)
	{
		if (App.net.battleInvite.sentInvitePending)
		{
			return true;
		}
		MPBattleInviteData mPBattleInviteData = invites.FirstOrDefault((MPBattleInviteData e) => e.type == "PvF");
		if (mPBattleInviteData != null)
		{
			DisplayBattleInvitedPopup(mPBattleInviteData);
			return true;
		}
		return false;
	}

	public void DisplayBattleInvitedPopup(MPBattleInviteData invitedToBattleData)
	{
		_invitedToBattleData = invitedToBattleData;
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(string.Format(MPTextDefs.MP_FRIEND_BATTLE_INVITE_RECEIVED_POPUP, _invitedToBattleData.userData.username), new string[1] { MPTextDefs.MP_FRIEND_BATTLE_INVITE_RECEIVED_POPUP_BTN });
		genericPopupState.ResetButtonClickedEvent(BattleInviteReplyPopupHandler);
		if (!genericPopupState.IsActive())
		{
			App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
	}

	private void UISelectedCellHandler(MPFriendData selectedFriendData)
	{
		MPSharedStateInfo.friendInfo.selectedFriendData = selectedFriendData;
	}

	private void UIAddFriendHandler()
	{
		App.states.Set<MPFriendAddState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIRequestMortyHandler()
	{
		App.states.Set<MPFriendRequestMortyState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBattleHandler(MPFriendData friendData)
	{
		DisplayWaitingForBattleInviteReplyPopup(friendData);
		App.net.battleInvite.SendInvite(friendData.userId, "PvF", delegate(eSendInviteError error)
		{
			switch (error)
			{
			case eSendInviteError.OPPONENT_STATE_INVALID:
				DisplaySentBattleInviteFailedPopup(MPTextDefs.MP_FRIEND_BATTLE_INVITE_SENT_DECLINED);
				break;
			default:
				DisplaySentBattleInviteFailedPopup(MPTextDefs.MP_FRIEND_BATTLE_INVITE_SENT_FAILED);
				break;
			case eSendInviteError.NONE:
				break;
			}
		});
	}

	private void UIDonateHandler(MPFriendData friendData)
	{
		MPSharedStateInfo.friendDonateMortyInfo = new MPFriendDonateMortyInfo(eMPStateOrigin.FRIEND, friendData);
		App.states.Set<MPFriendDonateMortyState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIAcceptHandler(MPFriendData friendData)
	{
		if (!App.net.friend.IsFriendListFull())
		{
			ActivateInput(false);
			_friendUI.StartCoroutine(_friendUI.ShowMenuWithTween(null));
			App.net.friend.RespondToRequest(friendData.userId, true, delegate(eFriendRespondToRequestError error)
			{
				if (error != eFriendRespondToRequestError.NONE)
				{
					_friendUI.DisplayAcceptRequestError(error);
				}
				ActivateInput(true);
			});
		}
		else
		{
			_friendUI.DisplayAcceptRequestError(eFriendRespondToRequestError.FRIEND_LIMIT_SELF);
		}
	}

	private void UIDenyHandler(MPFriendData friendData)
	{
		ActivateInput(false);
		_friendUI.StartCoroutine(_friendUI.ShowMenuWithTween(null));
		App.net.friend.RespondToRequest(friendData.userId, false, delegate(eFriendRespondToRequestError error)
		{
			if (error != eFriendRespondToRequestError.NONE)
			{
				_friendUI.DisplayDenyRequestError(error);
			}
			ActivateInput(true);
		});
	}

	private void UIRemoveHandler(MPFriendData friendData)
	{
		_removeFriendData = friendData;
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(string.Format(MPTextDefs.MP_FRIEND_REMOVE_POPUP, friendData.username), new string[1] { MPTextDefs.MP_FRIEND_REMOVE_POPUP_BTN });
		genericPopupState.ResetButtonClickedEvent(RemoveFriendPopupHandler);
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.friendInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void BattleSentInviteFailedPopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ResetButtonClickedEvent();
		if (!TryDisplayBattleInvitedPopupIfNeeded(App.net.battleInvite.GetReceivedInvites()))
		{
			App.states.SetBack(new PopTransInfo());
		}
	}

	private void WaitingForBattleInviteReplyPopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ResetButtonClickedEvent();
		App.net.battleInvite.CancelPendingSentInvite();
		if (!TryDisplayBattleInvitedPopupIfNeeded(App.net.battleInvite.GetReceivedInvites()))
		{
			App.states.SetBack(new PopTransInfo());
		}
	}

	private void BattleInviteReplyPopupHandler(int buttonIndex)
	{
		MPBattleInviteData invitedToBattleData = _invitedToBattleData;
		_invitedToBattleData = null;
		if (buttonIndex == 1)
		{
			App.net.battleInvite.RespondToInvite(invitedToBattleData, true);
		}
		else
		{
			App.net.battleInvite.RespondToInvite(invitedToBattleData, false);
		}
	}

	private void RemoveFriendPopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ResetButtonClickedEvent();
		if (buttonIndex == 1)
		{
			ActivateInput(false);
			App.net.friend.Remove(_removeFriendData.userId, delegate(eFriendRemoveError error)
			{
				if (error != eFriendRemoveError.NONE)
				{
					_friendUI.DisplayRemoveRequestError(_removeFriendData, error);
				}
				App.states.SetBack(new PopTransInfo());
				ActivateInput(true);
			});
		}
		else
		{
			App.states.SetBack(new PopTransInfo());
		}
	}

	private void NetFriendDataRefreshedHandler(List<MPFriendData> friendRequests, List<MPFriendData> friends, string mortyRequestId, float mortyRequestCountdown, MPFriendData mortyDonatedUser, int pollingInterval, eFriendRefreshDataError error)
	{
		if (IsTransitioningOut())
		{
			return;
		}
		if (error == eFriendRefreshDataError.NONE)
		{
			if (!_friendDataLoaded)
			{
				TryDisplayBattleInvitedPopupIfNeeded(App.net.battleInvite.GetReceivedInvites());
			}
			_friendUI.SetScrollMenu(friendRequests, friends, mortyRequestId, mortyRequestCountdown, mortyDonatedUser);
			CancelInvoke("RefreshFriendData");
			InvokeRepeating("RefreshFriendData", pollingInterval, pollingInterval);
			_friendDataLoaded = true;
		}
		else
		{
			_friendUI.DisplayRefreshDataError(error);
		}
	}

	private void InviteQueueChangeHandler(List<MPBattleInviteData> invites, bool friendInvitePending)
	{
		if (_friendDataLoaded && !TryDisplayBattleInvitedPopupIfNeeded(invites))
		{
			GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
			if (genericPopupState.IsActive())
			{
				genericPopupState.ResetButtonClickedEvent();
				App.states.SetBack(new PopTransInfo());
			}
		}
	}

	private void SentInviteExpiredHandler()
	{
		DisplaySentBattleInviteFailedPopup(MPTextDefs.MP_FRIEND_BATTLE_INVITE_SENT_DECLINED);
	}

	private void SentInviteDeclinedHandler()
	{
		DisplaySentBattleInviteFailedPopup(MPTextDefs.MP_FRIEND_BATTLE_INVITE_SENT_DECLINED);
	}
}
