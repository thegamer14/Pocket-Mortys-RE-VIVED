using DG.Tweening;
using UnityEngine;

public class QuestGiverNode : AdvancedNode
{
	private GameObject _questIcon;

	public QuestGiverData questData
	{
		get
		{
			return (QuestGiverData)_data;
		}
		private set
		{
			_data = value;
		}
	}

	public override void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		_questIcon = base.transform.FindChild("Icons/Quest").gameObject;
		base.SetUp(data, initialDirection, applyRotation);
		if (Application.isPlaying)
		{
			RefreshIcon();
		}
	}

	protected override void RefreshAssetID()
	{
		if (!string.IsNullOrEmpty(data.id))
		{
			_assetID = QuestDefs.GiverAssetID(data.id);
		}
	}

	public void RefreshIcon()
	{
		bool flag = questData.progress == eQuestProgress.WAITING;
		_questIcon.gameObject.SetActive(flag);
		if (flag)
		{
			StartTweenIcon();
		}
		else
		{
			_questIcon.transform.DOKill();
		}
	}

	public void StartTweenIcon()
	{
		_questIcon.transform.DOLocalMoveY(0.2f, 0.7f).SetRelative().SetLoops(-1, LoopType.Yoyo);
	}
}
