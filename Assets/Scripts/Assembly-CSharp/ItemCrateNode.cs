using UnityEngine;

public class ItemCrateNode : AdvancedNode
{
	private const int _kRotationSpeed = 120;

	private Transform _meshTrans;

	private void Awake()
	{
		_meshTrans = base.transform.FindChild("Mesh");
		_sfxInteract = "Audio/SFX/PickupItem";
		_sfxInteractVolume = 1f;
	}

	private void Update()
	{
		_meshTrans.Rotate(0f, 120f * Time.deltaTime, 0f);
	}
}
