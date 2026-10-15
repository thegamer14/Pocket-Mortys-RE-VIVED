using UnityEngine;

public class SettingsState : StateBase
{
	private SettingsUI _settingsUI;

	private eSaveDataType _deletingDataType;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Settings/{0}", "Settings");
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
		_settingsUI = _ui as SettingsUI;
	}

	protected override void Destroy()
	{
		base.Destroy();
		_settingsUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_settingsUI.SfxVolumeChangedEvent += SfxVolumeChangedHandler;
		_settingsUI.MusicVolumeChangedEvent += MusicVolumeChangedHandler;
		_settingsUI.DeleteCampaignPressedEvent += UIDeleteCampaignPressedHandler;
		_settingsUI.DeleteMultiplayerPressedEvent += UIDeleteMultiplayerPressedHandler;
		_settingsUI.SupportPressedEvent += UISupportPressedHandler;
		_settingsUI.CreditsPressedEvent += UICreditsPressedHandler;
		_settingsUI.LanguagePressedEvent += LanguagePressedHandler;
		_settingsUI.BackPressedEvent += UIBackPressedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_settingsUI.SfxVolumeChangedEvent -= SfxVolumeChangedHandler;
		_settingsUI.MusicVolumeChangedEvent -= MusicVolumeChangedHandler;
		_settingsUI.DeleteCampaignPressedEvent -= UIDeleteCampaignPressedHandler;
		_settingsUI.DeleteMultiplayerPressedEvent -= UIDeleteMultiplayerPressedHandler;
		_settingsUI.SupportPressedEvent -= UISupportPressedHandler;
		_settingsUI.CreditsPressedEvent -= UICreditsPressedHandler;
		_settingsUI.LanguagePressedEvent -= LanguagePressedHandler;
		_settingsUI.BackPressedEvent -= UIBackPressedHandler;
	}

	private void SfxVolumeChangedHandler(float value)
	{
		App.saveData.settings.soundFXVolume = value;
		App.sound.sfxVolume = App.saveData.settings.soundFXVolume;
	}

	private void MusicVolumeChangedHandler(float value)
	{
		App.saveData.settings.musicVolume = value;
		App.sound.musicVolume = App.saveData.settings.musicVolume;
	}

	public void UIDeleteCampaignPressedHandler()
	{
		_deletingDataType = eSaveDataType.PLAYER | eSaveDataType.SEGMENTED_WORLD | eSaveDataType.TOURNAMENT;
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(TextDefs.DELETE_SAVE_DATA_CONFIRM, new string[1] { TextDefs.DELETE_SAVE_DATA_CONFIRM_BTN });
		genericPopupState.ButtonClickedEvent += DeleteConfirmButtonClickedHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void UIDeleteMultiplayerPressedHandler()
	{
		_deletingDataType = eSaveDataType.MULTIPLAYER;
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(MPTextDefs.MP_DELETE_SAVE_DATA_CONFIRM, new string[1] { TextDefs.DELETE_SAVE_DATA_CONFIRM_BTN });
		genericPopupState.ButtonClickedEvent += DeleteConfirmButtonClickedHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void UISupportPressedHandler()
	{
		App.saveData.EmailCloudContainerToSupport();
	}

	public void UICreditsPressedHandler()
	{
		App.states.Set<CreditsState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void LanguagePressedHandler()
	{
		App.states.Set<LanguageState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void UIBackPressedHandler()
	{
		App.saveData.Save(eSaveDataType.SETTINGS);
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void DeleteConfirmButtonClickedHandler(int buttonIndex)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.ButtonClickedEvent -= DeleteConfirmButtonClickedHandler;
		if (buttonIndex == 1)
		{
			App.saveData.Delete(_deletingDataType);
			App.saveData.Save(eSaveDataType.ALL);
			genericPopupState.SetDisplayValues(TextDefs.SAVE_DATA_DELETED, null);
			genericPopupState.ButtonClickedEvent += SaveDeletedPopupButtonClickedHandler;
		}
		else
		{
			_deletingDataType = eSaveDataType.NONE;
			App.states.SetBack(new PopTransInfo());
		}
	}

	private void SaveDeletedPopupButtonClickedHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= SaveDeletedPopupButtonClickedHandler;
		if (App.currentMode == eGameMode.CAMPAIGN && (_deletingDataType & eSaveDataType.PLAYER) == eSaveDataType.PLAYER)
		{
			App.states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
		}
		else if (App.currentMode == eGameMode.MULTIPLAYER && (_deletingDataType & eSaveDataType.MULTIPLAYER) == eSaveDataType.MULTIPLAYER)
		{
			App.redirectOnMPSessionDisconnect = false;
			App.net.session.Disconnect(eNetSessionDisconnectType.SAVE_DATA_DELETED);
			App.states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
		}
		else
		{
			App.states.SetBack(new PopTransInfo());
		}
		_deletingDataType = eSaveDataType.NONE;
	}
}
