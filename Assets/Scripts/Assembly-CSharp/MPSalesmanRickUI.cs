public class MPSalesmanRickUI : UIBase
{
	public delegate void SalesmanRickUIBtnDelegate();

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	public event SalesmanRickUIBtnDelegate ItemsEvent;

	public event SalesmanRickUIBtnDelegate CustomizationEvent;

	public event SalesmanRickUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.SALESMAN_RICK_BTN_ITEMS);
		_mainMenu.SetBtnTitle(1, TextDefs.SALESMAN_RICK_BTN_CUSTOMIZATION);
		_dialogue.SetText(TextDefs.SALESMAN_RICK_DIAL);
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
			if (this.ItemsEvent != null)
			{
				this.ItemsEvent();
			}
			break;
		case 1:
			if (this.CustomizationEvent != null)
			{
				this.CustomizationEvent();
			}
			break;
		case 2:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
