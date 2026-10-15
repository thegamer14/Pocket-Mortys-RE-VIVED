using UnityEngine;

public class PlaceholderNode : MonoBehaviour
{
	public NodeData data;

	public ePlaceholderType type;

	public eNodeInitialDirection initialDirection;

	public bool applyRotation;

	public void SetUpFromNode(Node dumbNode)
	{
		base.transform.SetParent(dumbNode.transform.parent);
		base.transform.localPosition = dumbNode.transform.localPosition;
		initialDirection = dumbNode.initialDirection;
		applyRotation = dumbNode.applyRotation;
		if (dumbNode is AdvancedNode)
		{
			AdvancedNode advancedNode = (AdvancedNode)dumbNode;
			type = GameDefs.GetPlaceholderTypeForNodeType(advancedNode.type);
			data = advancedNode.data;
		}
	}
}
