using UnityEngine;
using UnityEngine.UI;

public class MortydexCell : CellBase
{
	public Sprite _unseenIcon;

	private Image _icon;

	private Text _name;

	private Text _number;

	private Image _caughtIcon;

	private Image _ownedIcon;

	protected override void Awake()
	{
		base.Awake();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_name = base.transform.FindChild("Name").GetComponent<Text>();
		_number = base.transform.FindChild("Number").GetComponent<Text>();
		_caughtIcon = base.transform.FindChild("CaughtIcon").GetComponent<Image>();
		_ownedIcon = base.transform.FindChild("OwnedIcon").GetComponent<Image>();
	}

	public void SetUp(string id, string assetId, string name, string number, bool isSeen, bool isCaught, bool isOwned)
	{
		base.SetUp(id);
		SetUsed(id, assetId, name, number, isSeen, isCaught, isOwned);
	}

	private void SetUsed(string id, string assetId, string name, string number, bool isSeen, bool isCaught, bool isOwned)
	{
		bool flag = isSeen || isCaught || isOwned;
		base.SetUsed(flag);
		if (flag)
		{
			_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", assetId));
			_icon.SetNativeSize();
			_name.text = name;
			_number.text = number;
			_caughtIcon.gameObject.SetActive(isCaught && !isOwned);
			_ownedIcon.gameObject.SetActive(isOwned);
		}
		else
		{
			_icon.sprite = _unseenIcon;
			_icon.SetNativeSize();
			_name.text = "???";
			_number.text = number;
			_caughtIcon.gameObject.SetActive(false);
			_ownedIcon.gameObject.SetActive(false);
		}
	}
}
