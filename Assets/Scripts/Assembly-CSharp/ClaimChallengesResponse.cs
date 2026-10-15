using System;
using System.Collections.Generic;

[Serializable]
public class ClaimChallengesResponse
{
	public bool challenge_reward;

	public Dictionary<string, object> challenge;

	public Dictionary<string, object> reward;
}
