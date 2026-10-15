using UnityEngine;

public class GenericPopupState : StateBase
{
	public delegate void ButtonClickedDelegate(int buttonIndex);

	private GenericPopupUI _genericPopupUI;

	private string _text;

	private string[] _buttons;

	private bool _showCloseBtn;

	private bool _showWaiting;

	private bool _showCouponHolder;

	public event ButtonClickedDelegate ButtonClickedEvent;

	public override void Awake()
	{
		base.Awake();
		base.hideStateBellow = false;
		_useBackgroundFade = true;
		_uiResourcePath = string.Format("UI/GenericPopup/{0}", "GenericPopup");
	}

	public void SetDisplayValues(string text, string[] buttons, bool showCloseBtn = true, bool showWaiting = false, bool showCouponHolder = false)
	{
		_text = text;
		_buttons = buttons;
		_showCloseBtn = showCloseBtn;
		_showWaiting = showWaiting;
		_showCouponHolder = showCouponHolder;
		if (_genericPopupUI != null)
		{
			_genericPopupUI.SetDisplayValues(_text, _buttons, _showCloseBtn, _showWaiting, _showCouponHolder);
		}
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		_genericPopupUI.SetDisplayValues(_text, _buttons, _showCloseBtn, _showWaiting, _showCouponHolder);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_genericPopupUI = _ui as GenericPopupUI;
	}

	protected override void Destroy()
	{
		base.Destroy();
		_genericPopupUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_genericPopupUI.ButtonClickedEvent += ButtonClickedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_genericPopupUI.ButtonClickedEvent -= ButtonClickedHandler;
	}

	public void ResetButtonClickedEvent(ButtonClickedDelegate clickedEvent = null)
	{
		this.ButtonClickedEvent = clickedEvent;
	}

	protected virtual void ButtonClickedHandler(int buttonIndex)
	{
		if (this.ButtonClickedEvent != null)
		{
			this.ButtonClickedEvent(buttonIndex);
		}
	}
}
