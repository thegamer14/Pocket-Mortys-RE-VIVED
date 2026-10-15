using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldInputFaceBtnsUI : MonoBehaviour
{
	public delegate void WorldInputFaceBtnsDelegate();

	private const int _kOutPositionY = -220;

	private const float _kInOutDuration = 0.3f;

	public Sprite avilableASprite;

	private RectTransform _rectTrans;

	private Button _aBtn;

	private float _startingPositionY;

	private Sprite unavilableASprite;

	public event WorldInputFaceBtnsDelegate PressedEvent;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_aBtn = base.transform.FindChild("A").GetComponent<Button>();
		_startingPositionY = base.transform.localPosition.y;
		_aBtn.enabled = false;
		unavilableASprite = _aBtn.image.sprite;
	}

	private void Update()
	{
		if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Space)) && _aBtn.IsActive())
		{
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(_aBtn.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
			ExecuteEvents.Execute(_aBtn.gameObject, eventData, ExecuteEvents.submitHandler);
		}
	}

	public void SetABtnSparkleActive(bool isActive)
	{
		_aBtn.image.sprite = ((!isActive) ? unavilableASprite : avilableASprite);
		_aBtn.interactable = isActive;
	}

	public void SetPositionIn()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY);
		_aBtn.enabled = true;
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, -220f);
		_aBtn.enabled = false;
	}

	public IEnumerator TweenIn()
	{
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY), 0.3f);
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
		_aBtn.enabled = true;
	}

	public IEnumerator TweenOut()
	{
		_aBtn.enabled = false;
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, -220f), 0.3f);
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
	}

	public void ABtnPressed()
	{
		if (this.PressedEvent != null)
		{
			this.PressedEvent();
		}
	}
}
