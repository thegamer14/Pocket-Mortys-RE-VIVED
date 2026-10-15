using System.Collections.Generic;

public class RewardInfo
{
	public List<RewardData> rewards;

	public Dictionary<RewardData, string> rewardsOutcomeText;

	public RewardInfo(List<RewardData> rewards, string analyticID, Dictionary<string, int> items = null)
	{
		this.rewards = rewards;
		rewardsOutcomeText = new Dictionary<RewardData, string>();
		GiveRewards(analyticID, items);
	}

	private void GiveRewards(string analyticID, Dictionary<string, int> items = null)
	{
		rewardsOutcomeText.Clear();
		for (int i = 0; i < rewards.Count; i++)
		{
			string resultText = null;
			PlayerDefs.GiveReward(rewards[i], analyticID, out resultText, items);
			rewardsOutcomeText.Add(rewards[i], resultText);
		}
		StoryState storyState = App.states.Get<StoryState>();
		if (!storyState.IsActive() || storyState.storyIDToLoad != "GarageIntro")
		{
			App.saveData.Save(eSaveDataType.PLAYER | eSaveDataType.TOURNAMENT);
		}
	}

	public string GetRewardOutcomeText(RewardData data)
	{
		if (rewardsOutcomeText.ContainsKey(data))
		{
			return rewardsOutcomeText[data];
		}
		return null;
	}

	public void RemoveReward(int index)
	{
		rewardsOutcomeText.Remove(rewards[index]);
		rewards.RemoveAt(index);
	}
}
