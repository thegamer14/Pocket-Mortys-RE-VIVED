using UnityEngine;
using UnityEngine.UI;

public class MortyCombineCell : CellBase
{
	private Text _nameLabel;

	private Image _iconImage;

	private MortyElementImage _elementTypeImage;

	private MortyLevelBar _levelBar;

	private GameObject _newBar;

	private Text _newBarLabel;

	private GameObject _bootcampIconObj;

	[HideInInspector]
	public MortyData data;

	protected override void Awake()
	{
		base.Awake();
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
		_iconImage = base.transform.FindChild("Icon").GetComponent<Image>();
		_elementTypeImage = base.transform.FindChild("ElementType").GetComponent<MortyElementImage>();
		_levelBar = base.transform.FindChild("LevelBar").GetComponent<MortyLevelBar>();
		_newBar = base.transform.FindChild("NewBar").gameObject;
		_newBarLabel = _newBar.transform.FindChild("Label").GetComponent<Text>();
		_bootcampIconObj = base.transform.FindChild("BootcampIcon").gameObject;
	}

	public void SetUp(MortyData data, bool isNew)
	{
		this.data = data;
		base.SetUp((data == null) ? null : data.id);
		_newBarLabel.text = TextDefs.MORTY_STORAGE_CELL_NEW_BAR;
		_newBar.SetActive(isNew);
		_bootcampIconObj.SetActive(BootcampDefs.IsMortyOnBootcamp(data));
		SetUsed(data != null);
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		if (value)
		{
			_nameLabel.text = MortyDefs.Name(data.id);
			_elementTypeImage.Set(MortyDefs.ElementType(data.id), true);
			_levelBar.SetLevel(data.level);
			_iconImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MortyDefs.AssetID(data.id)));
			_iconImage.SetNativeSize();
		}
		else
		{
			_nameLabel.gameObject.SetActive(false);
			_iconImage.gameObject.SetActive(false);
			_elementTypeImage.SetActive(false);
			_levelBar.gameObject.SetActive(false);
			_newBar.SetActive(false);
		}
	}
}
