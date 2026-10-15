using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EvolveBackground : MonoBehaviour
{
	private const float _kBackgroundOutDuration = 0.6f;

	private const float _kLayer1Speed = 1f;

	private const float _kLayer2Speed = 2f;

	private CanvasGroup _background;

	private RawImage _backgroundLayer1;

	private RawImage _backgroundLayer2;

	private void Awake()
	{
		_background = base.transform.FindChild("Background").GetComponent<CanvasGroup>();
		_backgroundLayer1 = _background.transform.FindChild("Layer1").GetComponent<RawImage>();
		_backgroundLayer2 = _background.transform.FindChild("Layer2").GetComponent<RawImage>();
	}

	private void Start()
	{
		_background.alpha = 0f;
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (_background.alpha > 0f)
		{
			Rect uvRect = _backgroundLayer1.uvRect;
			uvRect.y -= 1f * Time.deltaTime;
			_backgroundLayer1.uvRect = uvRect;
			Rect uvRect2 = _backgroundLayer2.uvRect;
			uvRect2.y -= 2f * Time.deltaTime;
			_backgroundLayer2.uvRect = uvRect2;
		}
	}

	public IEnumerator TweenBackgroundOut()
	{
		Tween tween = _background.DOFade(0f, 0.6f);
		yield return tween.WaitForCompletion();
	}

	public IEnumerator TweenBackground(eBattleOpponentSide side = eBattleOpponentSide.LEFT)
	{
		base.gameObject.SetActive(true);
		_background.alpha = 1f;
		base.transform.localScale = ((side != eBattleOpponentSide.LEFT) ? new Vector3(-1f, 1f, 1f) : Vector3.one);
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(TweenBackgroundOut());
	}
}
