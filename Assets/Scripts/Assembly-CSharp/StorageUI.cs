using UnityEngine;
using UnityEngine.UI;

public class StorageUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private Transform _combineCountBar;

	private Text _combineCountLabel;

	private Transform _bootcampCountBar;

	private Text _bootcampCountLabel;

	public event BagUIBtnDelegate DecksEvent;

	public event BagUIBtnDelegate BootcampEvent;

	public event BagUIBtnDelegate CombineEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_combineCountBar = _mainMenu.transform.FindChild("CombineBtn/CountBar");
		_combineCountLabel = _combineCountBar.FindChild("Label").GetComponent<Text>();
		_bootcampCountBar = _mainMenu.transform.FindChild("BootcampBtn/CountBar");
		_bootcampCountLabel = _bootcampCountBar.FindChild("Label").GetComponent<Text>();
	}

	public void SetUp()
	{
		SetCombineCountBar(PlayerDefs.GetMortyCombineCount());
		SetBootcampCountBar(BootcampDefs.GetBootcampResultsCount());
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_STORAGE_BTN_DECKS);
		_mainMenu.SetBtnTitle(1, TextDefs.MORTY_STORAGE_BTN_BOOTCAMP);
		_mainMenu.SetBtnTitle(2, TextDefs.MORTY_STORAGE_BTN_COMBINE);
		_dialogue.SetText(TextDefs.MORTY_STORAGE_DIAL);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public void SetCombineCountBar(int count)
	{
		if (count <= 0)
		{
			_combineCountBar.gameObject.SetActive(false);
			return;
		}
		_combineCountBar.gameObject.SetActive(true);
		int num = Mathf.Clamp(count, 1, 99);
		_combineCountLabel.text = ((count <= num) ? num.ToString() : (num + "+"));
	}

	public void SetBootcampCountBar(int count)
	{
		if (count <= 0)
		{
			_bootcampCountBar.gameObject.SetActive(false);
			return;
		}
		_bootcampCountBar.gameObject.SetActive(true);
		_bootcampCountLabel.text = count.ToString();
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.DecksEvent != null)
			{
				this.DecksEvent();
			}
			break;
		case 1:
			if (this.BootcampEvent != null)
			{
				this.BootcampEvent();
			}
			break;
		case 2:
			if (this.CombineEvent != null)
			{
				this.CombineEvent();
			}
			break;
		case 3:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
