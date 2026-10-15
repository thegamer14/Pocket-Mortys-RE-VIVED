using UnityEngine;

public class MPHealingCenterNode : MPNode
{
	private const float _kSignScrollSpeed = 0.2f;

	private Material _signMaterial;

	private Vector2 _offset;

	private void Awake()
	{
		_signMaterial = base.transform.FindChild("Meshes/MPHealingSign").GetComponent<MeshRenderer>().sharedMaterial;
	}

	private void Update()
	{
		_offset = new Vector2(MathExt.Wrap(_offset.x + 0.2f * Time.deltaTime, 0f, 1f), 0f);
		_signMaterial.mainTextureOffset = _offset;
	}
}
