using UnityEngine;
using UnityEngine.UI;

public class QuestCompleteCell : CellBase
{
	private Image _iconImage;

	private Image _solutionIconImage;

	private Image _tickIconImage;

	private Text _nameLabel;

	protected override void Awake()
	{
		base.Awake();
		_iconImage = base.transform.FindChild("Icon").GetComponent<Image>();
		_solutionIconImage = base.transform.FindChild("SolutionIcon").GetComponent<Image>();
		_tickIconImage = base.transform.FindChild("TickIcon").GetComponent<Image>();
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
	}

	public void SetUp(string id, string solutionID)
	{
		base.SetUp(id);
		SetUsed(id != null, solutionID);
	}

	public void SetUsed(bool value, string solutionID)
	{
		base.SetUsed(value);
		if (value)
		{
			_iconImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", QuestDefs.GiverAssetID(base.id)));
			_iconImage.SetNativeSize();
			_solutionIconImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", ItemDefs.AssetID(solutionID)));
			_solutionIconImage.SetNativeSize();
			_nameLabel.text = QuestDefs.Name(base.id);
		}
		else
		{
			_iconImage.gameObject.SetActive(false);
			_solutionIconImage.gameObject.SetActive(false);
			_tickIconImage.gameObject.SetActive(false);
			_nameLabel.gameObject.SetActive(false);
		}
	}
}
