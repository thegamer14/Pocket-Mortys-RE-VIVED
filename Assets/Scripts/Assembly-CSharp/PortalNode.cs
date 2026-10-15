using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PortalNode : AdvancedNode
{
	private const int _kRotationSpeed1 = 120;

	private const int _kRotationSpeed2 = 160;

	private const int _kRotationSpeed3 = 200;

	private const int _kRotationSpeed4 = 220;

	private Transform _parts;

	private Transform _part1;

	private Transform _part2;

	private Transform _part3;

	private Transform _part4;

	private AudioSource _audioLoop;

	private void Awake()
	{
		_parts = base.transform.FindChild("Parts");
		_part1 = _parts.FindChild("Part1");
		_part2 = _parts.FindChild("Part2");
		_part3 = _parts.FindChild("Part3");
		_part4 = _parts.FindChild("Part4");
		_audioLoop = GetComponent<AudioSource>();
	}

	private void Update()
	{
		_part1.Rotate(0f, 120f * Time.deltaTime, 0f);
		_part2.Rotate(0f, 160f * Time.deltaTime, 0f);
		_part3.Rotate(0f, 200f * Time.deltaTime, 0f);
		_part4.Rotate(0f, 220f * Time.deltaTime, 0f);
	}

	public IEnumerator Open()
	{
		App.sound.PlaySFX("Audio/SFX/PortalOpen");
		base.transform.localScale = Vector3.zero;
		Tween scaleTween = base.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.Linear);
		yield return scaleTween.WaitForCompletion();
		_parts.DOShakeScale(3f, 0.2f, 3, 0f);
		if (_audioLoop != null)
		{
			_audioLoop.mute = false;
		}
	}

	public IEnumerator Close(bool destoryOnClose = false)
	{
		if (_audioLoop != null)
		{
			_audioLoop.mute = true;
		}
		App.sound.PlaySFX("Audio/SFX/PortalClose");
		base.transform.localScale = Vector3.one;
		Tween scaleTween = base.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear);
		yield return scaleTween.WaitForCompletion();
		if (destoryOnClose)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public override void InteractionStarted(eDirection direction, bool isTrigger)
	{
		base.InteractionStarted(direction, isTrigger);
		if (direction == eDirection.DOWN)
		{
			App.sound.PlaySFX("Audio/SFX/PortalUse");
		}
	}
}
