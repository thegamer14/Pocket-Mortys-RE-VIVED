public class TournamentAnnouncerTableUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private DialogueUI _dialogue;

	public event BagUIBtnDelegate ContinueEvent;

	protected override void LateAwake()
	{
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<DialogueUI>();
	}

	public void SetUp(string trainerID)
	{
		DisplayDialogue(trainerID);
	}

	private void DisplayDialogue(string trainerID)
	{
		DialogueData data = TournamentDefs.DialogueAnnouncer(trainerID);
		_dialogue.DisplaySpeech(data, null, delegate
		{
			DisplayDialogueComplete();
		});
	}

	private void DisplayDialogueComplete()
	{
		if (this.ContinueEvent != null)
		{
			this.ContinueEvent();
		}
	}
}
