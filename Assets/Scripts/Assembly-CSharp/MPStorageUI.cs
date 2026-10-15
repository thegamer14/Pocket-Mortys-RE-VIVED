using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MPStorageUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private Transform _combineCountBar;

	private Text _combineCountLabel;

	public event BagUIBtnDelegate DecksEvent;

	public event BagUIBtnDelegate CombineEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_combineCountBar = _mainMenu.transform.FindChild("CombineBtn/CountBar");
		_combineCountLabel = _combineCountBar.FindChild("Label").GetComponent<Text>();
	}

	public void SetUp()
	{
		SetCombineCountBar();
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_STORAGE_BTN_DECKS);
		_mainMenu.SetBtnTitle(1, MPTextDefs.MP_MORTY_STORAGE_BTN_COMBINE);
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

	public void SetCombineCountBar()
	{
		List<MPOpponentMortyData> mortys = new List<MPOpponentMortyData>(App.net.player.data.morties);
		mortys.RemoveAll((MPOpponentMortyData data) => MPMortyDefs.EvolutionRequirement(data.id) <= 0);
		mortys.RemoveAll((MPOpponentMortyData data) => mortys.Count((MPOpponentMortyData countData) => countData.id.Equals(data.id)) < 2);
		int count = mortys.Count;
		if (count <= 0)
		{
			_combineCountBar.gameObject.SetActive(false);
			return;
		}
		_combineCountBar.gameObject.SetActive(true);
		int num = Mathf.Clamp(count, 1, 99);
		_combineCountLabel.text = ((count <= num) ? num.ToString() : (num + "+"));
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
			if (this.CombineEvent != null)
			{
				this.CombineEvent();
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
