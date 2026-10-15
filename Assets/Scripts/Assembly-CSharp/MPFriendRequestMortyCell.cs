using UnityEngine;
using UnityEngine.UI;

public class MPFriendRequestMortyCell : CellBase
{
	private Image _icon;

	private Text _title;

	private Text _description;

	private GameObject _countdownObj;

	private Text _countdownLabel;

	protected override void Awake()
	{
		base.Awake();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_title = base.transform.FindChild("Title").GetComponent<Text>();
		_description = base.transform.FindChild("Description").GetComponent<Text>();
		_countdownObj = base.transform.FindChild("CountdownBar").gameObject;
		_countdownLabel = _countdownObj.transform.FindChild("Label").GetComponent<Text>();
		base.retainSelection = false;
	}

	public void SetUp(string mortyId, float countdown, MPFriendData donatedUser)
	{
		base.SetUp(null);
		SetCountdown(countdown);
		if (string.IsNullOrEmpty(mortyId))
		{
			_title.text = MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_TITLE;
			_description.text = MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_DESCRIPTION;
			return;
		}
		_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPMortyDefs.AssetID(mortyId)));
		_icon.SetNativeSize();
		if (donatedUser == null)
		{
			if (countdown <= 0f)
			{
				_title.text = MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_NOTHING_TITLE;
				_description.text = string.Format(MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_NOTHING_DESCRIPTION, MPMortyDefs.Name(mortyId));
			}
			else
			{
				_title.text = string.Format(MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_REQUESTING_TITLE, MPMortyDefs.Name(mortyId));
				_description.text = MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_REQUESTING_DESCRIPTION;
			}
		}
		else if (countdown <= 0f)
		{
			_title.text = string.Format(MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_AGAIN_TITLE, MPMortyDefs.Name(mortyId));
			_description.text = string.Format(MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_AGAIN_DESCRIPTION, donatedUser.username);
		}
		else
		{
			_title.text = string.Format(MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_RECEIVED_TITLE, MPMortyDefs.Name(mortyId));
			_description.text = string.Format(MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_RECEIVED_DESCRIPTION, donatedUser.username);
		}
	}

	public void SetCountdown(float countdown)
	{
		_countdownObj.SetActive(countdown > 0f);
		if (countdown > 0f)
		{
			_countdownLabel.text = TextDefs.AbbreviateTimestamp(countdown);
		}
	}
}
