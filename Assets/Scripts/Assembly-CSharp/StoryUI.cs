using System;

public class StoryUI : UIBase
{
	public delegate void WorldUIDelegate();

	private DialogueUI _dialogue;

	public event WorldUIDelegate DialogueStartedEvent;

	public event WorldUIDelegate DialogueFinishedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		_dialogue = base.transform.FindChild("Dialogue").GetComponent<DialogueUI>();
	}

	protected override void AddEvents()
	{
		_dialogue.DialogueStartedEvent += DialogueStartedHandler;
		_dialogue.DialogueFinishedEvent += DialogueFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		_dialogue.DialogueStartedEvent -= DialogueStartedHandler;
		_dialogue.DialogueFinishedEvent -= DialogueFinishedHandler;
	}

	public void DisplayDialogueSpeech(DialogueData dialogueData, AdvancedNode nodeTrigger = null, Action<AdvancedNode> callback = null)
	{
		_dialogue.DisplaySpeech(dialogueData, nodeTrigger, callback);
	}

	public void DisplayDialogueAction(string text, AdvancedNode nodeTrigger = null, Action<AdvancedNode> callback = null)
	{
		_dialogue.DisplayAction(text, nodeTrigger, callback);
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
}
