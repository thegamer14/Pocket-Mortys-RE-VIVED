using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MortyLabAttacksUI : UIBase
{
	public delegate void AttacksUIBtnDelegate();

	public delegate void MortyLabAttacksUIBtnDelegate();

	private const float _kAttackMenuPaddingTop = 0f;

	private const float _kAttackMenuPaddingBottom = 0f;

	private const float _kAttackMenuCellGap = -8f;

	private MortyData _mortyData;

	private string _selectedItemID;

	private BattleAttackMenu _battleAttackMenu;

	private MenuDialogue _dialogue;

	private Image _mortyImage;

	private CouponBar _couponBar;

	private ScrollMenu _attackScrollMenu;

	public List<string> tempAttackIDs { get; private set; }

	public event AttacksUIBtnDelegate BackEvent;

	public event MortyLabAttacksUIBtnDelegate BuyCouponsBtnEvent;

	protected override void LateAwake()
	{
		_attackScrollMenu = base.transform.FindChild("AttackScrollMenu").GetComponent<ScrollMenu>();
		_battleAttackMenu = base.transform.FindChild("MenuBar/AttackMenu").GetComponent<BattleAttackMenu>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_mortyImage = base.transform.FindChild("Morty").GetComponent<Image>();
		_couponBar = base.transform.FindChild("CouponBar").GetComponent<CouponBar>();
	}

	public void SetUp(MortyData mortyData)
	{
		_mortyData = mortyData;
		tempAttackIDs = new List<string>(_mortyData.attackIDs);
		SetMortyImage();
		_attackScrollMenu.SetUp(1, false, 0f, 0f, -8f);
		_couponBar.SetUp(PlayerDefs.CouponCount);
		_couponBar.SetLabels();
		RefreshAttacks();
	}

	private void RefreshDialogue()
	{
		if (_attackScrollMenu.count > 1)
		{
			_dialogue.SetText(TextDefs.MORTY_LAB_PICK_ATTACK_DIAL);
		}
		else
		{
			_dialogue.SetText(TextDefs.MORTY_LAB_NO_ATTACKS);
		}
	}

	public void SetMortyImage()
	{
		_mortyImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MortyDefs.AssetID(_mortyData.id), "frontFlipped"));
		if (_mortyImage.sprite == null)
		{
			_mortyImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MortyDefs.AssetID(_mortyData.id), "front"));
		}
		_mortyImage.SetNativeSize();
	}

	protected override void AddEvents()
	{
		_attackScrollMenu.SelectedEvent += MortyCellSelectedHandler;
		_battleAttackMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_couponBar.GetMoreEvent += CoinBarGetMoreHandler;
		PlayerDefs.CouponCountChangedEvent += CouponCountChangedHandler;
	}

	protected override void RemoveEvents()
	{
		_attackScrollMenu.SelectedEvent -= MortyCellSelectedHandler;
		_battleAttackMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_couponBar.GetMoreEvent -= CoinBarGetMoreHandler;
		PlayerDefs.CouponCountChangedEvent -= CouponCountChangedHandler;
	}

	private void RefreshAttacks()
	{
		RefreshForgotAttacks();
		RefreshLearnedAttacks();
		RefreshDialogue();
	}

	private void RefreshForgotAttacks()
	{
		_attackScrollMenu.RemoveAllCells();
		_attackScrollMenu.AddHeaderCell(TextDefs.MORTY_LAB_TITLE_FORGOT_ATTACKS);
		List<KeyValuePair<int, string>> list = MortyDefs.Attacks(_mortyData.id);
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<int, string> item in list)
		{
			if (item.Key <= _mortyData.level && !tempAttackIDs.Contains(item.Value))
			{
				list2.Add(item.Value);
			}
		}
		for (int i = 0; i < list2.Count; i++)
		{
			AddForgotAttackCell(AttackDefs.GetData(list2[i]));
		}
	}

	private void AddForgotAttackCell(AttackData attackData)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "AttackCell"));
		AttackCell component = rectTransform.GetComponent<AttackCell>();
		component.SetUp(attackData);
		_attackScrollMenu.AddCell(component);
	}

	private void RefreshLearnedAttacks()
	{
		for (int i = 0; i < 4; i++)
		{
			if (i < tempAttackIDs.Count && tempAttackIDs[i] != null)
			{
				_battleAttackMenu.SetBtnEnabled(i, true);
				AttackData data = AttackDefs.GetData(tempAttackIDs[i]);
				_battleAttackMenu.SetUpBtn(i, new AttackDisplayInfo(data), false);
			}
			else
			{
				_battleAttackMenu.SetBtnEnabled(i, false);
			}
		}
	}

	private void ForgetAttackAtIndex(int atIndex)
	{
		if (!string.IsNullOrEmpty(tempAttackIDs[atIndex]))
		{
			string id = tempAttackIDs[atIndex];
			tempAttackIDs[atIndex] = null;
			RefreshAttacks();
			AttackCell attackCell = (AttackCell)_attackScrollMenu.GetCellForID(id);
			_attackScrollMenu.TweenToCell(attackCell);
			attackCell.TweenIn();
		}
	}

	private void TryLearnAttack(string attackID)
	{
		for (int i = 0; i < 4; i++)
		{
			if (string.IsNullOrEmpty(tempAttackIDs[i]))
			{
				tempAttackIDs[i] = attackID;
				RefreshAttacks();
				return;
			}
		}
		_dialogue.TweenText(TextDefs.MORTY_LAB_NO_SLOTS);
	}

	public void MortyCellSelectedHandler(CellBase cell, int index)
	{
		cell.SetSelect(false);
		TryLearnAttack(((AttackCell)cell).data.id);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		if (index == 4)
		{
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
		}
		else
		{
			ForgetAttackAtIndex(index);
		}
	}

	private void CouponCountChangedHandler(int count)
	{
		_couponBar.SetCount(PlayerDefs.CouponCount);
	}

	public void CoinBarGetMoreHandler()
	{
		if (this.BuyCouponsBtnEvent != null)
		{
			this.BuyCouponsBtnEvent();
		}
	}
}
