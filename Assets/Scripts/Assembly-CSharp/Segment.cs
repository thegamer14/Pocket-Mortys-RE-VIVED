using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
	[HideInInspector]
	public int editorSegmentNamesIndex;

	[HideInInspector]
	public int editorNodeNamesIndex;

	public eSegmentType type;

	public eSegmentJointType jointType;

	public List<Node> nonBasicNodes;

	public Transform basicNodesHolder { get; private set; }

	public Transform nonBasicNodesHolder { get; private set; }

	public void SetUp(Material material)
	{
		SetMaterial(material);
		basicNodesHolder = base.transform.FindChild("BasicNodes");
		nonBasicNodesHolder = base.transform.FindChild("NonBasicNodes");
	}

	public void SetMaterial(Material material)
	{
		if (material != null)
		{
			base.transform.Find("Mesh").GetComponent<MeshRenderer>().material = material;
		}
	}

	public Node[] GetAllNodes()
	{
		return base.transform.GetComponentsInChildren<Node>();
	}

	public Node[] GetAllBasicNodes()
	{
		return (!(basicNodesHolder != null)) ? new Node[0] : basicNodesHolder.GetComponentsInChildren<Node>(true);
	}

	public Node[] GetAllNonBasicNodes()
	{
		return (!(nonBasicNodesHolder != null)) ? new Node[0] : nonBasicNodesHolder.GetComponentsInChildren<Node>(true);
	}

	public PlaceholderNode[] GetAllNodePlaceholders()
	{
		return base.transform.GetComponentsInChildren<PlaceholderNode>();
	}

	public PlaceholderNode[] GetAllBasicNodePlaceholders()
	{
		return (!(basicNodesHolder != null)) ? new PlaceholderNode[0] : basicNodesHolder.GetComponentsInChildren<PlaceholderNode>();
	}

	public PlaceholderNode[] GetAllNonBasicNodePlaceholders()
	{
		return (!(nonBasicNodesHolder != null)) ? new PlaceholderNode[0] : nonBasicNodesHolder.GetComponentsInChildren<PlaceholderNode>();
	}
}
