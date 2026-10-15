using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MPWorldUI : UIBase
{
	public delegate void WorldUIDelegate();

	public delegate void WorldUIUserWithIniviteDelegate(MPBattleInviteData inviteData = null);

	public delegate void WorldUIUserSendInviteDelegate(string userId);

	public delegate void WorldUIBattleInviteResponseDelegate(MPBattleInviteData data, bool accept);

	public delegate void WorldUIBattleInviteInfoDelegate(MPBattleInviteData data);

	public delegate void WorldUIDirectionDelegate(eDirection direction);

	public delegate void WorldTouchDelegate(Vector2 position);

	public delegate void UIEmoteBarDelegate(string emoteId);

	private MPWorldTouch _worldTouch;

	private WorldInputUI _input;

	private MPWorldHudUI _hud;

	private MPChallengeNotificationUI _challengeNotification;

	private DialogueUI _dialogue;

	private MPUserBoxUI _userBox;

	private MPWorldEmoteBarUI _emoteBar;

	public event WorldUIDelegate InputFaceBtnAPressedEvent;

	public event WorldUIDelegate PausePressedEvent;

	public event WorldUIDelegate MapPressedEvent;

	public event WorldUIDelegate DialogueStartedEvent;

	public event WorldUIDelegate DialogueFinishedEvent;

	public event WorldUIDelegate UserBoxStartedEvent;

	public event WorldUIDelegate UserBoxFinishedEvent;

	public event WorldUIUserWithIniviteDelegate UserBoxClosedEvent;

	public event WorldUIUserWithIniviteDelegate UserBoxAcceptInviteEvent;

	public event WorldUIUserSendInviteDelegate UserBoxBattleInviteEvent;

	public event WorldUIUserSendInviteDelegate UserBoxAddFriendEvent;

	public event WorldUIBattleInviteResponseDelegate InviteResponseEvent;

	public event WorldUIBattleInviteInfoDelegate InviteInfoEvent;

	public event WorldUIDirectionDelegate InputDPadChangedEvent;

	public event WorldTouchDelegate WorldTouchEvent;

	public event UIEmoteBarDelegate EmoteBarBtnPressedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		_worldTouch = base.transform.FindChild("WorldTouch").GetComponent<MPWorldTouch>();
		_input = base.transform.FindChild("Input").GetComponent<WorldInputUI>();
		_hud = base.transform.FindChild("Hud").GetComponent<MPWorldHudUI>();
		_challengeNotification = base.transform.FindChild("ChallengeNotification").GetComponent<MPChallengeNotificationUI>();
		_dialogue = base.transform.FindChild("Dialogue").GetComponent<DialogueUI>();
		_userBox = base.transform.FindChild("UserBox").GetComponent<MPUserBoxUI>();
		_emoteBar = base.transform.FindChild("EmoteBar").GetComponent<MPWorldEmoteBarUI>();
	}

	public void SetUp()
	{
		_hud.SetUp();
		_emoteBar.SetUp();
		_userBox.SetPositionOut();
	}

	public void RefreshFromCache()
	{
		_hud.RefreshFromCache();
	}

	protected override void AddEvents()
	{
		_worldTouch.TouchDownEvent += WorldTouchDownHandler;
		_input.DPadChangedEvent += InputDPadChangedHandler;
		_input.FaceBtnAPressedEvent += InputFaceBtnAPressedHandler;
		_hud.PauseBtnPressedEvent += HudPauseBtnPressedHandler;
		_hud.MapBtnPressedEvent += HudMapBtnPressedHandler;
		_hud.EmoteBtnPressedEvent += HudEmoteBtnPressedHandler;
		_dialogue.DialogueStartedEvent += DialogueStartedHandler;
		_dialogue.DialogueFinishedEvent += DialogueFinishedHandler;
		_challengeNotification.InviteResponseEvent += BattleInviteResponseHandler;
		_challengeNotification.InfoEvent += BattleInviteInfoHandler;
		_userBox.StartedEvent += UserBoxStartedHandler;
		_userBox.FinishedEvent += UserBoxFinishedHandler;
		_userBox.CloseEvent += UserBoxCloseHandler;
		_userBox.BattleInviteEvent += UserBoxBattleInviteHandler;
		_userBox.AddFriendEvent += UserBoxAddFriendHandler;
		_userBox.AcceptInviteEvent += UserBoxAcceptInviteHandler;
		_emoteBar.BackBtnPressedEvent += EmoteBarBackBtnPressedHandler;
		_emoteBar.EmoteBtnPressedEvent += EmoteBarEmoteBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_worldTouch.TouchDownEvent -= WorldTouchDownHandler;
		_input.DPadChangedEvent -= InputDPadChangedHandler;
		_input.FaceBtnAPressedEvent -= InputFaceBtnAPressedHandler;
		_hud.PauseBtnPressedEvent -= HudPauseBtnPressedHandler;
		_hud.MapBtnPressedEvent -= HudMapBtnPressedHandler;
		_hud.EmoteBtnPressedEvent -= HudEmoteBtnPressedHandler;
		_dialogue.DialogueStartedEvent -= DialogueStartedHandler;
		_dialogue.DialogueFinishedEvent -= DialogueFinishedHandler;
		_challengeNotification.InviteResponseEvent -= BattleInviteResponseHandler;
		_challengeNotification.InfoEvent -= BattleInviteInfoHandler;
		_userBox.StartedEvent -= UserBoxStartedHandler;
		_userBox.FinishedEvent -= UserBoxFinishedHandler;
		_userBox.CloseEvent -= UserBoxCloseHandler;
		_userBox.BattleInviteEvent -= UserBoxBattleInviteHandler;
		_userBox.AddFriendEvent -= UserBoxAddFriendHandler;
		_userBox.AcceptInviteEvent -= UserBoxAcceptInviteHandler;
		_emoteBar.BackBtnPressedEvent -= EmoteBarBackBtnPressedHandler;
		_emoteBar.EmoteBtnPressedEvent -= EmoteBarEmoteBtnPressedHandler;
	}

	public override void ActivateInput(bool isActive)
	{
		base.ActivateInput(isActive);
		if (isActive && !_dialogue.inUse)
		{
			TweenIn();
		}
		else
		{
			TweenOut();
		}
	}

	public void SetInputForPlayerInteract(bool isInteractable)
	{
		_input.SetFaceButtonsForPlayerInteract(isInteractable);
	}

	public void DisplayDialogueSpeech(DialogueData dialogueData, AdvancedNode nodeTrigger = null, Action<AdvancedNode> callback = null)
	{
		if (_challengeNotification.state == eMPChallengeNotificationUIState.IN)
		{
			_challengeNotification.TweenOut();
		}
		_dialogue.DisplaySpeech(dialogueData, nodeTrigger, callback);
	}

	public void DisplayDialogueAction(string text, AdvancedNode nodeTrigger = null, Action<AdvancedNode> callback = null)
	{
		if (_challengeNotification.state == eMPChallengeNotificationUIState.IN)
		{
			_challengeNotification.TweenOut();
		}
		_dialogue.DisplayAction(text, nodeTrigger, callback);
	}

	public void DisplayUserBoxToChallenge(MPUserData data)
	{
		_userBox.SetStateDefault(data);
	}

	public void DisplayUserBoxForInviteResponse(MPBattleInviteData data)
	{
		_userBox.SetStateBattleInviteReceived(data);
	}

	public void DisplayUserBoxWaiting()
	{
		_userBox.SetStateWaiting();
	}

	public void DisplayUserBoxDeclined()
	{
		_userBox.SetStateBattleInviteDeclined();
	}

	public void DisplayUserBoxFailed()
	{
		_userBox.SetStateBattleInviteFailed();
	}

	public void CloseUserBox()
	{
		StartCoroutine(_userBox.TweenOut());
	}

	public void DisplayUserBoxFriendAdded(eFriendSendRequestError error)
	{
		_userBox.SetStateAddFriendComplete(error);
	}

	private void RefreshUserBoxForInvites(List<MPBattleInviteData> invites)
	{
		if (_userBox.state == eUserBoxState.DEFAULT || _userBox.state == eUserBoxState.BATTLE_INVITE_DECLINED)
		{
			if (invites.Count > 0)
			{
				MPBattleInviteData mPBattleInviteData = invites.FirstOrDefault((MPBattleInviteData i) => i.userData.netId.Equals(_userBox.userData.netId));
				if (mPBattleInviteData != null)
				{
					_userBox.SetStateBattleInviteReceived(mPBattleInviteData);
				}
			}
		}
		else if (_userBox.state == eUserBoxState.BATTLE_INVITE_RECEIVED && _userBox.battleInviteData != null && !invites.Any((MPBattleInviteData i) => i.id.Equals(_userBox.battleInviteData.id)))
		{
			_userBox.SetStateDefault(_userBox.battleInviteData.userData);
		}
	}

	public void RefreshUserBoxForOpponentStateChanges(MPUserData userData)
	{
		if (_userBox.state == eUserBoxState.DEFAULT && userData.netId.Equals(_userBox.userData.netId))
		{
			_userBox.RefreshUserStateChanged();
		}
	}

	private void RefreshNotificationForInvites(List<MPBattleInviteData> invites)
	{
		if (_challengeNotification.state == eMPChallengeNotificationUIState.IN)
		{
			if (_challengeNotification.data != null && !invites.Any((MPBattleInviteData i) => i.id.Equals(_challengeNotification.data.id)))
			{
				_challengeNotification.TweenOut();
			}
		}
		else if (_challengeNotification.state == eMPChallengeNotificationUIState.OUT && _userBox.state == eUserBoxState.NONE && !_dialogue.inUse)
		{
			MPBattleInviteData mPBattleInviteData = invites.FirstOrDefault((MPBattleInviteData e) => e.type == "PvP");
			if (mPBattleInviteData != null)
			{
				_challengeNotification.SetUp(mPBattleInviteData);
				_challengeNotification.TweenIn();
			}
		}
	}

	public void HideChallengeNotification()
	{
		if (_challengeNotification.state == eMPChallengeNotificationUIState.IN)
		{
			_challengeNotification.TweenOut();
		}
	}

	public void RefreshForBattleInvites(List<MPBattleInviteData> invites, bool friendInvitePending)
	{
		RefreshUserBoxForInvites(invites);
		RefreshNotificationForInvites(invites);
		_hud.RefreshForBattleInvites(friendInvitePending);
	}

	public void RefreshForOpponentsRemoved(MPUserData userData)
	{
		if (_userBox.state == eUserBoxState.DEFAULT && userData.netId.Equals(_userBox.userData.netId))
		{
			_userBox.RefreshUserLeft();
		}
	}

	private void DisplayEmoteBar()
	{
		TweenOut();
		_emoteBar.TweenIn();
	}

	private void HideEmoteBar()
	{
		_emoteBar.TweenOut();
		TweenIn();
	}

	public void TweenIn()
	{
		_worldTouch.enabled = true;
		_input.TweenIn();
		_hud.TweenIn();
	}

	public void TweenOut()
	{
		_worldTouch.enabled = false;
		_input.TweenOut();
		_hud.TweenOut();
	}

	private void WorldTouchDownHandler(Vector2 screenPoint)
	{
		if (this.WorldTouchEvent != null)
		{
			this.WorldTouchEvent(screenPoint);
		}
	}

	private void InputDPadChangedHandler(eDirection direction)
	{
		if (this.InputDPadChangedEvent != null)
		{
			this.InputDPadChangedEvent(direction);
		}
	}

	private void InputFaceBtnAPressedHandler()
	{
		if (this.InputFaceBtnAPressedEvent != null)
		{
			this.InputFaceBtnAPressedEvent();
		}
	}

	private void HudPauseBtnPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.PausePressedEvent != null)
		{
			this.PausePressedEvent();
		}
	}

	private void HudMapBtnPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.MapPressedEvent != null)
		{
			this.MapPressedEvent();
		}
	}

	private void HudEmoteBtnPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		DisplayEmoteBar();
	}

	private void DialogueStartedHandler()
	{
		if (this.DialogueStartedEvent != null)
		{
			this.DialogueStartedEvent();
		}
	}

	private void DialogueFinishedHandler()
	{
		if (this.DialogueFinishedEvent != null)
		{
			this.DialogueFinishedEvent();
		}
	}

	private void BattleInviteResponseHandler(MPBattleInviteData data, bool accept)
	{
		if (this.InviteResponseEvent != null)
		{
			this.InviteResponseEvent(data, accept);
		}
	}

	private void BattleInviteInfoHandler(MPBattleInviteData data)
	{
		if (this.InviteInfoEvent != null)
		{
			this.InviteInfoEvent(data);
		}
	}

	private void UserBoxStartedHandler()
	{
		if (this.UserBoxStartedEvent != null)
		{
			this.UserBoxStartedEvent();
		}
	}

	private void UserBoxFinishedHandler()
	{
		if (this.UserBoxFinishedEvent != null)
		{
			this.UserBoxFinishedEvent();
		}
	}

	private void UserBoxCloseHandler(MPBattleInviteData inviteData = null)
	{
		if (this.UserBoxClosedEvent != null)
		{
			this.UserBoxClosedEvent(inviteData);
		}
	}

	private void UserBoxBattleInviteHandler(string userId)
	{
		if (this.UserBoxBattleInviteEvent != null)
		{
			this.UserBoxBattleInviteEvent(userId);
		}
	}

	private void UserBoxAddFriendHandler(string userId)
	{
		if (this.UserBoxAddFriendEvent != null)
		{
			this.UserBoxAddFriendEvent(userId);
		}
	}

	private void UserBoxAcceptInviteHandler(MPBattleInviteData inviteData = null)
	{
		if (this.UserBoxAcceptInviteEvent != null)
		{
			this.UserBoxAcceptInviteEvent(inviteData);
		}
	}

	private void EmoteBarEmoteBtnPressedHandler(string emoteId)
	{
		if (this.EmoteBarBtnPressedEvent != null)
		{
			this.EmoteBarBtnPressedEvent(emoteId);
		}
		HideEmoteBar();
	}

	private void EmoteBarBackBtnPressedHandler()
	{
		HideEmoteBar();
	}
}
