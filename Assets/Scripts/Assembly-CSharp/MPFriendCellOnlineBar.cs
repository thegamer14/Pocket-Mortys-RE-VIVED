using UnityEngine;
using UnityEngine.UI;

public class MPFriendCellOnlineBar : MonoBehaviour
{
	public Sprite onlineSprite;

	public Sprite offlineSprite;

	private Image _background;

	private Text _title;

	private void Awake()
	{
		_background = base.transform.FindChild("Background").GetComponent<Image>();
		_title = base.transform.FindChild("Title").GetComponent<Text>();
	}

	public void SetUp(bool isOnline)
	{
		if (isOnline)
		{
			_background.sprite = onlineSprite;
			_title.text = MPTextDefs.MP_FRIEND_CELL_ONLINE_TITLE;
		}
		else
		{
			_background.sprite = offlineSprite;
			_title.text = MPTextDefs.MP_FRIEND_CELL_OFFLINE_TITLE;
		}
	}
}
