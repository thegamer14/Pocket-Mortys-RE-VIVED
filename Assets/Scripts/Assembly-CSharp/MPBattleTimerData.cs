using UnityEngine;

public class MPBattleTimerData
{
	public float time { get; private set; }

	public float timeTotal { get; private set; }

	public MPBattleTimerData(float time)
	{
		this.time = time;
		timeTotal = time;
	}

	public bool ManualUpdate()
	{
		if (time > 0f)
		{
			time = Mathf.Max(0f, time - Time.deltaTime);
			if (time <= 0f)
			{
				return false;
			}
		}
		return true;
	}
}
