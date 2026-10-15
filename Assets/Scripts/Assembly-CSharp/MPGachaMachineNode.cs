using UnityEngine;

public class MPGachaMachineNode : MPNode
{
	private const int _kRotationSpeed = 60;

	private Transform _top;

	public override void SetUp()
	{
		base.SetUp();
		_top = base.transform.FindChild("Mesh/Top");
	}

	private void Update()
	{
		_top.Rotate(0f, 60f * Time.deltaTime, 0f);
	}
}
