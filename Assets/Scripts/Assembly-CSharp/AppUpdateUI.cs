using UnityEngine.UI;

public class AppUpdateUI : UIBase
{
	public delegate void AppUpdateUIBtnDelegate();

	public AppUpdateUIBtnDelegate ContinueEvent;

	public AppUpdateUIBtnDelegate BackEvent;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private Text _title_1_3;

	private Text _description_1_3;

	private Text _title_1_4;

	private Text _description_1_4;

	private Text _title_1_6;

	private Text _description_1_6;

	private Text _title_1_7;

	private Text _description_1_7;

	private Text _title_1_8;

	private Text _description_1_8;

	private Text _title_1_9;

	private Text _description_1_9;

	private Text _title_1_10;

	private Text _description_1_10;

	private Text _title_2_0;

	private Text _description_2_0;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_title_1_3 = base.transform.FindChild("ScrollMenu/Container/V1.3/Title/Label").GetComponent<Text>();
		_description_1_3 = base.transform.FindChild("ScrollMenu/Container/V1.3/Description").GetComponent<Text>();
		_title_1_4 = base.transform.FindChild("ScrollMenu/Container/V1.4/Title/Label").GetComponent<Text>();
		_description_1_4 = base.transform.FindChild("ScrollMenu/Container/V1.4/Description").GetComponent<Text>();
		_title_1_6 = base.transform.FindChild("ScrollMenu/Container/V1.6/Title/Label").GetComponent<Text>();
		_description_1_6 = base.transform.FindChild("ScrollMenu/Container/V1.6/Description").GetComponent<Text>();
		_title_1_7 = base.transform.FindChild("ScrollMenu/Container/V1.7/Title/Label").GetComponent<Text>();
		_description_1_7 = base.transform.FindChild("ScrollMenu/Container/V1.7/Description").GetComponent<Text>();
		_title_1_8 = base.transform.FindChild("ScrollMenu/Container/V1.8/Title/Label").GetComponent<Text>();
		_description_1_8 = base.transform.FindChild("ScrollMenu/Container/V1.8/Description").GetComponent<Text>();
		_title_1_9 = base.transform.FindChild("ScrollMenu/Container/V1.9/Title/Label").GetComponent<Text>();
		_description_1_9 = base.transform.FindChild("ScrollMenu/Container/V1.9/Description").GetComponent<Text>();
		_title_1_10 = base.transform.FindChild("ScrollMenu/Container/V1.10/Title/Label").GetComponent<Text>();
		_description_1_10 = base.transform.FindChild("ScrollMenu/Container/V1.10/Description").GetComponent<Text>();
		_title_2_0 = base.transform.FindChild("ScrollMenu/Container/V2.0/Title/Label").GetComponent<Text>();
		_description_2_0 = base.transform.FindChild("ScrollMenu/Container/V2.0/Description").GetComponent<Text>();
	}

	public override void SetLabels()
	{
		_dialogue.SetText(TextDefs.APP_UPDATE_DIALOGUE);
		_mainMenu.SetBtnTitle(0, TextDefs.APP_UPDATE_BTN_CONTINUE);
		_title_1_3.text = TextDefs.APP_UPDATE_TITLE_1_3;
		_description_1_3.text = TextDefs.APP_UPDATE_DESCRIPTION_1_3;
		_title_1_4.text = TextDefs.APP_UPDATE_TITLE_1_4;
		_description_1_4.text = TextDefs.APP_UPDATE_DESCRIPTION_1_4;
		_title_1_6.text = TextDefs.APP_UPDATE_TITLE_1_6;
		_description_1_6.text = TextDefs.APP_UPDATE_DESCRIPTION_1_6;
		_title_1_7.text = TextDefs.APP_UPDATE_TITLE_1_7;
		_description_1_7.text = TextDefs.APP_UPDATE_DESCRIPTION_1_7;
		_title_1_8.text = TextDefs.APP_UPDATE_TITLE_1_8;
		_description_1_8.text = TextDefs.APP_UPDATE_DESCRIPTION_1_8;
		_title_1_9.text = TextDefs.APP_UPDATE_TITLE_1_9;
		_description_1_9.text = TextDefs.APP_UPDATE_DESCRIPTION_1_9;
		_title_1_10.text = TextDefs.APP_UPDATE_TITLE_1_10;
		_description_1_10.text = TextDefs.APP_UPDATE_DESCRIPTION_1_10;
		_title_2_0.text = TextDefs.APP_UPDATE_TITLE_2_0;
		_description_2_0.text = TextDefs.APP_UPDATE_DESCRIPTION_2_0;
	}

	protected override void AddEvents()
	{
		App.states.BackButtonEvent += BackButtonHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		App.states.BackButtonEvent -= BackButtonHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		if (ContinueEvent != null)
		{
			ContinueEvent();
		}
	}

	private void BackButtonHandler()
	{
		if (_canvasGroup.interactable && BackEvent != null)
		{
			BackEvent();
		}
	}
}
