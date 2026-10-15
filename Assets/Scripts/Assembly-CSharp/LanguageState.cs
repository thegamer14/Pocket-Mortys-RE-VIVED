using System;
using UnityEngine;

public class LanguageState : StateBase
{
	[HideInInspector]
	public Type nextStateType;

	private LanguageUI _languageUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Language/{0}", "Language");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		nextStateType = null;
		base.Deactivate(transOutInfo);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_languageUI = _ui as LanguageUI;
		_languageUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_languageUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_languageUI.LanguageSelectedEvent += UILanguageSelectedHandler;
		_languageUI.BackEvent += UIBackPressedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_languageUI.LanguageSelectedEvent -= UILanguageSelectedHandler;
		_languageUI.BackEvent -= UIBackPressedHandler;
	}

	private void UILanguageSelectedHandler(eLanguage language)
	{
		LocalizationDefs.SetLanguage(language);
	}

	public void UIBackPressedHandler()
	{
		App.saveData.settings.languageSetByUser = true;
		App.saveData.Save(eSaveDataType.SETTINGS);
		if (nextStateType != null)
		{
			App.states.Set(nextStateType, new FadeTransInfo(), new FadeTransInfo());
		}
		else
		{
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}
}
