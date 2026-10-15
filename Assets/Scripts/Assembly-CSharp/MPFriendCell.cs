using UnityEngine;
using UnityEngine.UI;

public class MPFriendCell : CellBase
{
	public delegate void BtnDelegate(MPFriendCell cell);

	public MPFriendData data;

	private Image _icon;

	private Text _name;

	private MPFriendCellOnlineBar _onlineBar;

	private MPFriendCellLevelBar _levelBar;

	private MPFriendRatioCellBar _ratioBar;

	private MPFriendMortyRequestCellBar _mortyRequest;

	private Text _pendingTitle;

	private Button _removeBtn;

	public event BtnDelegate RemoveEvent;

	protected override void Awake()
	{
		base.Awake();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_name = base.transform.FindChild("Name").GetComponent<Text>();
		_onlineBar = base.transform.FindChild("OnlineBar").GetComponent<MPFriendCellOnlineBar>();
		_levelBar = base.transform.FindChild("LevelBar").GetComponent<MPFriendCellLevelBar>();
		_ratioBar = base.transform.FindChild("RatioBar").GetComponent<MPFriendRatioCellBar>();
		_mortyRequest = base.transform.FindChild("MortyRequest").GetComponent<MPFriendMortyRequestCellBar>();
		_pendingTitle = base.transform.FindChild("PendingTitle").GetComponent<Text>();
		_removeBtn = base.transform.FindChild("RemoveBtn").GetComponent<Button>();
	}

	public void SetUp(MPFriendData data)
	{
		this.data = data;
		base.SetUp((data == null) ? null : data.userId);
		_pendingTitle.text = MPTextDefs.MP_FRIEND_CELL_PENDING_DESCRIPTION;
		SetUsed(data != null);
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		_icon.gameObject.SetActive(value);
		_name.gameObject.SetActive(value);
		_onlineBar.gameObject.SetActive(value && !data.isPending);
		_levelBar.gameObject.SetActive(value);
		_ratioBar.gameObject.SetActive(value && !data.isPending);
		_mortyRequest.gameObject.SetActive(value && !data.isPending);
		_pendingTitle.gameObject.SetActive(value && data.isPending);
		_removeBtn.gameObject.SetActive(value);
		if (value && data != null)
		{
			_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPPlayerAvatarDefs.AssetID(data.avatarId)));
			_icon.SetNativeSize();
			_name.text = data.username;
			_onlineBar.SetUp(data.isOnline);
			_levelBar.SetUp(data.level);
			_ratioBar.SetUp(data.winCount, data.lossCount);
			_mortyRequest.SetUp(data.requestingMortyId);
		}
	}

	public void RemoveBtnPressed()
	{
		if (this.RemoveEvent != null)
		{
			this.RemoveEvent(this);
		}
	}
}
