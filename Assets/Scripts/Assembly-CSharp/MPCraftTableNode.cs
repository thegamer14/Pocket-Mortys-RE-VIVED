using DG.Tweening;
using UnityEngine;

public class MPCraftTableNode : MPNode
{
	private Transform _top;

	public override void SetUp()
	{
		base.SetUp();
		_top = base.transform.FindChild("Mesh/Top");
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0.6f);
		sequence.Append(_top.DOLocalMoveY(0.2f, 1f).SetRelative().SetEase(Ease.InOutSine));
		sequence.AppendInterval(0.6f);
		sequence.Append(_top.DOLocalMoveY(-0.2f, 1f).SetRelative().SetEase(Ease.InOutSine));
		sequence.SetLoops(-1, LoopType.Restart);
	}
}
