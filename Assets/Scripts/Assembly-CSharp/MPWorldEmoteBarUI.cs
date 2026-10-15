using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MPWorldEmoteBarUI : MonoBehaviour
{
	public delegate void UIEmoteBtnDelegate(string emoteId);

	public delegate void UIBtnDelegate();

	private const int _kInPositionY = 160;

	private const int _kOutPositionY = -200;

	private const float _kInOutDuration = 0.3f;

	private RectTransform _rectTrans;

	private CanvasGroup _canvasGroup;

	public event UIEmoteBtnDelegate EmoteBtnPressedEvent;

	public event UIBtnDelegate BackBtnPressedEvent;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_canvasGroup = GetComponent<CanvasGroup>();
	}

	public void SetUp()
	{
		SetPositionOut();
	}

	public void SetPositionIn()
	{
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, 160f);
		_canvasGroup.interactable = true;
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, -200f);
		_canvasGroup.interactable = false;
	}

	public void TweenIn()
	{
		StartCoroutine(TweenInCo());
	}

	public IEnumerator TweenInCo()
	{
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, 160f), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
		_canvasGroup.interactable = true;
	}

	public void TweenOut()
	{
		StartCoroutine(TweenOutCo());
	}

	public IEnumerator TweenOutCo()
	{
		_canvasGroup.interactable = false;
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, -200f), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
	}

	public void EmoteBtnPressed(string emoteId)
	{
		if (this.EmoteBtnPressedEvent != null)
		{
			this.EmoteBtnPressedEvent(emoteId);
		}
	}

	public void BackBtnPressed()
	{
		if (this.BackBtnPressedEvent != null)
		{
			this.BackBtnPressedEvent();
		}
	}
}
