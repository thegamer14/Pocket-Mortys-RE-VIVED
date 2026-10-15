using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BPSSocialScoreInfo : ScriptableObject
{
	public int rank;

	public string formattedRank = "0";

	public string avatarURL;

	public Texture2D avatar;

	public int score;

	public string formattedScore = "0";

	public string playerId = string.Empty;

	public string displayName = string.Empty;

	public IUserProfile socialProfile;

	public IEnumerator loadAvatarFromURL(string url, GameObject callbackObj = null, string callbackMethod = null)
	{
		WWW www = new WWW(url);
		yield return www;
		avatar = www.texture;
		if (callbackObj != null && callbackMethod != null)
		{
			callbackObj.SendMessage(callbackMethod, avatar);
		}
	}
}
