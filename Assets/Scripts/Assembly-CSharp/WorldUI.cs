using System;

public class WorldUI : UIBase
{
	public delegate void WorldUIDelegate();

	public delegate void WorldUIDirectionDelegate(eDirection direction);

	private WorldPauseUI _pause;

	private WorldHudUI _hud;

	private WorldInputUI _input;

	private DialogueUI _dialogue;

	public bool tweenInputOnActivate;

	public event WorldUIDelegate PausePressedEvent;

	public event WorldUIDelegate PortalGunPressedEvent;

	public event WorldUIDelegate InputFaceBtnAPressedEvent;

	public event WorldUIDelegate DialogueStartedEvent;

	public event WorldUIDelegate DialogueFinishedEvent;

	public event WorldUIDirectionDelegate InputDPadChangedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		_pause = base.transform.FindChild("Pause").GetComponent<WorldPauseUI>();
		_hud = base.transform.FindChild("Hud").GetComponent<WorldHudUI>();
		_input = base.transform.FindChild("Input").GetComponent<WorldInputUI>();
		_dialogue = base.transform.FindChild("Dialogue").GetComponent<DialogueUI>();
	}

	public void SetUp(bool showObjectives, bool itemsComplete, bool trainersComplete, bool gymLeaderComplete, bool tweenInputOnActivate)
	{
		_pause.SetUp();
		_hud.SetUp(showObjectives);
		_hud.SetItemsTicked(itemsComplete, false);
		_hud.SetTrainersTicked(trainersComplete, false);
		_hud.SetGymLeaderTicked(gymLeaderComplete, false);
		this.tweenInputOnActivate = tweenInputOnActivate;
	}

	protected override void AddEvents()
	{
		_pause.MPadPressedEvent += PauseMPadPressedHandler;
		_pause.PortalGunPressedEvent += PausePortalGunPressedHandler;
		_input.DPadChangedEvent += InputDPadChangedHandler;
		_input.FaceBtnAPressedEvent += InputFaceBtnAPressedHandler;
		_dialogue.DialogueStartedEvent += DialogueStartedHandler;
		_dialogue.DialogueFinishedEvent += DialogueFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		_pause.MPadPressedEvent -= PauseMPadPressedHandler;
		_pause.PortalGunPressedEvent -= PausePortalGunPressedHandler;
		_input.DPadChangedEvent -= InputDPadChangedHandler;
		_input.FaceBtnAPressedEvent -= InputFaceBtnAPressedHandler;
		_dialogue.DialogueStartedEvent -= DialogueStartedHandler;
		_dialogue.DialogueFinishedEvent -= DialogueFinishedHandler;
	}

	public void SetHudItemsObjectTicked()
	{
		_hud.SetItemsTicked(true, true);
	}

	public void SetHudTrainersObjectTicked()
	{
		_hud.SetTrainersTicked(true, true);
	}

	public void SetHudGymLeadersObjectTicked()
	{
		_hud.SetGymLeaderTicked(true, true);
	}

	public override void ActivateInput(bool isActive)
	{
		base.ActivateInput(isActive);
		if (tweenInputOnActivate)
		{
			if (isActive && !_dialogue.inUse)
			{
				TweenIn();
			}
			else
			{
				TweenOut();
			}
		}
	}

	public void SetInputForPlayerInteract(bool isInteractable)
	{
		_input.SetFaceButtonsForPlayerInteract(isInteractable);
	}

	public void DisplayDialogueSpeech(DialogueData dialogueData, AdvancedNode nodeTrigger = null, Action<AdvancedNode> callback = null)
	{
		_dialogue.DisplaySpeech(dialogueData, nodeTrigger, callback);
	}

	public void DisplayDialogueAction(string text, AdvancedNode nodeTrigger = null, Action<AdvancedNode> callback = null)
	{
		_dialogue.DisplayAction(text, nodeTrigger, callback);
	}

	public void TweenIn()
	{
		if (PlayerDefs.CurrentAlertNotificationID == null)
		{
			TweenAllInExcludingPause();
		}
		else if (PlayerDefs.CurrentAlertNotificationID.Equals("AlertTutorialUseMPadID"))
		{
			TweenInOnlyPause();
		}
		else
		{
			TweenAllIn();
		}
	}

	public void TweenOut()
	{
		_pause.TweenOut();
		_hud.TweenOut();
		_input.TweenOut();
	}

	public void TweenAllIn()
	{
		_pause.TweenIn();
		_hud.TweenIn();
		_input.TweenIn();
	}

	public void TweenAllInExcludingPause()
	{
		_hud.TweenIn();
		_input.TweenIn();
	}

	public void TweenInOnlyPause()
	{
		_pause.TweenIn();
	}

	private void PauseMPadPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.PausePressedEvent != null)
		{
			this.PausePressedEvent();
		}
	}

	private void PausePortalGunPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.PortalGunPressedEvent != null)
		{
			this.PortalGunPressedEvent();
		}
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
}
