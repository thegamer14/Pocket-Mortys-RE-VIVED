using UnityEngine;
using UnityEngine.UI;

public class QuestActiveCell : CellBase
{
	private Image _iconImage;

	private Image _questIconImage;

	private Text _nameLabel;

	private Text _descriptionLabel;

	protected override void Awake()
	{
		base.Awake();
		_iconImage = base.transform.FindChild("Icon").GetComponent<Image>();
		_questIconImage = base.transform.FindChild("QuestIcon").GetComponent<Image>();
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
		_descriptionLabel = base.transform.FindChild("DescriptionLabel").GetComponent<Text>();
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
			_iconImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", QuestDefs.GiverAssetID(base.id)));
			_iconImage.SetNativeSize();
			_nameLabel.text = QuestDefs.Name(base.id);
			_descriptionLabel.text = QuestDefs.Description(base.id);
		}
		else
		{
			_iconImage.gameObject.SetActive(false);
			_questIconImage.gameObject.SetActive(false);
			_nameLabel.gameObject.SetActive(false);
			_descriptionLabel.gameObject.SetActive(false);
		}
	}
}
