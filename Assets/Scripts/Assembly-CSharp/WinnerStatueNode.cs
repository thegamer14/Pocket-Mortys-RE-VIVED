using UnityEngine;

public class WinnerStatueNode : AdvancedNode
{
	public override void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		if (!Application.isPlaying || PlayerDefs.IsCouncilStoryComplete())
		{
			base.SetUp(data, initialDirection, applyRotation);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
