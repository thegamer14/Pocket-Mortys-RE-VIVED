using UnityEngine;
using UnityEngine.UI;

public class ItemCell : CellBase
{
	private Text _nameLabel;

	private RectTransform _holdingBar;

	private Text _holdingLabel;

	private Image _icon;

	private string _name;

	protected string _description;

	private string _assetId;

	private int _holdingValue;

	protected override void Awake()
	{
		base.Awake();
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
		_holdingBar = base.transform.FindChild("HoldingBar").GetComponent<RectTransform>();
		_holdingLabel = base.transform.FindChild("HoldingBar/Label").GetComponent<Text>();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
	}

	public void SetUp(string id, string name, string description, string assetId, int holdingValue)
	{
		base.SetUp(id);
		_name = name;
		_description = description;
		_assetId = assetId;
		_holdingValue = holdingValue;
		SetUsed(name != null);
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		if (value)
		{
			_nameLabel.text = _name;
			_holdingLabel.text = string.Format("x{0}", _holdingValue);
			_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", _assetId));
			_icon.SetNativeSize();
		}
		else
		{
			_nameLabel.gameObject.SetActive(false);
			_holdingBar.gameObject.SetActive(false);
			_icon.gameObject.SetActive(false);
		}
	}
}
