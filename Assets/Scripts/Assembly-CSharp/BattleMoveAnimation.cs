using System.Collections;
using UnityEngine;

public class BattleMoveAnimation : MonoBehaviour
{
	private Animator _animator;

	private BattleMorty _performingMorty;

	private BattleMorty _receivingMorty;

	private Transform _receivingMortyBarTrans;

	private Transform _receivingMortyOriginalParent;

	private int _receivingMortyOriginalSiblingIndex;

	private Transform _receivingMortyBarOriginalParent;

	private int _receivingMortyBarOriginalSiblingIndex;

	public void SetUp(BattleMorty performingMorty, BattleMorty receivingMorty, Transform receivingMortyBarTrans)
	{
		_performingMorty = performingMorty;
		_receivingMorty = receivingMorty;
		_receivingMortyBarTrans = receivingMortyBarTrans;
		_animator = GetComponent<Animator>();
		MoveMortyIntoAnimation();
	}

	public void MoveMortyIntoAnimation()
	{
		_receivingMortyOriginalParent = _receivingMorty.transform.parent;
		_receivingMortyOriginalSiblingIndex = _receivingMorty.transform.GetSiblingIndex();
		_receivingMortyBarOriginalParent = _receivingMortyBarTrans.parent;
		_receivingMortyBarOriginalSiblingIndex = _receivingMortyBarTrans.GetSiblingIndex();
		_receivingMorty.transform.SetParent(base.transform, false);
		_receivingMorty.transform.SetSiblingIndex(1);
		_receivingMortyBarTrans.SetParent(base.transform, false);
		_receivingMortyBarTrans.SetSiblingIndex(2);
	}

	public void MoveMortyOutOfAnimation()
	{
		_receivingMorty.transform.SetParent(_receivingMortyOriginalParent, false);
		_receivingMorty.transform.SetSiblingIndex(_receivingMortyOriginalSiblingIndex);
		_receivingMortyBarTrans.SetParent(_receivingMortyBarOriginalParent, false);
		_receivingMortyBarTrans.SetSiblingIndex(_receivingMortyBarOriginalSiblingIndex);
	}

	public IEnumerator Play()
	{
		yield return StartCoroutine(_animator.PlayAndWait("Animation"));
	}

	public void AttackingMortyTweenShake()
	{
		_performingMorty.TweenHit();
	}

	public void DefendingMortyTweenShake()
	{
		_receivingMorty.TweenHit();
	}

	public void AttackingMortyTweenOut()
	{
		_performingMorty.TweenOut();
	}

	public void DefendingMortyTweenOut()
	{
		_receivingMorty.TweenOut();
	}

	public void AttackingMortyTweenIn()
	{
		_performingMorty.TweenIn();
	}

	public void DefendingMortyTweenIn()
	{
		_receivingMorty.TweenIn();
	}

	public void AttackingMortyTweenFaint()
	{
		_performingMorty.TweenFaint();
	}

	public void DefendingMortyTweenFaint()
	{
		_receivingMorty.TweenFaint();
	}

	public void FlashState()
	{
		App.states.FlashCurrentState();
	}

	public void SFXChipBeep()
	{
		App.sound.PlaySFX("Audio/SFX/ChipBeep");
	}

	public void SFXChipFailed()
	{
		App.sound.PlaySFX("Audio/SFX/ChipFail");
	}

	public void SFXChipSucceed()
	{
		App.sound.PlaySFX("Audio/SFX/ChipSuccess");
	}
}
