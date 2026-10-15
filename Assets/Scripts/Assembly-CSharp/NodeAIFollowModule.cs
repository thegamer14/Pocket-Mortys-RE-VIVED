using UnityEngine;

public class NodeAIFollowModule : MonoBehaviour
{
	private NodeMoveModule _moveModule;

	public void SetUp()
	{
		_moveModule = GetComponent<NodeMoveModule>();
	}

	public void MoveToCoord(Coord newCoord)
	{
		if (base.enabled)
		{
			_moveModule.MoveToCoord(newCoord, true);
		}
	}
}
