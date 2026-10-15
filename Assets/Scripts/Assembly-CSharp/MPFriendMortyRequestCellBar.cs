using UnityEngine;
using UnityEngine.UI;

public class MPFriendMortyRequestCellBar : MonoBehaviour
{
	private Image _icon;

	private Text _title;

	private Text _name;

	private Text _count;

	private void Awake()
	{
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_title = base.transform.FindChild("Title").GetComponent<Text>();
		_name = base.transform.FindChild("Name").GetComponent<Text>();
		_count = base.transform.FindChild("CountBar/Label").GetComponent<Text>();
	}

	public void SetUp(string mortyId)
	{
		if (!string.IsNullOrEmpty(mortyId))
		{
			base.gameObject.SetActive(true);
			_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPMortyDefs.AssetID(mortyId)));
			_icon.SetNativeSize();
			_title.text = MPTextDefs.MP_FRIEND_MORTY_REQUEST_CELL_TITLE;
			_name.text = MPMortyDefs.Name(mortyId);
			_count.text = App.net.player.data.GetAllMortyDatasOfId(mortyId).Count.ToString();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
