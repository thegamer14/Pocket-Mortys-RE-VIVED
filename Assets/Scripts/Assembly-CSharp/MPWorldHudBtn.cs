using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPWorldHudBtn : MonoBehaviour
{
	public delegate void PasueUIDelegate();

	private Button _btn;

	private Image _image;

	private Sequence _shakeTween;

	public bool enableBackBtn;

	public bool interactable
	{
		get
		{
			return _btn.IsInteractable();
		}
		set
		{
			_btn.interactable = value;
		}
	}

	public event PasueUIDelegate PressedEvent;

	private void Awake()
	{
		_btn = base.transform.FindChild("Button").GetComponent<Button>();
		_image = base.transform.FindChild("Button").GetComponent<Image>();
	}

	public void SetUp()
	{
		AddEvents(true);
	}

	private void AddEvents(bool value)
	{
		if (value)
		{
			App.states.BackButtonEvent += BackButtonHandler;
		}
		else
		{
			App.states.BackButtonEvent -= BackButtonHandler;
		}
	}

	private void OnDestroy()
	{
		AddEvents(false);
	}

	public void TweenShake(bool enable)
	{
		if (!enable && _shakeTween != null)
		{
			_shakeTween.Kill();
			_shakeTween = null;
		}
		else if (enable)
		{
			_shakeTween = DOTween.Sequence();
			_shakeTween.Append(_image.rectTransform.DOLocalRotate(new Vector3(0f, 0f, -30f), 0.2f));
			_shakeTween.Append(_image.rectTransform.DOLocalRotate(new Vector3(0f, 0f, 25f), 0.2f));
			_shakeTween.Append(_image.rectTransform.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.2f));
			_shakeTween.Append(_image.rectTransform.DOLocalRotate(new Vector3(0f, 0f, 15f), 0.2f));
			_shakeTween.Append(_image.rectTransform.DOLocalRotate(new Vector3(0f, 0f, -10f), 0.2f));
			_shakeTween.Append(_image.rectTransform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.2f));
			_shakeTween.AppendInterval(1f);
			_shakeTween.SetLoops(-1, LoopType.Restart);
			_shakeTween.Play();
		}
	}

	public void BtnPressedHandler()
	{
		if (this.PressedEvent != null)
		{
			this.PressedEvent();
		}
	}

	private void BackButtonHandler()
	{
		if (interactable && enableBackBtn)
		{
			BtnPressedHandler();
		}
	}
}
