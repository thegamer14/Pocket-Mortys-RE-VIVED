public class MortyLabUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	public event BagUIBtnDelegate ModifyEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public override void SetLabels()
	{
		_dialogue.SetText(TextDefs.MORTY_LAB_DIAL);
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_LAB_BTN_SELECT);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.ModifyEvent != null)
			{
				this.ModifyEvent();
			}
			break;
		case 1:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
