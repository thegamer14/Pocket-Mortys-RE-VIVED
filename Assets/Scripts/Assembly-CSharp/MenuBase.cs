using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MenuBase : MonoBehaviour
{
	public delegate void MenuBaseDelegate(int index);

	public float outPositionY = -190f;

	public float inOutSpeed = 800f;

	private RectTransform _rectTrans;

	private CanvasGroup _canvasGroup;

	protected MenuBtnBase[] _btns;

	private float _startingPositionY;

	public bool isIn { get; private set; }

	public event MenuBaseDelegate BtnPressedEvent;

	public event MenuBaseDelegate BtnLongPressStartEvent;

	protected virtual void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_canvasGroup = GetComponent<CanvasGroup>();
		_btns = base.gameObject.GetComponentsInChildren<MenuBtnBase>();
		_startingPositionY = base.transform.localPosition.y;
	}

	private void Start()
	{
		AddEvents(true);
	}

	private void AddEvents(bool value)
	{
		if (value)
		{
			for (int i = 0; i < _btns.Length; i++)
			{
				_btns[i].PressedEvent += BtnPressedHandler;
				_btns[i].LongPressStartEvent += BtnLongPressStartHandler;
			}
		}
		else
		{
			for (int j = 0; j < _btns.Length; j++)
			{
				_btns[j].PressedEvent -= BtnPressedHandler;
				_btns[j].LongPressStartEvent -= BtnLongPressStartHandler;
			}
		}
	}

	private void OnDestroy()
	{
		AddEvents(false);
	}

	public virtual void SetBtnVisible(int index, bool isInvisible)
	{
		_btns[index].gameObject.SetActive(isInvisible);
	}

	public virtual void SetBtnEnabled(int index, bool isEnabled)
	{
		_btns[index].SetEnabled(isEnabled);
	}

	public virtual void SetInteractable(bool isInteractable)
	{
		_canvasGroup.interactable = isInteractable;
	}

	public void SetBtnTitle(int index, string title)
	{
		_btns[index].SetTitle(title);
	}

	public void SetBtnPosition(int index, Vector3 position, bool isRelative = false)
	{
		if (!isRelative)
		{
			_btns[index].transform.localPosition = position;
		}
		else
		{
			_btns[index].transform.localPosition += position;
		}
	}

	public void SetPositionIn()
	{
		base.gameObject.SetActive(true);
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY);
		SetInteractable(true);
		isIn = true;
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, outPositionY);
		SetInteractable(false);
		isIn = false;
		base.gameObject.SetActive(false);
	}

	public IEnumerator TweenIn(float durationOverride = -1f)
	{
		StopAllCoroutines();
		base.gameObject.SetActive(true);
		yield return StartCoroutine(TweenInCo(durationOverride));
	}

	private IEnumerator TweenInCo(float durationOverride = -1f)
	{
		base.transform.DOKill();
		Tween tween = ShortcutExtensions46.DOAnchorPos(duration: (!(durationOverride < 0f)) ? durationOverride : Mathf.Abs((base.transform.localPosition.y - _startingPositionY) / inOutSpeed), target: _rectTrans, endValue: new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY));
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
		SetInteractable(true);
		isIn = true;
	}

	public IEnumerator TweenOut(float durationOverride = -1f)
	{
		StopAllCoroutines();
		if (base.gameObject.activeSelf)
		{
			yield return StartCoroutine(TweenOutCo(durationOverride));
		}
		base.gameObject.SetActive(false);
	}

	private IEnumerator TweenOutCo(float durationOverride = -1f)
	{
		base.transform.DOKill();
		SetInteractable(false);
		isIn = false;
		Tween tween = ShortcutExtensions46.DOAnchorPos(duration: (!(durationOverride < 0f)) ? durationOverride : Mathf.Abs((base.transform.localPosition.y - outPositionY) / inOutSpeed), target: _rectTrans, endValue: new Vector2(_rectTrans.anchoredPosition.x, outPositionY));
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
	}

	public IEnumerator TweenOutFor(MenuBase inMenu)
	{
		StopAllCoroutines();
		yield return StartCoroutine(TweenOut());
		yield return StartCoroutine(inMenu.TweenIn());
	}

	public void BtnPressedHandler(MenuBtnBase btn)
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.BtnPressedEvent != null)
		{
			this.BtnPressedEvent(Array.IndexOf(_btns, btn));
		}
	}

	public void BtnLongPressStartHandler(MenuBtnBase btn)
	{
		if (this.BtnLongPressStartEvent != null)
		{
			this.BtnLongPressStartEvent(Array.IndexOf(_btns, btn));
		}
	}
}
