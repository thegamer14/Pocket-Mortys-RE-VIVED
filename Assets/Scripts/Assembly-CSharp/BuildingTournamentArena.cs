using UnityEngine;

public class BuildingTournamentArena : AdvancedNode
{
	public Object NPCResource;

	public override void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		base.SetUp(data, initialDirection, applyRotation);
		if (Application.isPlaying)
		{
			if (PlayerDefs.DefeatedCouncilMembers < 2)
			{
				AddNPCNode(new NodeData("NPCTournExteriorRickLocked"), eNodeInitialDirection.DOWN, new Vector3(-1f, 0f, -7f));
				AddNPCNode(new NodeData("NPCTournExteriorRickLocked"), eNodeInitialDirection.DOWN, new Vector3(0f, 0f, -7f));
				AddNPCNode(new NodeData("NPCTournExteriorRickLocked"), eNodeInitialDirection.DOWN, new Vector3(1f, 0f, -7f));
			}
			else
			{
				AddNPCNode(new NodeData("NPCTournExteriorRick1"), eNodeInitialDirection.DOWN, new Vector3(-2f, 0f, -8f));
				AddNPCNode(new NodeData("NPCTournExteriorRick2"), eNodeInitialDirection.DOWN, new Vector3(2f, 0f, -8f));
			}
		}
	}

	private void AddNPCNode(NodeData data, eNodeInitialDirection initialDirection, Vector3 position)
	{
		GameObject gameObject = Object.Instantiate(NPCResource) as GameObject;
		Node component = gameObject.GetComponent<Node>();
		component.name = NPCResource.name;
		component.transform.SetParent(base.transform, false);
		component.transform.localPosition = position;
		component.SetUp(data, initialDirection, base.applyRotation);
	}
}
