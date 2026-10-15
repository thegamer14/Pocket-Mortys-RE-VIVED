using UnityEngine;

public class FadeTransInfo : TransInfo
{
	public float from;

	public float to = 1f;

	public Color color;

	public FadeTransInfo()
	{
		type = eTransition.FADE;
		color = Color.black;
	}
}
