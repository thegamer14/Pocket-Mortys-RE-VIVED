using System.Collections;
using UnityEngine;

public class TournamentBlackedOutUI : UIBase
{
	public delegate void TournamentBlackedOutUIDelegate();

	private Animator _animator;

	private MenuDialogue dialogue;

	public event TournamentBlackedOutUIDelegate FinishedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		_animator = GetComponent<Animator>();
		dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp()
	{
		dialogue.SetText(TextDefs.TOURNAMENT_BLACKED_OUT_DAIL_1);
		StartCoroutine(PlayAnimation());
	}

	public IEnumerator PlayAnimation()
	{
		yield return new WaitForSeconds(1f);
		App.sound.PlaySFX("Audio/SFX/BirdPersonCall");
		VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Blackout/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f);
		yield return StartCoroutine(_animator.PlayAndWait("Animation"));
		if (this.FinishedEvent != null)
		{
			this.FinishedEvent();
		}
	}

	public void AnimationSetNextText()
	{
		VoiceOverDefs.PlayRandom("Audio/VO/Birdperson Save/{0}", 0.6f, 0f, 0.7f);
		dialogue.SetText(TextDefs.TOURNAMENT_BLACKED_OUT_DAIL_2);
	}
}
