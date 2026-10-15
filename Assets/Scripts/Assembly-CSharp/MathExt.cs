using UnityEngine;

public static class MathExt
{
	public static float Wrap(float value, float minValue, float maxValue)
	{
		return value - (maxValue - minValue) * Mathf.Floor(value / (maxValue - minValue));
	}
}
