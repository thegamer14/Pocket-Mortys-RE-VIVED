using UnityEngine;
using UnityEngine.UI;

public class ItemTournamentCell : CellBase
{
	private Text _nameLabel;

	private Text _descriptionLabel;

	private Text _costLabel;

	private Image _icon;

	protected override void Awake()
	{
		base.Awake();
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
		_descriptionLabel = base.transform.FindChild("DescriptionLabel").GetComponent<Text>();
		_costLabel = base.transform.FindChild("Cost/Label").GetComponent<Text>();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
	}

	public void SetUp(string id, int cost)
	{
		base.SetUp(id);
		_costLabel.text = cost.ToString();
		SetUsed(id != null);
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		if (value)
		{
			_nameLabel.text = ItemDefs.Name(base.id);
			_descriptionLabel.text = ItemDefs.Description(base.id);
			_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", ItemDefs.AssetID(base.id)));
			_icon.SetNativeSize();
		}
		else
		{
			_nameLabel.gameObject.SetActive(false);
			_descriptionLabel.gameObject.SetActive(false);
			_costLabel.gameObject.SetActive(false);
			_icon.gameObject.SetActive(false);
		}
	}
}
