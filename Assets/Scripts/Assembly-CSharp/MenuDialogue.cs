using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuDialogue : MonoBehaviour
{
	private const float _kTextScrollDuration = 0.2f;

	private Text _label;

	private RectTransform _labelTrans;

	private GameObject _nextIconObj;

	private bool _skipText;

	private bool _isSkippable;

	private float _readDelayCountdown;

	private void Awake()
	{
		_labelTrans = (RectTransform)base.transform.FindChild("LabelMask/Label");
		_label = _labelTrans.GetComponent<Text>();
		_nextIconObj = base.transform.FindChild("NextIcon").GetComponent<Image>().gameObject;
		SetUpNextIcon();
	}

	private void Update()
	{
		if (!_skipText && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)))
		{
			_skipText = true;
		}
	}

	public bool IsReadDelayActive()
	{
		return _readDelayCountdown > 0f;
	}

	public string GetText()
	{
		return _label.text;
	}

	public void SetText(string text)
	{
		_labelTrans.DOKill();
		StopAllCoroutines();
		_label.text = text;
	}

	public IEnumerator SetTextCo(string text, float readDelay = 0f, bool isSkippable = false)
	{
		_labelTrans.DOKill();
		StopAllCoroutines();
		yield return StartCoroutine(SetTextCoPrivate(false, text, readDelay, isSkippable));
	}

	public void TweenText(string text)
	{
		StartCoroutine(TweenTextCo(text));
	}

	public IEnumerator TweenTextCo(string text, float readDelay = 0f, bool isSkippable = false)
	{
		_labelTrans.DOKill();
		StopAllCoroutines();
		yield return StartCoroutine(SetTextCoPrivate(true, text, readDelay, isSkippable));
	}

	private IEnumerator SetTextCoPrivate(bool animate, string text, float readDelay, bool isSkippable = false)
	{
		_skipText = false;
		_readDelayCountdown = readDelay;
		if (animate)
		{
			yield return StartCoroutine(AnimateText(text));
		}
		else
		{
			_label.text = text;
		}
		ShowNextPageIcon(isSkippable);
		while ((_readDelayCountdown > 0f || readDelay < 0f) && (!_skipText || !_isSkippable))
		{
			_readDelayCountdown -= Time.deltaTime;
			yield return null;
		}
		if (_readDelayCountdown > 0f)
		{
			App.sound.PlaySFX("Audio/SFX/GenericButton");
		}
		_readDelayCountdown = 0f;
		ShowNextPageIcon(false);
	}

	private IEnumerator AnimateText(string text)
	{
		Tween tween = _labelTrans.DOAnchorPos(new Vector2(0f, 84f), 0.2f);
		tween.SetEase(Ease.InCubic);
		yield return tween.WaitForCompletion();
		_label.text = text;
		_labelTrans.anchoredPosition = new Vector2(0f, -76f);
		tween = _labelTrans.DOAnchorPos(new Vector2(0f, 4f), 0.2f);
		tween.SetEase(Ease.OutBack);
		yield return tween.WaitForCompletion();
	}

	private void SetUpNextIcon()
	{
		RectTransform target = (RectTransform)_nextIconObj.transform;
		target.DOAnchorPos(new Vector2(0f, 34f), 0.4f).SetLoops(-1, LoopType.Yoyo).SetRelative();
		ShowNextPageIcon(false);
	}

	public void ShowNextPageIcon(bool value)
	{
		_isSkippable = value;
		_nextIconObj.SetActive(_isSkippable);
	}
}
