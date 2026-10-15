using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
	public delegate void DialogueDelegate();

	private const int _kInPositionY = 0;

	private const int _kOutPositionY = -800;

	private const float _kInOutDuration = 0.25f;

	private const float _kTextScrollSpeed = 0.01f;

	private const float _kCharacterNameBoxSize = 170f;

	private RectTransform _rectTrans;

	private Text _label;

	private Image _character;

	private RectTransform _characterNameBarBackground;

	private Text _characterNameLabel;

	private Button _btn;

	private Tween _textTween;

	private eDirection _side;

	public event DialogueDelegate ContinuePressedEvent;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_label = base.transform.FindChild("Box/Label").GetComponent<Text>();
		_btn = base.transform.FindChild("ContinueBtn").GetComponent<Button>();
		Transform transform = base.transform.FindChild("Character");
		if (transform != null)
		{
			_character = transform.GetComponent<Image>();
		}
		Transform transform2 = base.transform.FindChild("NameBar");
		if (transform2 != null)
		{
			_characterNameBarBackground = transform2.FindChild("Background").GetComponent<RectTransform>();
			_characterNameLabel = transform2.FindChild("Label").GetComponent<Text>();
		}
		ShowBtn(false);
	}

	public void SetUp(eDirection side)
	{
		_side = side;
	}

	public void ManualUpdate()
	{
		bool flag = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0);
		if (flag && _textTween != null && _textTween.IsPlaying())
		{
			_textTween.Complete();
		}
		if (_btn.gameObject.activeSelf && flag)
		{
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(_btn.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
			ExecuteEvents.Execute(_btn.gameObject, eventData, ExecuteEvents.submitHandler);
		}
	}

	public void SetText(string text)
	{
		_label.text = text;
	}

	public IEnumerator TweenText(string text)
	{
		SetText(string.Empty);
		_textTween = ShortcutExtensions46.DOText(duration: (float)text.Length * 0.01f, target: _label, endValue: text);
		_textTween.SetEase(Ease.Linear);
		yield return _textTween.WaitForCompletion();
	}

	public IEnumerator SetCharacterImage(string assetID)
	{
		if (!(_character == null))
		{
			_character.sprite = null;
			if (_side == eDirection.LEFT)
			{
				_character.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", assetID, "frontFlipped"));
			}
			if (_character.sprite == null)
			{
				_character.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", assetID, "front"));
			}
			_character.SetNativeSize();
			yield return new WaitForEndOfFrame();
			_character.gameObject.SetActive(_character.sprite != null);
		}
	}

	public void SetCharacterName(string name)
	{
		if (!(_characterNameLabel == null))
		{
			base.gameObject.SetActive(true);
			StartCoroutine(SetCharacterNameCo(name));
		}
	}

	private IEnumerator SetCharacterNameCo(string name)
	{
		_characterNameLabel.text = name;
		yield return new WaitForEndOfFrame();
		float width = _characterNameLabel.rectTransform.sizeDelta.x;
		_characterNameBarBackground.sizeDelta = new Vector2(width + 170f, _characterNameBarBackground.sizeDelta.y);
	}

	public void ShowBtn(bool isShow)
	{
		_btn.gameObject.SetActive(isShow);
	}

	public void SetPositionIn()
	{
		base.gameObject.SetActive(true);
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, 0f);
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, -800f);
		base.gameObject.SetActive(false);
	}

	public IEnumerator TweenIn()
	{
		base.gameObject.SetActive(true);
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, 0f), 0.25f);
		yield return tween.WaitForCompletion();
	}

	public IEnumerator TweenOut()
	{
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, -800f), 0.25f);
		yield return tween.WaitForCompletion();
		base.gameObject.SetActive(false);
	}

	public void ContinuePressedHandler()
	{
		if (this.ContinuePressedEvent != null)
		{
			this.ContinuePressedEvent();
		}
	}
}
