using UnityEngine;
using UnityEngine.UI;

public class MPRegisterMortyCell : CellBase
{
	private Image _icon;

	private Text _name;

	protected MortyElementImage _elementTypeImage;

	protected override void Awake()
	{
		base.Awake();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_name = base.transform.FindChild("Name").GetComponent<Text>();
		_elementTypeImage = base.transform.FindChild("ElementType").GetComponent<MortyElementImage>();
	}

	public override void SetUp(string id)
	{
		base.SetUp(id);
		_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPMortyDefs.AssetID(id)));
		_icon.SetNativeSize();
		_name.text = MPMortyDefs.Name(id);
		_elementTypeImage.Set(MPMortyDefs.ElementType(id), true);
	}
}
