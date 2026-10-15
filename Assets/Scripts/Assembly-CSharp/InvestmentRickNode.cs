using DG.Tweening;
using UnityEngine;

public class InvestmentRickNode : AdvancedNode
{
	private GameObject _icon;

	public override void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		if (Application.isPlaying && PlayerDefs.BadgeCount < 2)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		_icon = base.transform.FindChild("Icons/Investment").gameObject;
		base.SetUp(data, initialDirection, applyRotation);
		if (Application.isPlaying)
		{
			StartTweenIcon();
		}
	}

	public void StartTweenIcon()
	{
		_icon.transform.DOLocalMoveY(0.2f, 0.7f).SetRelative().SetLoops(-1, LoopType.Yoyo);
	}
}
