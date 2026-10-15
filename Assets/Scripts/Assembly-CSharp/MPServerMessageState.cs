using System;
using UnityEngine;

public class MPServerMessageState : StateBase
{
	private DateTime _downFromDateTime;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPServerMessage/{0}", "MPServerMessage");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		MPNotificaionLayer.Instantiate();
		StartCoroutine(ActivateComplete());
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		App.net.session.GetServerMessage(delegate(MPServerMessageData messageData, eServerMessageError error)
		{
			if (messageData != null)
			{
				if (string.IsNullOrEmpty(messageData.showOnce) || App.saveData.multiplayer.lastServerMessageId != messageData.showOnce)
				{
					DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(messageData.validFrom);
					DateTime dateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(messageData.validUntil);
					if (dateTime < DateTime.UtcNow && dateTime2 > DateTime.UtcNow)
					{
						DisplayServerMessagePopup(messageData);
					}
				}
			}
			else if (error != eServerMessageError.NONE)
			{
				DisplayErrorAppPopup();
			}
			else
			{
				LoadNextState();
			}
		});
	}

	private void LoadNextState()
	{
		Type nextState = MPSharedStateInfo.serverMessage.nextState;
		MPSharedStateInfo.serverMessage = null;
		FadeTransInfo fadeTransInfo = new FadeTransInfo();
		fadeTransInfo.duration = 0.1f;
		App.states.Set(nextState, fadeTransInfo, new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}

	private void DisplayServerMessagePopup(MPServerMessageData messageData)
	{
		App.saveData.multiplayer.lastServerMessageId = messageData.showOnce;
		App.saveData.Save(eSaveDataType.MULTIPLAYER);
		_downFromDateTime = DateTime.MinValue;
		if (messageData.downFrom > 0)
		{
			_downFromDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			_downFromDateTime = _downFromDateTime.AddSeconds(messageData.downFrom).ToLocalTime();
		}
		string text = messageData.text;
		if (messageData.text != null && !string.IsNullOrEmpty(text) && _downFromDateTime != DateTime.MinValue)
		{
			text = string.Format(text, _downFromDateTime);
		}
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		string[] buttons = (IsDownTimeInFuture() ? new string[1] { "Continue" } : new string[1] { "Main Menu" });
		genericPopupState.SetDisplayValues(text, buttons, false);
		genericPopupState.ButtonClickedEvent += DisplayServerMessagePopupHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void DisplayErrorAppPopup()
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(string.Format(MPTextDefs.MP_SIGN_IN_ERROR, eNetSessionConnectError.FAILED), new string[1] { "Main Menu" }, false);
		genericPopupState.ButtonClickedEvent += DisplayErrorAppPopupHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private bool IsDownTimeInFuture()
	{
		return _downFromDateTime == DateTime.MinValue || _downFromDateTime > DateTime.UtcNow;
	}

	private void DisplayServerMessagePopupHandler(int buttonIndex)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.ResetButtonClickedEvent();
		if (IsDownTimeInFuture())
		{
			LoadNextState();
			return;
		}
		MPSharedStateInfo.serverMessage = null;
		App.states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}

	private void DisplayErrorAppPopupHandler(int buttonIndex)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.ResetButtonClickedEvent();
		MPSharedStateInfo.serverMessage = null;
		App.states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}
}
