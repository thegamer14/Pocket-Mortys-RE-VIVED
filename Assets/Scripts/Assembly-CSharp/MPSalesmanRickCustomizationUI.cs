using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPSalesmanRickCustomizationUI : UIBase
{
	public delegate void CustomizationUIIDBtnDelegate(string avatarID);

	public delegate void CustomizationUIBtnDelegate();

	private const int _kAvatarMenuMinimumCellCount = 5;

	private const float _kAvatarMenuPaddingTop = 85f;

	private const float _kAvatarMenuPaddingBottom = 100f;

	private const float _kAvatarMenuCellGap = -4f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private MPCoinBar _coinBar;

	private MPCouponBar _couponBar;

	private RectTransform _hologramTrans;

	private Image _hologramImg;

	private Image _avatarImg;

	private Image _avatarOverlayImg;

	private ScrollMenu _avatarMenu;

	private string selectedAvatarID;

	private int _coinValue;

	private int _couponValue;

	private bool _sfxEnabled;

	private Tween _coinTween;

	private Tween _couponTween;

	private Coroutine _animateAvatarCo;

	private Color _defaultAvatarOverlayColor;

	public event CustomizationUIIDBtnDelegate UseEvent;

	public event CustomizationUIIDBtnDelegate BuyEvent;

	public event CustomizationUIBtnDelegate BackEvent;

	public event CustomizationUIBtnDelegate GetCoinsEvent;

	public event CustomizationUIBtnDelegate GetCouponsEvent;

	protected override void LateAwake()
	{
		_avatarMenu = base.transform.FindChild("AvatarMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_coinBar = base.transform.FindChild("MenuBar/MainMenu/CoinBar").GetComponent<MPCoinBar>();
		_couponBar = base.transform.FindChild("MenuBar/MainMenu/CouponBar").GetComponent<MPCouponBar>();
		_hologramTrans = base.transform.FindChild("HologramSprite") as RectTransform;
		_hologramImg = _hologramTrans.GetComponent<Image>();
		_avatarImg = base.transform.FindChild("AvatarSprite").GetComponent<Image>();
		_avatarOverlayImg = base.transform.FindChild("AvatarSprite/Overlay").GetComponent<Image>();
		_defaultAvatarOverlayColor = _avatarOverlayImg.color;
	}

	public void SetUp()
	{
		selectedAvatarID = null;
		_avatarMenu.SetUp(1, false, 85f, 100f, -4f);
		CreateCells();
		_mainMenu.SetPositionIn();
		_mainMenu.SetBtnVisible(2, false);
		_mainMenu.SetBtnVisible(3, false);
		_coinValue = App.net.player.data.coins;
		_coinBar.SetUp(App.net.player.data.coins);
		_couponValue = App.net.player.data.coupons;
		_couponBar.SetUp(App.net.player.data.coupons);
		_avatarMenu.TryCenterAndSelectCellWithID(App.net.player.data.avatarId);
		_sfxEnabled = true;
		_dialogue.SetText(TextDefs.SALESMAN_RICK_CUSTOMIZE_DIAL);
	}

	public override void SetLabels()
	{
		_coinBar.SetLabels();
		_couponBar.SetLabels();
		_mainMenu.SetBtnTitle(2, TextDefs.SALESMAN_RICK_CUSTOMIZE_BTN_USE);
		_mainMenu.SetBtnTitle(3, TextDefs.SALESMAN_RICK_CUSTOMIZE_BTN_BUY);
		_dialogue.SetText(TextDefs.SALESMAN_RICK_CUSTOMIZE_DIAL);
	}

	protected override void AddEvents()
	{
		_avatarMenu.SelectedEvent += CellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_avatarMenu.SelectedEvent -= CellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateCells()
	{
		List<string> avatarIDs = MPPlayerAvatarDefs.GetAvatarIDs();
		int num = Mathf.Max(5, avatarIDs.Count);
		for (int i = 0; i < num; i++)
		{
			AddCell((i >= avatarIDs.Count) ? null : avatarIDs[i]);
		}
	}

	private void AddCell(string id)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPSalesmanRickAvatarCell"));
		SalesmanRickAvatarCell component = rectTransform.GetComponent<SalesmanRickAvatarCell>();
		eAvatarCellState avatarCellState = ((!PlayerAvatarDefs.IsAvatarLockedForCouncilLevel(id, App.saveData.player.councilMembersDefeated)) ? ((!App.saveData.player.boughtPlayerAvatarIDs.Contains(id) && !App.net.player.data.IsAvatarOwned(id) && MPPlayerAvatarDefs.Cost(id) != 0 && !PlayerAvatarDefs.IsAvatarLockedByCouncilLevel(id)) ? eAvatarCellState.UNLOCKED : eAvatarCellState.OWNED) : eAvatarCellState.LOCKED);
		component.SetUp(id, avatarCellState, MPPlayerAvatarDefs.Name(id), MPPlayerAvatarDefs.CurrencyType(id), MPPlayerAvatarDefs.Cost(id), MPPlayerAvatarDefs.AssetID(id));
		component.SetAsActive(App.net.player.data.assetId.Equals(id));
		_avatarMenu.AddCell(component);
	}

	private void SetCoins(int val)
	{
		_coinValue = val;
		_coinBar.SetCount(_coinValue);
	}

	public void TweenCoins(int val)
	{
		_coinTween = DOTween.To(() => _coinValue, delegate(int x)
		{
			_coinValue = x;
		}, val, 1f);
		_coinTween.OnUpdate(delegate
		{
			SetCoins(_coinValue);
		});
	}

	private void SetCoupons(int val)
	{
		_couponValue = val;
		_couponBar.SetCount(_couponValue);
	}

	public void TweenCoupons(int val)
	{
		_couponTween = DOTween.To(() => _couponValue, delegate(int x)
		{
			_couponValue = x;
		}, val, 0.5f);
		_couponTween.OnUpdate(delegate
		{
			SetCoupons(_couponValue);
		});
	}

	public void DisplayCantAfford(eCurrencyType currency)
	{
		switch (currency)
		{
		case eCurrencyType.COIN:
			_dialogue.TweenText(string.Format(MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_NO_COIN, MPPlayerAvatarDefs.Name(selectedAvatarID)));
			break;
		case eCurrencyType.COUPON:
			_dialogue.TweenText(string.Format(MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_NO_COUPON, MPPlayerAvatarDefs.Name(selectedAvatarID)));
			break;
		}
	}

	public void DisplayWaiting()
	{
		StartCoroutine(_mainMenu.TweenOut());
		_dialogue.TweenText(MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_WAIT);
	}

	public void DisplayBuyError()
	{
		StartCoroutine(_mainMenu.TweenIn());
		_dialogue.TweenText(MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_BUY_ERROR);
	}

	public void DisplayPurchaseComplete()
	{
		StartCoroutine(_mainMenu.TweenIn());
		(_avatarMenu.GetCellForID(selectedAvatarID) as SalesmanRickAvatarCell).SetAvatarCellState(eAvatarCellState.OWNED);
		AvatarSet(selectedAvatarID);
	}

	public void DisplayUseError()
	{
		StartCoroutine(_mainMenu.TweenIn());
		_dialogue.TweenText(MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_USE_ERROR);
	}

	private void AvatarSet(string avatarID)
	{
		for (int i = 0; i < _avatarMenu.count; i++)
		{
			SalesmanRickAvatarCell salesmanRickAvatarCell = _avatarMenu.GetCellAtIndex(i) as SalesmanRickAvatarCell;
			salesmanRickAvatarCell.SetAsActive(salesmanRickAvatarCell.id.Equals(avatarID));
		}
		RefreshForSelectedAvatarID();
		_dialogue.TweenText(string.Format(TextDefs.SALESMAN_RICK_CUSTOMIZE_USED, MPPlayerAvatarDefs.Name(avatarID)));
	}

	private void RefreshForSelectedAvatarID()
	{
		if (!string.IsNullOrEmpty(selectedAvatarID))
		{
			eAvatarCellState avatarCellState = ((SalesmanRickAvatarCell)_avatarMenu.GetCellForID(selectedAvatarID)).avatarCellState;
			if (avatarCellState == eAvatarCellState.LOCKED)
			{
				_mainMenu.SetBtnVisible(2, false);
				_mainMenu.SetBtnVisible(3, false);
				int num = PlayerAvatarDefs.CouncilRequirement(selectedAvatarID);
				string text = ((num != 1) ? string.Format(MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_LOCKED_SELECTED_PLUR_DIAL, num) : MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_LOCKED_SELECTED_DIAL);
				_dialogue.TweenText(text);
				return;
			}
			if (App.net.player.data.avatarId.Equals(selectedAvatarID))
			{
				_mainMenu.SetBtnVisible(2, false);
				_mainMenu.SetBtnVisible(3, false);
				_dialogue.TweenText(TextDefs.SALESMAN_RICK_CUSTOMIZE_CURRENT_SELECTED_DIAL);
				return;
			}
			if (avatarCellState == eAvatarCellState.OWNED)
			{
				_mainMenu.SetBtnVisible(2, true);
				_mainMenu.SetBtnVisible(3, false);
				_dialogue.TweenText(string.Format(TextDefs.SALESMAN_RICK_CUSTOMIZE_OWNED_SELECTED_DIAL, MPPlayerAvatarDefs.Name(selectedAvatarID)));
				return;
			}
			_mainMenu.SetBtnVisible(2, false);
			_mainMenu.SetBtnVisible(3, true);
			if (MPPlayerAvatarDefs.CurrencyType(selectedAvatarID) == eCurrencyType.COIN)
			{
				_dialogue.TweenText(string.Format(MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_BUY_SELECTED_COIN_DIAL, MPPlayerAvatarDefs.Name(selectedAvatarID), MPPlayerAvatarDefs.Cost(selectedAvatarID)));
			}
			else if (MPPlayerAvatarDefs.Cost(selectedAvatarID) == 1)
			{
				_dialogue.TweenText(string.Format(MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_BUY_SELECTED_COUPON_DIAL, MPPlayerAvatarDefs.Name(selectedAvatarID), MPPlayerAvatarDefs.Cost(selectedAvatarID)));
			}
			else
			{
				_dialogue.TweenText(string.Format(MPTextDefs.MP_SALESMAN_RICK_CUSTOMIZE_BUY_SELECTED_COUPON_PLUR_DIAL, MPPlayerAvatarDefs.Name(selectedAvatarID), MPPlayerAvatarDefs.Cost(selectedAvatarID)));
			}
		}
		else
		{
			_mainMenu.SetBtnVisible(2, false);
			_mainMenu.SetBtnVisible(3, false);
			_dialogue.TweenText(string.Format(TextDefs.SALESMAN_RICK_CUSTOMIZE_DIAL, MPPlayerAvatarDefs.Name(selectedAvatarID)));
		}
	}

	private IEnumerator AnimateAvatarCo(string toAvatarId, bool locked = false)
	{
		if (_sfxEnabled)
		{
			App.sound.PlaySFX("Audio/SFX/CraftUseItem");
		}
		Sprite newSprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPPlayerAvatarDefs.AssetID(toAvatarId), "front"));
		_avatarImg.sprite = newSprite;
		_avatarImg.SetNativeSize();
		_avatarOverlayImg.sprite = newSprite;
		_avatarOverlayImg.SetNativeSize();
		if (locked)
		{
			_avatarImg.color = Color.black;
			_avatarOverlayImg.color = Color.black;
			_hologramImg.color = Color.black;
		}
		else
		{
			_avatarImg.color = Color.white;
			_avatarOverlayImg.color = _defaultAvatarOverlayColor;
			_hologramImg.color = Color.white;
		}
		_avatarImg.enabled = false;
		_avatarOverlayImg.enabled = false;
		_hologramTrans.gameObject.SetActive(true);
		_hologramTrans.DOKill();
		_hologramTrans.localScale = new Vector3(0.4f, 1.25f, 1f);
		yield return _hologramTrans.DOScale(Vector3.one, 0.1f).WaitForCompletion();
		_hologramTrans.gameObject.SetActive(false);
		_avatarOverlayImg.enabled = true;
		yield return new WaitForSeconds(0.1f);
		_avatarOverlayImg.enabled = false;
		_avatarImg.enabled = true;
		_animateAvatarCo = null;
	}

	public void CellSelectedHandler(CellBase cell, int index)
	{
		SalesmanRickAvatarCell salesmanRickAvatarCell = (SalesmanRickAvatarCell)cell;
		selectedAvatarID = salesmanRickAvatarCell.id;
		RefreshForSelectedAvatarID();
		if (_animateAvatarCo != null)
		{
			StopCoroutine(_animateAvatarCo);
		}
		_animateAvatarCo = StartCoroutine(AnimateAvatarCo(selectedAvatarID, salesmanRickAvatarCell.avatarCellState == eAvatarCellState.LOCKED));
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			App.sound.PlaySFX("Audio/SFX/GenericButton");
			if (this.GetCoinsEvent != null)
			{
				this.GetCoinsEvent();
			}
			break;
		case 1:
			App.sound.PlaySFX("Audio/SFX/GenericButton");
			if (this.GetCouponsEvent != null)
			{
				this.GetCouponsEvent();
			}
			break;
		case 2:
			if (this.UseEvent != null)
			{
				this.UseEvent(selectedAvatarID);
			}
			break;
		case 3:
			if (this.BuyEvent != null)
			{
				this.BuyEvent(selectedAvatarID);
			}
			break;
		case 4:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
