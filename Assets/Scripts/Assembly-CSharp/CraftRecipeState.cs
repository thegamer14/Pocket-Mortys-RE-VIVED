using System.Collections.Generic;
using UnityEngine;

public class CraftRecipeState : StateBase
{
	private CraftRecipeUI _craftRecipeUI;

	private string _selectedRecipeID;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/CraftRecipe/{0}", "CraftRecipe");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_craftRecipeUI = _ui as CraftRecipeUI;
		_craftRecipeUI.SetUp(_selectedRecipeID);
		_selectedRecipeID = null;
	}

	protected override void Destroy()
	{
		base.Destroy();
		_craftRecipeUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_craftRecipeUI.BackEvent += UIBackHandler;
		_craftRecipeUI.CraftEvent += UICraftHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_craftRecipeUI.BackEvent -= UIBackHandler;
		_craftRecipeUI.CraftEvent -= UICraftHandler;
	}

	private void UICraftHandler(string recipeID)
	{
		string id = RecipeDefs.ResultID(recipeID);
		App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.CRAFT, id);
		List<string> combinationForRecipeID = RecipeDefs.GetCombinationForRecipeID(recipeID);
		foreach (string item in combinationForRecipeID)
		{
			ItemDefs.RemoveItem(SharedStatInfo.craftInfo.items, item);
		}
		List<RewardData> list = new List<RewardData>();
		list.Add(new RewardData(eRewardType.ITEM, id, 100f));
		List<RewardData> rewards = list;
		_selectedRecipeID = recipeID;
		SharedStatInfo.craftInfo.ClearSlots();
		SharedStatInfo.rewardInfo = new RewardInfo(rewards, "craft");
		App.states.Set<RewardState>(new FadeTransInfo
		{
			duration = 0.05f,
			color = Color.white
		}, new FadeTransInfo
		{
			duration = 0.2f,
			color = Color.white
		}, true);
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
