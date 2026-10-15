using DG.Tweening;
using UnityEngine;

public class BuildingChambersNode : AdvancedNode
{
	private const float _kDoorTweenPosition = 1.5f;

	private const float _kDoorTweenDuration = 0.8f;

	private Transform _doorLeftTrans;

	private Transform _doorRightTrans;

	private void Awake()
	{
		_doorLeftTrans = base.transform.FindChild("Meshs/DoorLeft");
		_doorRightTrans = base.transform.FindChild("Meshs/DoorRight");
	}

	public void OpenDoors()
	{
		_doorLeftTrans.DOLocalMoveX(-1.5f, 0.8f);
		_doorRightTrans.DOLocalMoveX(1.5f, 0.8f);
	}

	public void CloseDoors()
	{
		_doorLeftTrans.DOLocalMoveX(0f, 0.8f);
		_doorRightTrans.DOLocalMoveX(0f, 0.8f);
	}

	public override void InteractionStarted(eDirection direction, bool isTrigger)
	{
		base.InteractionStarted(direction, isTrigger);
		OpenDoors();
	}

	public override void InteractionFinished()
	{
		base.InteractionFinished();
		CloseDoors();
	}
}
