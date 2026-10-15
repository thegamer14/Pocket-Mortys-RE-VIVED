using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PartyUI : UIBase
{
	public delegate void PartyUIMortySortDelegate(int index1, int index2);

	public delegate void PartyUIMortyDelegate(MortyData mortyData);

	public delegate void PartyUIUseItemDelegate(bool isSuccess, bool bagStateOnComplete);

	public delegate void PartyUIBtnDelegate();

	private const int _kMortyCellMax = 5;

	private const float _kMortyMenuPaddingTop = 0f;

	private const float _kMortyMenuPaddingBottom = 0f;

	private const float _kMortyMenuCellGap = -4f;

	private const float _kSortTweenSpeed = 500f;

	private AttackLearnNewBox _newAttackBox;

	private MenuBase _mainMenu;

	private MenuBase _mainMenuItem;

	private MenuBase _mainMenuSwitch;

	private MenuBase _mainMenuBattleSwitch;

	private MenuBase _mainMenuBattleForceSwitch;

	private BattleAttackMenu _attackMenu;

	private MenuDialogue _dialogue;

	private Transform _attackLearnContainer;

	private ScrollMenu _mortyMenu;

	private MortyCell _currentBattleCell;

	private MortyCell _selectedCell;

	private Dictionary<string, int> _items;

	private string _selectedItemID;

	private bool _isBattle;

	private bool _isSortMode;

	private int _levelUpSwapAttackIndex;

	public event PartyUIMortySortDelegate SwitchEvent;

	public event PartyUIMortyDelegate MortySelectedEvent;

	public event PartyUIUseItemDelegate UseItemFinishedEvent;

	public event PartyUIBtnDelegate SwitchInBattleEvent;

	public event PartyUIBtnDelegate UseItemInBattleEvent;

	public event PartyUIBtnDelegate UseItemNotYetSetEvent;

	public event PartyUIBtnDelegate StatsEvent;

	public event PartyUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mortyMenu = base.transform.FindChild("MortyMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_mainMenuItem = base.transform.FindChild("MenuBar/MainMenuItem").GetComponent<MenuBase>();
		_mainMenuSwitch = base.transform.FindChild("MenuBar/MainMenuSwitch").GetComponent<MenuBase>();
		_mainMenuBattleSwitch = base.transform.FindChild("MenuBar/MainMenuBattleSwitch").GetComponent<MenuBase>();
		_mainMenuBattleForceSwitch = base.transform.FindChild("MenuBar/MainMenuBattleForceSwitch").GetComponent<MenuBase>();
		_attackMenu = base.transform.FindChild("MenuBar/AttackMenu").GetComponent<BattleAttackMenu>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_attackLearnContainer = base.transform.FindChild("AttackLearnContainer");
	}

	public void SetUp(List<MortyData> mortyDatas, MortyData startingMortyData, MortyData currentBattleMorty, Dictionary<string, int> items, string selectedItemID)
	{
		_items = items;
		_selectedItemID = selectedItemID;
		SetUp(mortyDatas, startingMortyData, currentBattleMorty, false);
	}

	public void SetUp(List<MortyData> mortyDatas, MortyData startingMortyData, MortyData currentBattleMorty, bool isBattleForceSwitch)
	{
		_isBattle = currentBattleMorty != null;
		_levelUpSwapAttackIndex = -1;
		SetSwitchBtnAvailable(false);
		SetItemBtnAvailable(false);
		SetStatsBtnAvailable(false);
		_mortyMenu.SetUp(1, false, 0f, 0f, -4f);
		CreateMortyCells(mortyDatas, currentBattleMorty);
		_currentBattleCell = GetCellForData(currentBattleMorty);
		_mortyMenu.TrySetCellSelected(GetCellForData(startingMortyData));
		if (_selectedItemID != null && _items != null)
		{
			if (_selectedCell != null)
			{
				UseItem();
			}
			else
			{
				ShowMenu(_mainMenuItem);
			}
		}
		else if (_isBattle)
		{
			if (isBattleForceSwitch)
			{
				ShowMenu(_mainMenuBattleForceSwitch);
			}
			else
			{
				ShowMenu(_mainMenuBattleSwitch);
			}
		}
		else
		{
			ShowMenu(_mainMenu);
		}
		_attackMenu.SetPositionOut();
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
		if (_mortyMenu.selectedCount == 0)
		{
			if (_selectedItemID == null)
			{
				_dialogue.SetText(TextDefs.PARTY_DIAL_SET_UP);
			}
			else
			{
				_dialogue.SetText(string.Format(TextDefs.PARTY_DIAL_SET_UP_BAG, ItemDefs.Name(_selectedItemID)));
			}
		}
	}

	protected override void AddEvents()
	{
		_mortyMenu.SelectedEvent += MortyCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_mainMenuItem.BtnPressedEvent += MainMenuItemBtnPressedHandler;
		_mainMenuSwitch.BtnPressedEvent += MainMenuSwitchBtnPressedHandler;
		_mainMenuBattleSwitch.BtnPressedEvent += MainMenuBattleSwitchBtnPressedHandler;
		_attackMenu.BtnPressedEvent += AttackMenuBtnPressedHandler;
		_mainMenuBattleForceSwitch.BtnPressedEvent += MainMenuBattleForceSwitchBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mortyMenu.SelectedEvent -= MortyCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_mainMenuItem.BtnPressedEvent -= MainMenuItemBtnPressedHandler;
		_mainMenuSwitch.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_mainMenuBattleSwitch.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_attackMenu.BtnPressedEvent -= AttackMenuBtnPressedHandler;
		_mainMenuBattleForceSwitch.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateMortyCells(List<MortyData> morties, MortyData currentBattleMorty)
	{
		for (int i = 0; i < 5; i++)
		{
			if (i >= morties.Count)
			{
				AddMortyCell(null, false);
				continue;
			}
			MortyData mortyData = morties[i];
			AddMortyCell(mortyData, currentBattleMorty != null && currentBattleMorty == mortyData);
		}
	}

	private void AddMortyCell(MortyData mortyData, bool isOut)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MortyCell"));
		MortyCell component = rectTransform.GetComponent<MortyCell>();
		component.SetUp(mortyData, isOut);
		_mortyMenu.AddCell(component);
	}

	private MortyCell GetCellForData(MortyData mortyData)
	{
		if (mortyData == null)
		{
			return null;
		}
		for (int i = 0; i < _mortyMenu.count; i++)
		{
			MortyCell mortyCell = (MortyCell)_mortyMenu.GetCellAtIndex(i);
			if (mortyCell.data == mortyData)
			{
				return mortyCell;
			}
		}
		return null;
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
		_mortyMenu.SetInteractable(false);
		StartCoroutine(_mainMenu.TweenOut());
		StartCoroutine(_mainMenuItem.TweenOut());
		StartCoroutine(_mainMenuSwitch.TweenOut());
		StartCoroutine(_mainMenuBattleSwitch.TweenOut());
		StartCoroutine(_mainMenuBattleForceSwitch.TweenOut());
		if (menu != null)
		{
			yield return StartCoroutine(menu.TweenIn());
			_mortyMenu.SetInteractable(true);
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

	protected virtual IEnumerator ShowAttackMenu()
	{
		RefreshAttackMenu();
		yield return StartCoroutine(_attackMenu.TweenIn());
	}

	private IEnumerator HideAttackMenu()
	{
		yield return StartCoroutine(_attackMenu.TweenOut());
	}

	private void RefreshAttackMenu()
	{
		for (int i = 0; i < 4; i++)
		{
			if (i < _selectedCell.data.attackIDs.Count && _selectedCell.data.attackIDs[i] != null)
			{
				_attackMenu.SetBtnEnabled(i, true);
				string id = _selectedCell.data.attackIDs[i];
				AttackData data = AttackDefs.GetData(id);
				_attackMenu.SetUpBtn(i, new AttackDisplayInfo(data, _selectedCell.data.attackPPs[i]));
			}
			else
			{
				_attackMenu.SetBtnEnabled(i, false);
			}
		}
	}

	private void StartSortOrder()
	{
		_isSortMode = true;
		_mortyMenu.SetUpSelectMax(2);
		StartCoroutine(ShowMenuWithTween(_mainMenuSwitch));
		_dialogue.TweenText(TextDefs.PARTY_DIAL_SORT);
	}

	private void EndSortOrder()
	{
		_isSortMode = false;
		_mortyMenu.SetUpSelectMax(1);
		StartCoroutine(ShowMenuWithTween(_mainMenu));
		_dialogue.TweenText(TextDefs.PARTY_DIAL_SELECTED);
	}

	private IEnumerator SortMorties(MortyCell fromCell, MortyCell toCell)
	{
		ActivateInput(false);
		if (this.SwitchEvent != null)
		{
			this.SwitchEvent(_mortyMenu.GetIndexFromCell(fromCell), _mortyMenu.GetIndexFromCell(toCell));
		}
		_mortyMenu.SwapCellsIndex(fromCell, toCell);
		_mortyMenu.SetUpSelectMax(1);
		if (this.MortySelectedEvent != null)
		{
			this.MortySelectedEvent(fromCell.data);
		}
		yield return StartCoroutine(SortMortiesTween(fromCell, toCell));
		EndSortOrder();
		ActivateInput(true);
	}

	private IEnumerator SortMortiesTween(CellBase fromCell, CellBase toCell)
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

	private void TryUseItem()
	{
		if (_selectedItemID != null)
		{
			if (_isBattle)
			{
				if (this.UseItemInBattleEvent != null)
				{
					this.UseItemInBattleEvent();
				}
			}
			else
			{
				UseItem(true);
			}
		}
		else if (this.UseItemNotYetSetEvent != null)
		{
			this.UseItemNotYetSetEvent();
		}
	}

	private void UseItem(bool bagStateOnComplete = false)
	{
		StartCoroutine(UseItemCo(_selectedCell, _selectedItemID, bagStateOnComplete));
		_selectedItemID = null;
	}

	private IEnumerator UseItemCo(MortyCell mortyCell, string itemID, bool bagStateOnComplete)
	{
		StartCoroutine(ShowMenuWithTween(null));
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.PARTY_DIAL_USE, ItemDefs.Name(itemID), MortyDefs.Name(mortyCell.data.id)), 2f, true));
		string resultText = null;
		bool isSuccess = ItemDefs.ApplyItemToMorty(itemID, _items, mortyCell.data, null, TextDefs.PRONOUN_PLAYERS, out resultText);
		if (isSuccess)
		{
			App.upsight.CustomAnalyticEvent(eSubType1.ITEMS, eSubType2.USE, itemID);
			App.sound.PlaySFX("Audio/SFX/UseItem");
			switch (ItemDefs.EffectType(itemID))
			{
			case "StatIncrease":
				if (ItemDefs.EffectStatType(itemID).Equals("Level"))
				{
					_selectedCell.PulseLevelBar();
					App.sound.PlaySFX("Audio/SFX/LevelUP", 0.5f);
				}
				break;
			case "RestoreHP":
			case "Revive":
				yield return StartCoroutine(_selectedCell.TweenToHP(mortyCell.data.hp));
				break;
			}
		}
		mortyCell.Refresh(null, true);
		if (resultText != null)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(resultText, 2f, true));
		}
		if (isSuccess && "StatIncrease".Equals(ItemDefs.EffectType(itemID)) && ItemDefs.EffectStatType(itemID).Equals("Level"))
		{
			yield return StartCoroutine(UsedLevelUpItemCo(mortyCell));
		}
		if (!bagStateOnComplete)
		{
			_dialogue.TweenText(TextDefs.PARTY_DIAL_SELECTED);
			StartCoroutine(ShowMenuWithTween(_mainMenu));
		}
		if (this.UseItemFinishedEvent != null)
		{
			this.UseItemFinishedEvent(isSuccess, bagStateOnComplete);
		}
	}

	public IEnumerator UsedLevelUpItemCo(MortyCell mortyCell)
	{
		if (mortyCell.data.id.Equals("MortyEgg") && mortyCell.data.level >= 20)
		{
			MortyData eggHatchData = MortyDefs.GetNewMortyData(MortyDefs.Evolution(mortyCell.data.id), mortyCell.data.level, eIVSetUp.RANDOM, eEVSetUp.RANDOM);
			int eggMortyIndex = App.saveData.player.partyMorties.IndexOf(mortyCell.data);
			PlayerDefs.RemoveOwnedMorty(mortyCell.data);
			string resultText;
			PlayerDefs.SetMortyCaught(eggHatchData, out resultText);
			App.saveData.player.partyMorties.Remove(eggHatchData);
			App.saveData.player.partyMorties.Insert(eggMortyIndex, eggHatchData);
			mortyCell.Refresh(eggHatchData);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.MORTY_EGG_HATCHED, MortyDefs.Name("MortyEgg"), MortyDefs.Name(eggHatchData.id)), 2f, true));
		}
		else
		{
			yield return StartCoroutine(UsedLevelUpItemLearnAttackCo(mortyCell));
		}
	}

	public IEnumerator UsedLevelUpItemLearnAttackCo(MortyCell mortyCell)
	{
		List<KeyValuePair<int, string>> unlockableAttacks = MortyDefs.Attacks(mortyCell.data.id);
		KeyValuePair<int, string>[] attacksToUnlock = unlockableAttacks.FindAll((KeyValuePair<int, string> keyValuePair) => keyValuePair.Key == mortyCell.data.level).ToArray();
		for (int a = 0; a < attacksToUnlock.Length; a++)
		{
			AttackData attackData = AttackDefs.GetData(attacksToUnlock[a].Value);
			eAttackCatagory attackCatagory = AttackDefs.GetAttackCatagory(attackData);
			int availableAttackSlotIndex = mortyCell.data.GetAvailableAttackSlot();
			if (availableAttackSlotIndex > -1)
			{
				_levelUpSwapAttackIndex = availableAttackSlotIndex;
				mortyCell.data.AddAttackToSlot(availableAttackSlotIndex, attackData.id);
			}
			else
			{
				RectTransform attackBoxTrans = UI.Instantiate(string.Format("UI/Menu/AttackLearnNewBox/AttackLearnNewBox{0}", attackCatagory), _attackLearnContainer);
				_newAttackBox = attackBoxTrans.GetComponent<AttackLearnNewBox>();
				_newAttackBox.SetUp(new AttackDisplayInfo(attackData));
				_newAttackBox.SetPositionOut();
				yield return new WaitForEndOfFrame();
				StartCoroutine(_newAttackBox.TweenIn());
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_WANTS_TO_LEARN_1, MortyDefs.Name(mortyCell.data.id), attackData.name), 2f, true));
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_WANTS_TO_LEARN_2, 2f, true));
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_WANTS_TO_LEARN_3, attackData.name)));
				StartCoroutine(ShowAttackMenu());
				while (_levelUpSwapAttackIndex == -1)
				{
					yield return null;
				}
				ActivateInput(false);
				if (_levelUpSwapAttackIndex != 4)
				{
					string forgetAttackID = mortyCell.data.attackIDs[_levelUpSwapAttackIndex];
					string forgetAttackName = AttackDefs.GetData(forgetAttackID).name;
					mortyCell.data.RemoveAttackFromSlot(_levelUpSwapAttackIndex);
					RefreshAttackMenu();
					yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_FORGOT_ATTACK, MortyDefs.Name(mortyCell.data.id), forgetAttackName), 2f, true));
					mortyCell.data.AddAttackToSlot(_levelUpSwapAttackIndex, attackData.id);
					RefreshAttackMenu();
				}
				else
				{
					yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_DID_NOT_LEARN, MortyDefs.Name(mortyCell.data.id), attackData.name), 2f, true));
				}
				StartCoroutine(_newAttackBox.TweenOut(true));
			}
			if (_levelUpSwapAttackIndex > -1 && _levelUpSwapAttackIndex < 4)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_LEARNED, MortyDefs.Name(mortyCell.data.id), attackData.name), 2f, true));
			}
			StartCoroutine(HideAttackMenu());
			_levelUpSwapAttackIndex = -1;
			ActivateInput(true);
		}
	}

	public void MortyCellSelectedHandler(CellBase cell, int index)
	{
		if (_isSortMode)
		{
			StartCoroutine(SortMorties(_selectedCell, (MortyCell)cell));
			return;
		}
		_selectedCell = (MortyCell)cell;
		if (_isBattle && (_currentBattleCell == _selectedCell || _selectedCell.data.isFainted))
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
			if (ItemDefs.CheckCanApplyItemToMorty(_selectedItemID, _selectedCell.data, out resultText))
			{
				SetItemBtnAvailable(true);
				_dialogue.TweenText(string.Format(TextDefs.PARTY_DIAL_USE_BAG, ItemDefs.Name(_selectedItemID), MortyDefs.Name(_selectedCell.data.id)));
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
			StartSortOrder();
			break;
		case 1:
			TryUseItem();
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
			TryUseItem();
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
			EndSortOrder();
		}
	}

	public void MainMenuBattleSwitchBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.SwitchInBattleEvent != null)
			{
				this.SwitchInBattleEvent();
			}
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
			if (this.SwitchInBattleEvent != null)
			{
				this.SwitchInBattleEvent();
			}
			break;
		case 1:
			if (this.StatsEvent != null)
			{
				this.StatsEvent();
			}
			break;
		}
	}

	public void AttackMenuBtnPressedHandler(int index)
	{
		_levelUpSwapAttackIndex = index;
	}
}
