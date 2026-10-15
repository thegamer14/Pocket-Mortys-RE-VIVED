using UnityEngine;

public class PopTransInfo : TransInfo
{
	public Vector3 from = Vector3.zero;

	public Vector3 to = Vector3.one;

	public PopTransInfo()
	{
		type = eTransition.POP;
	}
}
