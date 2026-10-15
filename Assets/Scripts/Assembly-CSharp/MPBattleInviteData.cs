using UnityEngine;

public class MPBattleInviteData
{
	public MPUserData userData;

	public string id;

	public string type;

	public float ttl;

	public string token;

	public MPBattleInviteData(string id, string type, MPUserData userData, float ttl, string token)
	{
		this.id = id;
		this.type = type;
		this.userData = userData;
		this.ttl = ttl;
		this.token = token;
	}

	public void ManualUpdateTtl()
	{
		if (ttl > 0f)
		{
			ttl = Mathf.Max(0f, ttl - Time.deltaTime);
		}
	}
}
