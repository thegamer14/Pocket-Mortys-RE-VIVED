using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MPMortyLabAttacksUI : UIBase
{
	public delegate void AttacksUIBtnDelegate();

	public delegate void MortyLabAttacksUIBtnDelegate();

	private const float _kAttackMenuPaddingTop = 0f;

	private const float _kAttackMenuPaddingBottom = 0f;

	private const float _kAttackMenuCellGap = -8f;

	private MPOpponentMortyData _mortyData;

	private string _selectedItemID;

	private BattleAttackMenu _battleAttackMenu;

	private MenuDialogue _dialogue;

	private Image _mortyImage;

	private MPCouponBar _couponBar;

	private ScrollMenu _attackScrollMenu;

	public SortedList<int, MPAttackData> tempAttacks { get; private set; }

	public event AttacksUIBtnDelegate BackEvent;

	public event MortyLabAttacksUIBtnDelegate BuyCouponsBtnEvent;

	protected override void LateAwake()
	{
		_attackScrollMenu = base.transform.FindChild("AttackScrollMenu").GetComponent<ScrollMenu>();
		_battleAttackMenu = base.transform.FindChild("MenuBar/AttackMenu").GetComponent<BattleAttackMenu>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_mortyImage = base.transform.FindChild("Morty").GetComponent<Image>();
		_couponBar = base.transform.FindChild("CouponBar").GetComponent<MPCouponBar>();
	}

	public void SetUp(MPOpponentMortyData mortyData)
	{
		_mortyData = mortyData;
		tempAttacks = new SortedList<int, MPAttackData>(_mortyData.attacks);
		SetMortyImage();
		_attackScrollMenu.SetUp(1, false, 0f, 0f, -8f);
		_couponBar.SetUp(App.net.player.data.coupons);
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
		_mortyImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPMortyDefs.AssetID(_mortyData.id), "frontFlipped"));
		if (_mortyImage.sprite == null)
		{
			_mortyImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPMortyDefs.AssetID(_mortyData.id), "front"));
		}
		_mortyImage.SetNativeSize();
	}

	protected override void AddEvents()
	{
		_attackScrollMenu.SelectedEvent += AttackCellSelectedHandler;
		_battleAttackMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_couponBar.GetMoreEvent += CouponBarGetMoreHandler;
		App.net.player.data.CouponsChangedEvent += CouponCountChangedHandler;
	}

	protected override void RemoveEvents()
	{
		_attackScrollMenu.SelectedEvent -= AttackCellSelectedHandler;
		_battleAttackMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_couponBar.GetMoreEvent -= CouponBarGetMoreHandler;
		App.net.player.data.CouponsChangedEvent -= CouponCountChangedHandler;
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
		List<string> attacksForMorty = MPMortyDefs.GetAttacksForMorty(_mortyData.id, 0, _mortyData.level);
		foreach (string attackID in attacksForMorty)
		{
			if (!tempAttacks.Any((KeyValuePair<int, MPAttackData> kvp) => kvp.Value != null && kvp.Value.id.Equals(attackID)))
			{
				AddForgotAttackCell(new MPAttackData(attackID, MPAttackDefs.PP(attackID)));
			}
		}
	}

	private void AddForgotAttackCell(MPAttackData attackData)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPAttackCell"));
		MPAttackCell component = rectTransform.GetComponent<MPAttackCell>();
		component.SetUp(attackData);
		_attackScrollMenu.AddCell(component);
	}

	private void RefreshLearnedAttacks()
	{
		for (int i = 0; i < 4; i++)
		{
			if (tempAttacks.ContainsKey(i))
			{
				_battleAttackMenu.SetBtnEnabled(i, true);
				_battleAttackMenu.SetUpBtn(i, new AttackDisplayInfo(tempAttacks[i].id, tempAttacks[i].PP), false);
			}
			else
			{
				_battleAttackMenu.SetBtnEnabled(i, false);
			}
		}
	}

	private void ForgetAttackAtIndex(int atIndex)
	{
		if (tempAttacks.ContainsKey(atIndex))
		{
			MPAttackData mPAttackData = tempAttacks[atIndex];
			tempAttacks.Remove(atIndex);
			RefreshAttacks();
			MPAttackCell mPAttackCell = (MPAttackCell)_attackScrollMenu.GetCellForID(mPAttackData.id);
			_attackScrollMenu.TweenToCell(mPAttackCell);
			mPAttackCell.TweenIn();
		}
	}

	private void TryLearnAttack(MPAttackData attackData)
	{
		for (int i = 0; i < 4; i++)
		{
			if (!tempAttacks.ContainsKey(i))
			{
				tempAttacks.Add(i, attackData);
				RefreshAttacks();
				return;
			}
		}
		_dialogue.TweenText(TextDefs.MORTY_LAB_NO_SLOTS);
	}

	public void AttackCellSelectedHandler(CellBase cell, int index)
	{
		cell.SetSelect(false);
		TryLearnAttack(((MPAttackCell)cell).data);
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
		_couponBar.SetCount(count);
	}

	public void CouponBarGetMoreHandler()
	{
		if (this.BuyCouponsBtnEvent != null)
		{
			this.BuyCouponsBtnEvent();
		}
	}
}
