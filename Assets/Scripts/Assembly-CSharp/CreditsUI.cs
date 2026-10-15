using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : UIBase
{
	public delegate void CreditsUIDelegate();

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	public event CreditsUIDelegate BackEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public override void SetLabels()
	{
		_dialogue.SetText(TextDefs.CREDITS_DIALOGUE);
		Transform transform = base.transform.FindChild("ItemMenu/Container/");
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			int num = 1;
			Transform transform2 = child.transform.FindChild("Person1");
			while (transform2 != null)
			{
				transform2.FindChild("Title").GetComponent<Text>().text = TextDefs.GetCreditTitle(child.name, num);
				num++;
				transform2 = child.transform.FindChild("Person" + num);
			}
		}
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
		if (index == 0 && this.BackEvent != null)
		{
			this.BackEvent();
		}
	}
}
