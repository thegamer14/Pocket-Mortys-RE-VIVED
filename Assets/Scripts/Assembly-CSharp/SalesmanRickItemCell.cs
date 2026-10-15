using UnityEngine;
using UnityEngine.UI;

public class SalesmanRickItemCell : CellBase
{
	private Image _icon;

	private Text _nameLabel;

	private Text _descriptionLabel;

	private RectTransform _costBar;

	private Text _costLabel;

	protected override void Awake()
	{
		base.Awake();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
		_descriptionLabel = base.transform.FindChild("DescriptionLabel").GetComponent<Text>();
		_costBar = base.transform.FindChild("CostBar").GetComponent<RectTransform>();
		_costLabel = base.transform.FindChild("CostBar/Label").GetComponent<Text>();
	}

	public override void SetUp(string id)
	{
		base.SetUp(id);
		SetUsed(id != null);
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		if (value)
		{
			_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", ItemDefs.AssetID(base.id)));
			_icon.SetNativeSize();
			_nameLabel.text = ItemDefs.Name(base.id);
			_descriptionLabel.text = ItemDefs.Description(base.id);
			_costLabel.text = ItemDefs.Cost(base.id).ToString();
		}
		else
		{
			_icon.gameObject.SetActive(false);
			_nameLabel.gameObject.SetActive(false);
			_descriptionLabel.gameObject.SetActive(false);
			_costBar.gameObject.SetActive(false);
		}
	}
}
