using System;
using UnityEngine;

public class StoryController : MonoBehaviour
{
	public delegate void StoryControllerDialogueDelegate(DialogueData dialogueData, Action<AdvancedNode> callback);

	public delegate void StoryControllerTrainerDelegate(TrainerNode trainerNode, bool isTutorial, Action<eBattleOutcomeType> Callback);

	public delegate void StoryControllerStoryDelegate(string id);

	public delegate void StoryControllerWorldDelegate(string id, string entranceID);

	protected StoryCamera _storyCamera;

	public event StoryControllerDialogueDelegate StartDialogueEvent;

	public event StoryControllerTrainerDelegate StartBattleEvent;

	public event StoryControllerStoryDelegate StartStoryEvent;

	public event StoryControllerWorldDelegate StartWorldEvent;

	protected virtual void Start()
	{
		_storyCamera = Camera.main.GetComponent<StoryCamera>();
	}

	protected void StartDialogue(DialogueData dialogueData, Action<AdvancedNode> callback)
	{
		if (this.StartDialogueEvent != null)
		{
			this.StartDialogueEvent(dialogueData, callback);
		}
	}

	protected void StartBattle(TrainerNode trainerNode, bool isTutorial, Action<eBattleOutcomeType> callback)
	{
		if (this.StartBattleEvent != null)
		{
			this.StartBattleEvent(trainerNode, isTutorial, callback);
		}
	}

	protected void StartStory(string id)
	{
		if (this.StartStoryEvent != null)
		{
			this.StartStoryEvent(id);
		}
	}

	protected void StartWorld(string id, string entranceID)
	{
		if (this.StartWorldEvent != null)
		{
			this.StartWorldEvent(id, entranceID);
		}
	}
}
