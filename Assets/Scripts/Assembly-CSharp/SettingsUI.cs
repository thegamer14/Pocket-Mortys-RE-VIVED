using UnityEngine.UI;

public class SettingsUI : UIBase
{
	public delegate void PauseUIDelegate();

	public delegate void SliderChangedDelegate(float val);

	private Text _title;

	private Text _sfxLabel;

	private Text _musicLabel;

	private Text _version;

	private Text _saveToCloudBtnTitle;

	private Text _deleteCampaignBtnTitle;

	private Text _deleteMultiplayerBtnTitle;

	private Text _supportBtnTitle;

	private Text _languageBtnTitle;

	private Text _creditsBtnTitle;

	private Slider _sfxSlider;

	private Slider _musicSlider;

	public event PauseUIDelegate DeleteCampaignPressedEvent;

	public event PauseUIDelegate DeleteMultiplayerPressedEvent;

	public event PauseUIDelegate LanguagePressedEvent;

	public event PauseUIDelegate CreditsPressedEvent;

	public event PauseUIDelegate BackPressedEvent;

	public event PauseUIDelegate SupportPressedEvent;

	public event SliderChangedDelegate SfxVolumeChangedEvent;

	public event SliderChangedDelegate MusicVolumeChangedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		_title = base.transform.FindChild("Box/TitleContainer/Title").GetComponent<Text>();
		_sfxLabel = base.transform.FindChild("Box/SfxSlider/Label").GetComponent<Text>();
		_musicLabel = base.transform.FindChild("Box/MusicSlider/Label").GetComponent<Text>();
		_version = base.transform.FindChild("Version").GetComponent<Text>();
		_deleteCampaignBtnTitle = base.transform.FindChild("Box/DeleteCampaign/Label").GetComponent<Text>();
		_deleteMultiplayerBtnTitle = base.transform.FindChild("Box/DeleteMultiplayer/Label").GetComponent<Text>();
		_supportBtnTitle = base.transform.FindChild("Box/Support/Label").GetComponent<Text>();
		_languageBtnTitle = base.transform.FindChild("Box/Language/Label").GetComponent<Text>();
		_creditsBtnTitle = base.transform.FindChild("Credits/Image/Label").GetComponent<Text>();
		_sfxSlider = base.transform.FindChild("Box/SfxSlider").GetComponent<Slider>();
		_musicSlider = base.transform.FindChild("Box/MusicSlider").GetComponent<Slider>();
		_sfxSlider.value = App.saveData.settings.soundFXVolume;
		_musicSlider.value = App.saveData.settings.musicVolume;
	}

	public override void SetLabels()
	{
		_title.text = TextDefs.SETTINGS_TITLE;
		_sfxLabel.text = TextDefs.SETTINGS_SFX_SLIDER;
		_musicLabel.text = TextDefs.SETTINGS_MUSIC_SLIDER;
		_version.text = "v2.0.3";
		_deleteCampaignBtnTitle.text = TextDefs.SETTINGS_BTN_DELETE;
		_deleteMultiplayerBtnTitle.text = MPTextDefs.MP_SETTINGS_BTN_DELETE;
		_supportBtnTitle.text = TextDefs.SETTINGS_BTN_EMAIL_SUPPORT;
		_languageBtnTitle.text = TextDefs.SETTINGS_BTN_LANGUAGE;
		_creditsBtnTitle.text = TextDefs.SETTINGS_BTN_CREDITS;
	}

	public void DeleteCampaignPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.DeleteCampaignPressedEvent != null)
		{
			this.DeleteCampaignPressedEvent();
		}
	}

	public void DeleteMultiplayerPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.DeleteMultiplayerPressedEvent != null)
		{
			this.DeleteMultiplayerPressedEvent();
		}
	}

	public void SupportPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.SupportPressedEvent != null)
		{
			this.SupportPressedEvent();
		}
	}

	public void LanguagePressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.LanguagePressedEvent != null)
		{
			this.LanguagePressedEvent();
		}
	}

	public void CreditsPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.CreditsPressedEvent != null)
		{
			this.CreditsPressedEvent();
		}
	}

	public void BackPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.BackPressedEvent != null)
		{
			this.BackPressedEvent();
		}
	}

	public void SfxSliderChangedHandler()
	{
		if (this.SfxVolumeChangedEvent != null)
		{
			this.SfxVolumeChangedEvent(_sfxSlider.value);
		}
	}

	public void MusicSliderChangedHandler()
	{
		if (this.MusicVolumeChangedEvent != null)
		{
			this.MusicVolumeChangedEvent(_musicSlider.value);
		}
	}
}
