using System.Collections.Generic;
using UnityEngine;

public class SocialDefs : MonoBehaviour
{
	private const string _bundleId = "com.turner.pocketmortys";

	public const string ACH_DEFEAT_TRAINERS_50 = "CgkIjtTv3ccYEAIQAQ";

	public const string ACH_DEFEAT_TRAINERS_100 = "CgkIjtTv3ccYEAIQAg";

	public const string ACH_DEFEAT_TRAINERS_200 = "CgkIjtTv3ccYEAIQAw";

	public const string ACH_EARN_BADGES_10 = "CgkIjtTv3ccYEAIQBA";

	public const string ACH_EARN_BADGES_20 = "CgkIjtTv3ccYEAIQBQ";

	public const string ACH_EARN_BADGES_30 = "CgkIjtTv3ccYEAIQBg";

	public const string ACH_DEFEAT_COUNCIL_1 = "CgkIjtTv3ccYEAIQBw";

	public const string ACH_DEFEAT_COUNCIL_2 = "CgkIjtTv3ccYEAIQCA";

	public const string ACH_DEFEAT_COUNCIL_3 = "CgkIjtTv3ccYEAIQCQ";

	public const string ACH_DEFEAT_COUNCIL_4 = "CgkIjtTv3ccYEAIQCg";

	public const string ACH_DEFEAT_COUNCIL_5 = "CgkIjtTv3ccYEAIQCw";

	public const string ACH_DEFEAT_COUNCIL_6 = "CgkIjtTv3ccYEAIQDA";

	public const string ACH_LVL_MORTY_TO_10 = "CgkIjtTv3ccYEAIQDQ";

	public const string ACH_LVL_MORTY_TO_20 = "CgkIjtTv3ccYEAIQDg";

	public const string ACH_LVL_MORTY_TO_30 = "CgkIjtTv3ccYEAIQDw";

	public const string ACH_CAPTURE_FIRST_MORTY = "CgkIjtTv3ccYEAIQEA";

	public const string ACH_COMBINE_MORTY = "CgkIjtTv3ccYEAIQEQ";

	private static Dictionary<string, float> _achievementTargets = new Dictionary<string, float>
	{
		{ "CgkIjtTv3ccYEAIQAQ", 50f },
		{ "CgkIjtTv3ccYEAIQAg", 100f },
		{ "CgkIjtTv3ccYEAIQAw", 200f },
		{ "CgkIjtTv3ccYEAIQBA", 10f },
		{ "CgkIjtTv3ccYEAIQBQ", 20f },
		{ "CgkIjtTv3ccYEAIQBg", 30f },
		{ "CgkIjtTv3ccYEAIQBw", 1f },
		{ "CgkIjtTv3ccYEAIQCA", 2f },
		{ "CgkIjtTv3ccYEAIQCQ", 3f },
		{ "CgkIjtTv3ccYEAIQCg", 4f },
		{ "CgkIjtTv3ccYEAIQCw", 5f },
		{ "CgkIjtTv3ccYEAIQDA", 6f },
		{ "CgkIjtTv3ccYEAIQDQ", 10f },
		{ "CgkIjtTv3ccYEAIQDg", 20f },
		{ "CgkIjtTv3ccYEAIQDw", 30f }
	};

	public static float GetAchievementTarget(string achievementID)
	{
		if (_achievementTargets.ContainsKey(achievementID))
		{
			return _achievementTargets[achievementID];
		}
		return 0f;
	}
}
