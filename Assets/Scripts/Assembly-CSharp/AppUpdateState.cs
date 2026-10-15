using System;
using UnityEngine;

public class AppUpdateState : StateBase
{
	private AppUpdateUI _appUpdateUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/AppUpdate/{0}", "AppUpdate");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/World/main_theme_final", 0.6f);
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0f);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_appUpdateUI = _ui as AppUpdateUI;
	}

	protected override void Destroy()
	{
		base.Destroy();
		_appUpdateUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		AppUpdateUI appUpdateUI = _appUpdateUI;
		appUpdateUI.ContinueEvent = (AppUpdateUI.AppUpdateUIBtnDelegate)Delegate.Combine(appUpdateUI.ContinueEvent, new AppUpdateUI.AppUpdateUIBtnDelegate(UIContinueHandler));
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		AppUpdateUI appUpdateUI = _appUpdateUI;
		appUpdateUI.ContinueEvent = (AppUpdateUI.AppUpdateUIBtnDelegate)Delegate.Remove(appUpdateUI.ContinueEvent, new AppUpdateUI.AppUpdateUIBtnDelegate(UIContinueHandler));
	}

	private void UIContinueHandler()
	{
		App.saveData.player.showUpdateState = false;
		App.saveData.Save(eSaveDataType.PLAYER);
		App.states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo());
	}
}
