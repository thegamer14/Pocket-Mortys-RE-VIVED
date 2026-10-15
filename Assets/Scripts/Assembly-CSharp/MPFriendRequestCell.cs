using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPFriendRequestCell : CellBase
{
	public MPFriendData data;

	private Image _icon;

	private Text _name;

	private MPFriendCellLevelBar _levelBar;

	private Tween _backgroundTween;

	protected override void Awake()
	{
		base.Awake();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_name = base.transform.FindChild("Name").GetComponent<Text>();
		_levelBar = base.transform.FindChild("LevelBar").GetComponent<MPFriendCellLevelBar>();
	}

	public void SetUp(MPFriendData data)
	{
		this.data = data;
		base.SetUp((data == null) ? null : data.userId);
		StartTweeningBackground();
		SetUsed(data != null);
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		_icon.gameObject.SetActive(value);
		_name.gameObject.SetActive(value);
		_levelBar.gameObject.SetActive(value);
		if (value && data != null)
		{
			_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPPlayerAvatarDefs.AssetID(data.avatarId)));
			_icon.SetNativeSize();
			_name.text = data.username;
			_levelBar.SetUp(data.level);
			StartTweeningBackground();
		}
		else
		{
			_backgroundTween.Pause();
		}
	}

	private void StartTweeningBackground()
	{
		if (_backgroundTween == null)
		{
			_backgroundTween = _backgroundImage.transform.DOScaleX(1.05f, 0.5f);
			_backgroundTween.SetLoops(-1, LoopType.Yoyo);
		}
		else
		{
			_backgroundTween.Play();
		}
	}
}
