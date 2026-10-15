using System.Collections.Generic;
using UnityEngine;

public class SaveConflictState : StateBase
{
	private SaveConflictUI _saveConflictUI;

	private CloudContainer _selectedCloudContainer;

	private bool _firstTransitionInDone;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/SaveConflict/{0}", "SaveConflict");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_saveConflictUI = _ui as SaveConflictUI;
		_saveConflictUI.SetUp();
		_saveConflictUI.DisplayLoading();
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_saveConflictUI.SaveSelectedEvent += SaveSelectedHandler;
		App.saveData.CloudContainerConflictEvent += CloudContainerConflictHandler;
		App.saveData.InitCloudContainerCompleteEvent += InitCloudContainerCompleteHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_saveConflictUI.SaveSelectedEvent -= SaveSelectedHandler;
		App.saveData.CloudContainerConflictEvent -= CloudContainerConflictHandler;
		App.saveData.InitCloudContainerCompleteEvent -= InitCloudContainerCompleteHandler;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (!_firstTransitionInDone)
		{
			_firstTransitionInDone = true;
			GotoNextState();
		}
	}

	private void AuthenticateHandler(bool success)
	{
		App.social.AuthenticateEvent -= AuthenticateHandler;
		App.saveData.InitCloudContainer();
	}

	private void GotoNextState()
	{
		InterstitialState interstitialState = App.states.Get<InterstitialState>();
		interstitialState.placementID = eUpsightPlacementID.ON_LAUNCH;
		if (TestDefs.TEST_APP_UPDATE_STATE || App.saveData.player.showUpdateState)
		{
			interstitialState.nextStateType = typeof(AppUpdateState);
		}
		else
		{
			interstitialState.nextStateType = typeof(MainMenuState);
		}
		if (!LocalizationDefs.TryAutoSelectLanguage())
		{
			LanguageState languageState = App.states.Get<LanguageState>();
			languageState.nextStateType = typeof(InterstitialState);
			App.states.Set<LanguageState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
		}
		else
		{
			App.states.Set<InterstitialState>(null, null, false, eStateAction.DEACTIVATE_ALL);
		}
	}

	private void SaveSelectedHandler(CloudContainer cloudContainer)
	{
		_selectedCloudContainer = cloudContainer;
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(TextDefs.SAVE_SELECT_CONFIRM, new string[1] { TextDefs.SAVE_SELECT_CONFIRM_YES_BTN });
		genericPopupState.ButtonClickedEvent += ConfirmPopupButtonClickedHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void ConfirmPopupButtonClickedHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= ConfirmPopupButtonClickedHandler;
		switch (buttonIndex)
		{
		case 0:
			App.states.SetBack(new PopTransInfo());
			_saveConflictUI.DeselectSelectedCell();
			break;
		case 1:
			App.states.SetBack();
			_saveConflictUI.DisplayLoading();
			App.saveData.TryResolveConflicWithContainer(_selectedCloudContainer);
			break;
		}
	}

	private void CloudContainerConflictHandler(List<CloudContainer> containers)
	{
		_saveConflictUI.DisplayConflicts(containers);
	}

	private void InitCloudContainerCompleteHandler()
	{
		GotoNextState();
	}
}
