using UnityEngine;

public class WorldInputUI : MonoBehaviour
{
	public delegate void WorldInputDelegate();

	public delegate void WorldInputDirectionDelegate(eDirection direction);

	private WorldInputDPadUI _dPad;

	private WorldInputFaceBtnsUI _faceBtns;

	public event WorldInputDelegate FaceBtnAPressedEvent;

	public event WorldInputDirectionDelegate DPadChangedEvent;

	private void Awake()
	{
		_dPad = base.transform.FindChild("DPad").GetComponent<WorldInputDPadUI>();
		_faceBtns = base.transform.FindChild("FaceBtns").GetComponent<WorldInputFaceBtnsUI>();
	}

	private void Start()
	{
		_dPad.SetPositionOut();
		_faceBtns.SetPositionOut();
		AddEvents(true);
	}

	private void AddEvents(bool value)
	{
		if (value)
		{
			_dPad.DirectionChangedEvent += DpadDirectionChangedHandler;
			_faceBtns.PressedEvent += FaceBtnAPressedHandler;
		}
		else
		{
			_dPad.DirectionChangedEvent -= DpadDirectionChangedHandler;
			_faceBtns.PressedEvent -= FaceBtnAPressedHandler;
		}
	}

	private void OnDestroy()
	{
		AddEvents(false);
	}

	public void TweenIn()
	{
		StopAllCoroutines();
		StartCoroutine(_dPad.TweenIn());
		StartCoroutine(_faceBtns.TweenIn());
	}

	public void TweenOut()
	{
		StopAllCoroutines();
		StartCoroutine(_dPad.TweenOut());
		StartCoroutine(_faceBtns.TweenOut());
		_dPad.SetDirection(eDirection.NONE);
	}

	public void SetFaceButtonsForPlayerInteract(bool isInteractable)
	{
		_faceBtns.SetABtnSparkleActive(isInteractable);
	}

	private void DpadDirectionChangedHandler(eDirection direction)
	{
		if (this.DPadChangedEvent != null)
		{
			this.DPadChangedEvent(direction);
		}
	}

	private void FaceBtnAPressedHandler()
	{
		if (this.FaceBtnAPressedEvent != null)
		{
			this.FaceBtnAPressedEvent();
		}
	}
}
