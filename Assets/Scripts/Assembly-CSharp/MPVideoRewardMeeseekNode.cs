using DG.Tweening;
using UnityEngine;

public class MPVideoRewardMeeseekNode : MPClientNode
{
	protected MPNodeCharacterMoveModule _characterMoveModule;

	private GameObject _icon;

	public override void SetUp()
	{
		if (App.net.room.videoRewardMeeseeksData == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		base.SetUp();
		_characterMoveModule = SetUpCharacterMoveModule(GetAssetId());
		_icon = base.transform.FindChild("Icon").gameObject;
		StartTweenIcon();
	}

	public void SetUpWithId(string id, eDirection direction)
	{
		base.id = id;
		initialDirection = direction;
		SetUp();
	}

	private void OnEnable()
	{
		if (App.net.room.videoRewardMeeseeksData == null)
		{
			Object.Destroy(base.gameObject);
		}
		else if (_characterMoveModule != null)
		{
			_characterMoveModule.SetDirection(eDirection.DOWN);
		}
	}

	protected override string GetAssetId()
	{
		return "NPCVideoRewardMeeseek";
	}

	public void StartTweenIcon()
	{
		_icon.transform.DOLocalMoveY(0.2f, 0.7f).SetRelative().SetLoops(-1, LoopType.Yoyo);
	}

	public override void InteractionStarted(MPNode otherNode, eDirection direction)
	{
		base.InteractionStarted(otherNode, direction);
		_characterMoveModule.SetDirection(direction);
	}
}
