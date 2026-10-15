using System.Collections.Generic;
using UnityEngine;

public class MPCraftRecipeState : StateBase
{
	private MPCraftRecipeUI _craftRecipeUI;

	private string _selectedRecipeID;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPCraftRecipe/{0}", "MPCraftRecipe");
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
		_craftRecipeUI = _ui as MPCraftRecipeUI;
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

	private void UICraftHandler(string recipeId)
	{
		App.net.player.CraftRecipe(recipeId, delegate(MPRewardData reward, eCraftRecipeError error)
		{
			if (error == eCraftRecipeError.NONE)
			{
				_selectedRecipeID = recipeId;
				App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.CRAFT, MPRecipeDefs.ResultID(_selectedRecipeID));
				MPSharedStateInfo.craftInfo.ClearAllSlots();
				MPSharedStateInfo.craftInfo.items = new Dictionary<string, int>(App.net.player.data.items);
				MPSharedStateInfo.rewardInfo = new MPRewardInfo(MPSharedStateInfo.craftInfo.origin, new MPRewardData[1] { reward }, "craft");
				App.states.Set<MPRewardState>(new FadeTransInfo
				{
					duration = 0.05f,
					color = Color.white
				}, new FadeTransInfo
				{
					duration = 0.2f,
					color = Color.white
				}, true);
			}
			else
			{
				_craftRecipeUI.TryCraftFailed(error);
			}
		});
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
