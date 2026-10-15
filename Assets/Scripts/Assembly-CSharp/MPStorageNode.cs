using UnityEngine;

public class MPStorageNode : MPNode
{
	private const int _kRotationSpeed = 30;

	private Transform _head;

	public override void SetUp()
	{
		base.SetUp();
		_head = base.transform.FindChild("HeadMesh");
	}

	private void Update()
	{
		_head.transform.Rotate(0f, 30f * Time.deltaTime, 0f);
	}
}
