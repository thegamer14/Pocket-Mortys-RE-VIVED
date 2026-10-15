using System;
using UnityEngine;

[Serializable]
public class NodeSaveData
{
	public string name;

	public NodeData data;

	public eNodeType type;

	public SerializableVector3 position;

	public eNodeInitialDirection initialDirection;

	public bool applyRotation;

	public NodeSaveData(string name, NodeData data, eNodeType type, Vector3 position, eNodeInitialDirection direction, bool applyRotation)
	{
		this.name = name;
		this.data = data;
		this.type = type;
		this.position = position;
		initialDirection = direction;
		this.applyRotation = applyRotation;
	}
}
