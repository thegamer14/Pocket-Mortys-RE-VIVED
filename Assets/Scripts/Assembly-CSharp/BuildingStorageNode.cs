using UnityEngine;

public class BuildingStorageNode : AdvancedNode
{
	private const int _kRotationSpeed = 30;

	private Transform _head;

	private void Awake()
	{
		_head = base.transform.FindChild("HeadMesh");
	}

	private void Update()
	{
		_head.transform.Rotate(0f, 30f * Time.deltaTime, 0f);
	}
}
