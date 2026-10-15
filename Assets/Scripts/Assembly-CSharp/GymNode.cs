using System;
using UnityEngine;

public class GymNode : AdvancedNode
{
	public bool IsPortalOpen { get; private set; }

	public void OpenPortal(UnityEngine.Object resource, string id)
	{
		IsPortalOpen = true;
		GameObject gameObject = UnityEngine.Object.Instantiate(resource) as GameObject;
		PortalNode component = gameObject.GetComponent<PortalNode>();
		component.name = resource.name;
		component.transform.SetParent(base.transform, false);
		component.transform.localPosition = new Vector3(0f, 2.5f, 2f);
		component.SetUp(new NodeData(id), eNodeInitialDirection.FIXED_DOWN, true);
		StartCoroutine(component.Open());
	}

	public Vector3 GetGymLeaderPosition()
	{
		float z = -1f * Mathf.Cos(base.transform.localEulerAngles.y * ((float)Math.PI / 180f));
		float x = -1f * Mathf.Sin(base.transform.localEulerAngles.y * ((float)Math.PI / 180f));
		return new Vector3(x, 0f, z);
	}
}
