using UnityEngine;

public class GachaMachineNode : AdvancedNode
{
	private const int _kRotationSpeed = 60;

	private Transform _top;

	private void Awake()
	{
		_top = base.transform.FindChild("Mesh/Top");
	}

	private void Update()
	{
		_top.Rotate(0f, 60f * Time.deltaTime, 0f);
	}
}
