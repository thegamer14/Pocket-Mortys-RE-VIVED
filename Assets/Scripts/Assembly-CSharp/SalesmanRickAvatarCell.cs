using UnityEngine;
using UnityEngine.UI;

public class SalesmanRickAvatarCell : CellBase
{
	public Sprite owenedSpr;

	private Image _icon;

	private Text _nameLabel;

	private RectTransform _costBar;

	private Text _costLabel;

	private GameObject _coinIcon;

	private GameObject _couponIcon;

	private RectTransform _ownedBar;

	private Text _ownedLabel;

	private RectTransform _lockedBar;

	private Text _lockedLabel;

	private RectTransform _activeBar;

	private Text _activeLabel;

	public eAvatarCellState avatarCellState { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
		_costBar = base.transform.FindChild("CostBar").GetComponent<RectTransform>();
		_costLabel = base.transform.FindChild("CostBar/Label").GetComponent<Text>();
		_coinIcon = base.transform.FindChild("CostBar/CoinIcon").gameObject;
		_couponIcon = base.transform.FindChild("CostBar/CouponIcon").gameObject;
		_ownedBar = base.transform.FindChild("OwnedBar").GetComponent<RectTransform>();
		_ownedLabel = base.transform.FindChild("OwnedBar/Label").GetComponent<Text>();
		_lockedBar = base.transform.FindChild("LockedBar").GetComponent<RectTransform>();
		_lockedLabel = base.transform.FindChild("LockedBar/Label").GetComponent<Text>();
		_activeBar = base.transform.FindChild("ActiveBar").GetComponent<RectTransform>();
		_activeLabel = base.transform.FindChild("ActiveBar/Label").GetComponent<Text>();
		_activeBar.gameObject.SetActive(false);
	}

	public void SetUp(string id, eAvatarCellState avatarCellState, string name, eCurrencyType currency, int cost, string assetID)
	{
		base.SetUp(id);
		_ownedLabel.text = TextDefs.SALESMAN_RICK_CUSTOMIZE_OWNED;
		_lockedLabel.text = TextDefs.SALESMAN_RICK_CUSTOMIZE_LOCKED;
		_activeLabel.text = TextDefs.SALESMAN_RICK_CUSTOMIZE_ACTIVE;
		_nameLabel.text = name;
		if (cost <= 0)
		{
			_costLabel.text = TextDefs.SALESMAN_RICK_CUSTOMIZE_FREE;
			_coinIcon.SetActive(false);
			_couponIcon.SetActive(false);
		}
		else
		{
			_costLabel.text = ((currency != eCurrencyType.COIN) ? string.Format("x {0}", cost.ToString()) : cost.ToString());
			switch (currency)
			{
			case eCurrencyType.COIN:
				_coinIcon.SetActive(true);
				_couponIcon.SetActive(false);
				break;
			case eCurrencyType.COUPON:
				_coinIcon.SetActive(false);
				_couponIcon.SetActive(true);
				break;
			}
		}
		_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", assetID));
		_icon.SetNativeSize();
		SetAvatarCellState(avatarCellState);
		SetUsed(true);
	}

	public void SetAvatarCellState(eAvatarCellState avatarCellstate)
	{
		avatarCellState = avatarCellstate;
		_icon.color = Color.white;
		_ownedBar.gameObject.SetActive(false);
		_costBar.gameObject.SetActive(false);
		_lockedBar.gameObject.SetActive(false);
		switch (avatarCellstate)
		{
		case eAvatarCellState.LOCKED:
			_icon.color = Color.black;
			_nameLabel.text = TextDefs.SALESMAN_RICK_CUSTOMIZE_UNKOWN;
			_lockedBar.gameObject.SetActive(true);
			break;
		case eAvatarCellState.UNLOCKED:
			_costBar.gameObject.SetActive(true);
			break;
		case eAvatarCellState.OWNED:
			_ownedBar.gameObject.SetActive(true);
			if (base.state == eScrollMenuCellState.UNSELECTED)
			{
				SetBackground(owenedSpr);
			}
			break;
		}
	}

	public void SetAsActive(bool value)
	{
		_activeBar.gameObject.SetActive(value);
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		if (base.state == eScrollMenuCellState.UNSELECTED && avatarCellState == eAvatarCellState.OWNED)
		{
			SetBackground(owenedSpr);
		}
	}

	public override void SetEnabled(bool value)
	{
		base.SetEnabled(value);
		if (base.state == eScrollMenuCellState.UNSELECTED && avatarCellState == eAvatarCellState.OWNED)
		{
			SetBackground(owenedSpr);
		}
	}

	public override void SetSelect(bool value)
	{
		base.SetSelect(value);
		if (base.state == eScrollMenuCellState.UNSELECTED && avatarCellState == eAvatarCellState.OWNED)
		{
			SetBackground(owenedSpr);
		}
	}
}
