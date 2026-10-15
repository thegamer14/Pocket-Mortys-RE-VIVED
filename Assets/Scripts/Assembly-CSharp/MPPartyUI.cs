using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MPPartyUI : UIBase
{
	public delegate void MPPartyUIMortyDelegate(MPOpponentMortyData mortyData);

	public delegate void MPPartyUISwitchDelegate(MPOpponentMortyData mortyData1, MPOpponentMortyData mortyData2);

	public delegate void MPPartyUIItemDelegate();

	public delegate void PartyUIDelegate();

	private const int _kMortyCellMax = 5;

	private const float _kMortyMenuPaddingTop = 0f;

	private const float _kMortyMenuPaddingBottom = 0f;

	private const float _kMortyMenuCellGap = -4f;

	private const float _kSortTweenSpeed = 500f;

	private MenuBase _mainMenu;

	private MenuBase _mainMenuItem;

	private MenuBase _mainMenuSwitch;

	private MenuBase _mainMenuBattleSwitch;

	private MenuBase _mainMenuBattleForceSwitch;

	private MenuDialogue _dialogue;

	private ScrollMenu _mortyScrollMenu;

	private MPMortyCell _currentBattleCell;

	private MPMortyCell _selectedCell;

	private string _selectedItemID;

	private MPOpponentMortyData _battleOutMortyData;

	private bool _isSwitchingOrderMode;

	public event MPPartyUIMortyDelegate MortySelectedEvent;

	public event MPPartyUISwitchDelegate SwitchEvent;

	public event MPPartyUIItemDelegate UseItemEvent;

	public event PartyUIDelegate UseItemFinishedEvent;

	public event PartyUIDelegate StatsEvent;

	public event PartyUIDelegate BackEvent;

	protected override void LateAwake()
	{
		_mortyScrollMenu = base.transform.FindChild("MortyMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_mainMenuItem = base.transform.FindChild("MenuBar/MainMenuItem").GetComponent<MenuBase>();
		_mainMenuSwitch = base.transform.FindChild("MenuBar/MainMenuSwitch").GetComponent<MenuBase>();
		_mainMenuBattleSwitch = base.transform.FindChild("MenuBar/MainMenuBattleSwitch").GetComponent<MenuBase>();
		_mainMenuBattleForceSwitch = base.transform.FindChild("MenuBar/MainMenuBattleForceSwitch").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(string.Format("UI/MPBattle/{0}", "Timer")));
		gameObject.transform.SetParent(base.transform);
	}

	public void SetUp(MPOpponentMortyData selectedMortyData, MPOpponentMortyData battleOutMortyData, string selectedItemID, bool forceSwich)
	{
		_selectedItemID = selectedItemID;
		_battleOutMortyData = battleOutMortyData;
		SetSwitchBtnAvailable(false);
		SetItemBtnAvailable(false);
		SetStatsBtnAvailable(false);
		_mortyScrollMenu.SetUp(1, false, 0f, 0f, -4f);
		SetUpMortyScrollMenu(battleOutMortyData);
		_currentBattleCell = GetCellForData(battleOutMortyData);
		_mortyScrollMenu.TrySetCellSelected(GetCellForData(selectedMortyData));
		if (MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.BAG))
		{
			_dialogue.SetText(string.Format(TextDefs.PARTY_DIAL_SET_UP_BAG, MPItemDefs.Name(_selectedItemID)));
			ShowMenu(_mainMenuItem);
		}
		else if (MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.BATTLE))
		{
			_dialogue.SetText(TextDefs.PARTY_DIAL_SET_UP);
			if (!forceSwich)
			{
				ShowMenu(_mainMenuBattleSwitch);
			}
			else
			{
				ShowMenu(_mainMenuBattleForceSwitch);
			}
		}
		else if (selectedMortyData != null && selectedItemID != null)
		{
			_mortyScrollMenu.SetInteractable(false);
			_dialogue.SetText(string.Empty);
			ShowMenu(null);
		}
		else
		{
			_dialogue.SetText(TextDefs.PARTY_DIAL_SET_UP);
			ShowMenu(_mainMenu);
		}
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.PARTY_BTN_SWITCH);
		_mainMenu.SetBtnTitle(1, TextDefs.PARTY_BTN_ITEM);
		_mainMenu.SetBtnTitle(2, TextDefs.PARTY_BTN_STATS);
		_mainMenuItem.SetBtnTitle(0, TextDefs.PARTY_BTN_ITEM);
		_mainMenuBattleSwitch.SetBtnTitle(0, TextDefs.PARTY_BTN_SWITCH);
		_mainMenuBattleSwitch.SetBtnTitle(1, TextDefs.PARTY_BTN_STATS);
		_mainMenuBattleForceSwitch.SetBtnTitle(0, TextDefs.PARTY_BTN_SWITCH);
		_mainMenuBattleForceSwitch.SetBtnTitle(1, TextDefs.PARTY_BTN_STATS);
	}

	protected override void AddEvents()
	{
		_mortyScrollMenu.SelectedEvent += MortyCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_mainMenuItem.BtnPressedEvent += MainMenuItemBtnPressedHandler;
		_mainMenuSwitch.BtnPressedEvent += MainMenuSwitchBtnPressedHandler;
		_mainMenuBattleSwitch.BtnPressedEvent += MainMenuBattleSwitchBtnPressedHandler;
		_mainMenuBattleForceSwitch.BtnPressedEvent += MainMenuBattleForceSwitchBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mortyScrollMenu.SelectedEvent -= MortyCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_mainMenuItem.BtnPressedEvent -= MainMenuItemBtnPressedHandler;
		_mainMenuSwitch.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_mainMenuBattleSwitch.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_mainMenuBattleForceSwitch.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public void SetUpMortyScrollMenu(MPOpponentMortyData currentBattleMorty)
	{
		int activeDeckCount = App.net.player.data.GetActiveDeckCount();
		for (int i = 0; i < 5; i++)
		{
			if (_mortyScrollMenu.count <= i)
			{
				if (i < activeDeckCount)
				{
					MPOpponentMortyData partyMortyData = App.net.player.data.GetPartyMortyData(i);
					AddMortyCell(partyMortyData, currentBattleMorty != null && currentBattleMorty == partyMortyData);
				}
				else
				{
					AddMortyCell(null, false);
				}
				continue;
			}
			MPMortyCell mPMortyCell = (MPMortyCell)_mortyScrollMenu.GetCellAtIndex(i);
			if (i < activeDeckCount)
			{
				MPOpponentMortyData partyMortyData2 = App.net.player.data.GetPartyMortyData(i);
				mPMortyCell.Refresh(partyMortyData2, currentBattleMorty != null && currentBattleMorty == partyMortyData2);
			}
			else
			{
				mPMortyCell.Refresh(null, false);
			}
		}
	}

	private void AddMortyCell(MPOpponentMortyData mortyData, bool isOut)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPMortyCell"));
		MPMortyCell component = rectTransform.GetComponent<MPMortyCell>();
		component.SetUp(mortyData, isOut, true, true);
		_mortyScrollMenu.AddCell(component);
	}

	private MPMortyCell GetCellForData(MPOpponentMortyData mortyData)
	{
		if (mortyData == null)
		{
			return null;
		}
		for (int i = 0; i < _mortyScrollMenu.count; i++)
		{
			MPMortyCell mPMortyCell = (MPMortyCell)_mortyScrollMenu.GetCellAtIndex(i);
			if (mPMortyCell.data == mortyData)
			{
				return mPMortyCell;
			}
		}
		return null;
	}

	private void StartSwitchingOrderMode()
	{
		_isSwitchingOrderMode = true;
		_mortyScrollMenu.SetUpSelectMax(2);
		StartCoroutine(ShowMenuWithTween(_mainMenuSwitch));
		_dialogue.TweenText(TextDefs.PARTY_DIAL_SORT);
	}

	public void EndSwitchingOrderMode()
	{
		_isSwitchingOrderMode = false;
		_mortyScrollMenu.SetUpSelectMax(1);
		MortyCellSelectedHandler(_mortyScrollMenu.GetSelectedCell(), 0);
		StartCoroutine(ShowMenuWithTween(_mainMenu));
		_dialogue.TweenText(TextDefs.PARTY_DIAL_SELECTED);
		ActivateInput(true);
	}

	private void RequestSwitchOrder(MPOpponentMortyData mortyData1, MPOpponentMortyData mortyData2)
	{
		StartCoroutine(ShowMenuWithTween(null));
		ActivateInput(false);
		_dialogue.TweenText(string.Empty);
		if (this.SwitchEvent != null)
		{
			this.SwitchEvent(mortyData1, mortyData2);
		}
	}

	public IEnumerator SwitchOrder(MPOpponentMortyData mortyData1, MPOpponentMortyData mortyData2)
	{
		CellBase fromCell = GetCellForData(mortyData1);
		CellBase toCell = GetCellForData(mortyData2);
		_mortyScrollMenu.SwapCellsIndex(fromCell, toCell);
		_mortyScrollMenu.SetUpSelectMax(1);
		yield return StartCoroutine(SortOrderTween(fromCell, toCell));
	}

	private IEnumerator SortOrderTween(CellBase fromCell, CellBase toCell)
	{
		RectTransform fromCellRectTrans = fromCell.GetComponent<RectTransform>();
		RectTransform toCellRectTrans = toCell.GetComponent<RectTransform>();
		float fromOldPosY = fromCellRectTrans.localPosition.y;
		float toOldPosY = toCellRectTrans.localPosition.y;
		toCellRectTrans.SetAsLastSibling();
		fromCellRectTrans.SetAsLastSibling();
		float duration = Mathf.Abs((fromOldPosY - toOldPosY) / 500f);
		Sequence mySequence = DOTween.Sequence();
		mySequence.Insert(0f, fromCellRectTrans.DOLocalMoveY(toOldPosY, duration));
		mySequence.Insert(0f, toCellRectTrans.DOLocalMoveY(fromOldPosY, duration));
		mySequence.Play();
		yield return mySequence.WaitForCompletion();
	}

	private void RequestUseItem()
	{
		StartCoroutine(ShowMenuWithTween(null));
		ActivateInput(false);
		if (this.UseItemEvent != null)
		{
			this.UseItemEvent();
		}
	}

	public void UseItem(MPOpponentMortyData mortyData, string itemID, bool isSuccess, int effectValue)
	{
		StartCoroutine(UseItemCo(mortyData, itemID, isSuccess, effectValue));
		_selectedItemID = null;
	}

	private IEnumerator UseItemCo(MPOpponentMortyData mortyData, string itemID, bool isSuccess, int effectValue)
	{
		MPMortyCell mortyCell = GetCellForData(mortyData);
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.PARTY_DIAL_USE, MPItemDefs.Name(itemID), MPMortyDefs.Name(mortyData.id)), 2f, true));
		App.sound.PlaySFX("Audio/SFX/UseItem");
		switch (MPItemDefs.EffectType(itemID))
		{
		case "StatIncrease":
			if (MPItemDefs.EffectStatType(itemID).Equals("Level"))
			{
				mortyCell.PulseLevelBar();
				App.sound.PlaySFX("Audio/SFX/LevelUP", 0.5f);
			}
			break;
		case "RestoreHP":
		case "Revive":
			yield return StartCoroutine(mortyCell.TweenToHP(mortyData.hp));
			break;
		}
		if (mortyCell != null)
		{
			mortyCell.Refresh(mortyData, false, true);
		}
		string resultText = MPItemDefs.GetItemUsedResultText(itemID, isSuccess, effectValue, mortyData.id, null, TextDefs.PRONOUN_PLAYERS, null);
		if (resultText != null)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(resultText, 2f, true));
		}
		SetUpMortyScrollMenu(_battleOutMortyData);
		if (this.UseItemFinishedEvent != null)
		{
			this.UseItemFinishedEvent();
		}
	}

	public IEnumerator UseItemFail(bool activateInput)
	{
		yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_PARTY_USE_ITEM_FAIL, 2f, true));
		if (activateInput)
		{
			UseItemFinished();
		}
		if (this.UseItemFinishedEvent != null)
		{
			this.UseItemFinishedEvent();
		}
	}

	public void UseItemFinished()
	{
		StartCoroutine(ShowMenuWithTween(_mainMenu));
		_dialogue.TweenText(TextDefs.PARTY_DIAL_SELECTED);
		ActivateInput(true);
		_mortyScrollMenu.SetInteractable(true);
		_selectedItemID = null;
	}

	private void ShowMenu(MenuBase menu)
	{
		_mainMenu.SetPositionOut();
		_mainMenuItem.SetPositionOut();
		_mainMenuSwitch.SetPositionOut();
		_mainMenuBattleSwitch.SetPositionOut();
		_mainMenuBattleForceSwitch.SetPositionOut();
		if (menu != null)
		{
			menu.SetPositionIn();
		}
	}

	private IEnumerator ShowMenuWithTween(MenuBase menu)
	{
		_mortyScrollMenu.SetInteractable(false);
		StartCoroutine(_mainMenu.TweenOut());
		StartCoroutine(_mainMenuItem.TweenOut());
		StartCoroutine(_mainMenuSwitch.TweenOut());
		StartCoroutine(_mainMenuBattleSwitch.TweenOut());
		StartCoroutine(_mainMenuBattleForceSwitch.TweenOut());
		if (menu != null)
		{
			yield return StartCoroutine(menu.TweenIn());
			_mortyScrollMenu.SetInteractable(true);
		}
	}

	private void SetSwitchBtnAvailable(bool value)
	{
		_mainMenu.SetBtnEnabled(0, value);
		_mainMenuBattleSwitch.SetBtnEnabled(0, value);
		_mainMenuBattleForceSwitch.SetBtnEnabled(0, value);
	}

	private void SetItemBtnAvailable(bool value)
	{
		_mainMenu.SetBtnEnabled(1, value);
		_mainMenuItem.SetBtnEnabled(0, value);
	}

	private void SetStatsBtnAvailable(bool value)
	{
		_mainMenu.SetBtnEnabled(2, value);
		_mainMenuBattleSwitch.SetBtnEnabled(1, value);
		_mainMenuBattleForceSwitch.SetBtnEnabled(1, value);
	}

	public void MortyCellSelectedHandler(CellBase cell, int index)
	{
		MPMortyCell mPMortyCell = (MPMortyCell)cell;
		if (_isSwitchingOrderMode)
		{
			RequestSwitchOrder(_selectedCell.data, mPMortyCell.data);
			return;
		}
		_selectedCell = mPMortyCell;
		if (_currentBattleCell == _selectedCell || _selectedCell.data.isFainted)
		{
			SetSwitchBtnAvailable(false);
		}
		else
		{
			SetSwitchBtnAvailable(true);
		}
		SetStatsBtnAvailable(true);
		if (_selectedItemID != null)
		{
			string resultText;
			if (MPItemDefs.CanItemBeApplyedToMorty(_selectedItemID, _selectedCell.data, out resultText))
			{
				SetItemBtnAvailable(true);
				_dialogue.TweenText(string.Format(TextDefs.PARTY_DIAL_USE_BAG, MPItemDefs.Name(_selectedItemID), MPMortyDefs.Name(_selectedCell.data.id)));
			}
			else
			{
				SetItemBtnAvailable(false);
				_dialogue.TweenText(resultText);
			}
		}
		else
		{
			SetItemBtnAvailable(true);
			_dialogue.TweenText(TextDefs.PARTY_DIAL_SELECTED);
		}
		if (this.MortySelectedEvent != null)
		{
			this.MortySelectedEvent(_selectedCell.data);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			StartSwitchingOrderMode();
			break;
		case 1:
			RequestUseItem();
			break;
		case 2:
			if (this.StatsEvent != null)
			{
				this.StatsEvent();
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

	public void MainMenuItemBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			RequestUseItem();
			break;
		case 1:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}

	public void MainMenuSwitchBtnPressedHandler(int index)
	{
		if (index == 0)
		{
			EndSwitchingOrderMode();
		}
	}

	public void MainMenuBattleSwitchBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			RequestSwitchOrder(_currentBattleCell.data, _selectedCell.data);
			break;
		case 1:
			if (this.StatsEvent != null)
			{
				this.StatsEvent();
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

	public void MainMenuBattleForceSwitchBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			RequestSwitchOrder(_currentBattleCell.data, _selectedCell.data);
			break;
		case 1:
			if (this.StatsEvent != null)
			{
				this.StatsEvent();
			}
			break;
		}
	}
}
