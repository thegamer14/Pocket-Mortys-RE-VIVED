using UnityEngine;

public class MPTutorialPopupState : StateBase
{
	private MPTutorialPopupUI _popupUI;

	private string _text;

	public override void Awake()
	{
		base.Awake();
		base.hideStateBellow = false;
		_useBackgroundFade = true;
		_uiResourcePath = string.Format("UI/MPTutorialPopup/{0}", "MPTutorialPopup");
	}

	public void SetDisplayValues(string text)
	{
		_text = text;
		if (_popupUI != null)
		{
			_popupUI.SetUp(_text);
		}
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		_popupUI.SetUp(_text);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_popupUI = _ui as MPTutorialPopupUI;
	}

	protected override void Destroy()
	{
		base.Destroy();
		_popupUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_popupUI.CloseButtonClickedEvent += CloseButtonClickedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_popupUI.CloseButtonClickedEvent -= CloseButtonClickedHandler;
	}

	protected virtual void CloseButtonClickedHandler()
	{
		App.states.SetBack(new PopTransInfo());
	}
}
