using UnityEngine;

public class MPNodeFollowModule : MonoBehaviour
{
	private MPNodeMoveModule _moveModule;

	public void SetUp()
	{
		_moveModule = GetComponent<MPNodeMoveModule>();
		_moveModule.isManualUpdate = true;
	}

	public void MoveToCoord(Coord newCoord)
	{
		if (base.enabled)
		{
			_moveModule.MoveToCoord(newCoord, true);
		}
	}

	public void SetMovementSpeedMultiplier(float value)
	{
		_moveModule.SetMovementSpeedMultiplier(value);
	}

	public void UpdateMovement()
	{
		if (_moveModule.isManualUpdate)
		{
			_moveModule.UpdateMovement();
		}
	}
}
