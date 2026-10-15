using UnityEngine;

public class TimeIndependant
{
	private static float prevRealTime;

	private static float thisRealTime;

	public static float deltaTime
	{
		get
		{
			return Time.realtimeSinceStartup - prevRealTime;
		}
	}

	public static void Update()
	{
		prevRealTime = thisRealTime;
		thisRealTime = Time.realtimeSinceStartup;
	}
}
