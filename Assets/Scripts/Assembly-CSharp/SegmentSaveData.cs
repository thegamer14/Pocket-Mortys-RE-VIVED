using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SegmentSaveData
{
	public eSegmentType type;

	public eSegmentJointType jointType;

	public string name;

	public float positionX;

	public float positionZ;

	public float rotation;

	public List<NodeSaveData> nonBasicNodeDatas;

	public SegmentSaveData(Segment segment)
	{
		type = segment.type;
		jointType = segment.jointType;
		name = segment.name;
		positionX = segment.transform.localPosition.x;
		positionZ = segment.transform.localPosition.z;
		rotation = segment.transform.eulerAngles.y;
		UpdateNonBasicNodes(segment);
	}

	public void UpdateNonBasicNodes(Segment segment)
	{
		nonBasicNodeDatas = new List<NodeSaveData>();
		UpdateNodes(nonBasicNodeDatas, segment.nonBasicNodes.ToArray());
	}

	private void UpdateNodes(List<NodeSaveData> List, Node[] nodes)
	{
		for (int i = 0; i < nodes.Length; i++)
		{
			if (!(nodes[i] == null))
			{
				if (nodes[i] is AdvancedNode)
				{
					AdvancedNode advancedNode = (AdvancedNode)nodes[i];
					Vector3 position = ((!(advancedNode is TrainerNode)) ? nodes[i].transform.localPosition : ((TrainerNode)advancedNode).setUpPosition);
					List.Add(new NodeSaveData(nodes[i].name, advancedNode.data, advancedNode.type, position, nodes[i].initialDirection, nodes[i].applyRotation));
				}
				else
				{
					List.Add(new NodeSaveData(nodes[i].name, null, eNodeType.NONE, nodes[i].transform.localPosition, nodes[i].initialDirection, nodes[i].applyRotation));
				}
			}
		}
	}
}
