using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftState : StateBase
{
	private CraftUI _craftUI;

	private int indexWaiting;

	private IEnumerator resultsEnumer;

	private string _recipeID;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Craft/{0}", "Craft");
		_useBackgroundFade = true;
		_backgroundFadeColor = new Color(0f, 0f, 0f, 1f);
		_backgroundFadeDuration = 0f;
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (SharedStatInfo.craftInfo == null)
		{
			SharedStatInfo.craftInfo = new CraftInfo(PlayerDefs.CurrentItems());
		}
		indexWaiting = -1;
		App.sound.PlaySoundtrack("Audio/Soundtrack/Store/RaM202m01.HumanMuzak.01005513.Ver1.0.Clean", 0.8f);
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	public override void Cache(TransInfo transOutInfo)
	{
		base.Cache(transOutInfo);
		_craftUI.TweenToCache();
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		if (SharedStatInfo.bagInfo != null)
		{
			AddItemToSlot(SharedStatInfo.bagInfo.selectedID, indexWaiting);
			RefreshResultSlot(0.3f);
		}
		SharedStatInfo.bagInfo = null;
		indexWaiting = -1;
		_craftUI.TweenFromCache();
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0.5f);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_craftUI = _ui as CraftUI;
		_craftUI.SetUp();
		if (SharedStatInfo.craftInfo == null)
		{
			return;
		}
		for (int i = 0; i < SharedStatInfo.craftInfo.slotIDs.Length; i++)
		{
			if (!string.IsNullOrEmpty(SharedStatInfo.craftInfo.slotIDs[i]))
			{
				AddItemToSlot(SharedStatInfo.craftInfo.slotIDs[i], i);
			}
		}
		RefreshResultSlot(0.3f);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_craftUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_craftUI.BackEvent += UIBackHandler;
		_craftUI.TweetEvent += UITweetHandler;
		_craftUI.RecipesEvent += UIRecipesHandler;
		_craftUI.AddEvent += UIAddHandler;
		_craftUI.RemoveEvent += UIRemoveHandler;
		_craftUI.CraftEvent += UICraftHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_craftUI.BackEvent -= UIBackHandler;
		_craftUI.TweetEvent -= UITweetHandler;
		_craftUI.RecipesEvent -= UIRecipesHandler;
		_craftUI.AddEvent -= UIAddHandler;
		_craftUI.RemoveEvent -= UIRemoveHandler;
		_craftUI.CraftEvent -= UICraftHandler;
	}

	private void AddItemToSlot(string selectedID, int slotIndex)
	{
		ItemDefs.RemoveItem(SharedStatInfo.craftInfo.items, selectedID);
		SharedStatInfo.craftInfo.slotIDs[slotIndex] = selectedID;
		StartCoroutine(_craftUI.TweenSlotFilled(slotIndex, selectedID));
	}

	private void RefreshResultSlot(float imageDelay)
	{
		if (resultsEnumer != null)
		{
			StopCoroutine(resultsEnumer);
			resultsEnumer = null;
		}
		int slotsFilled = SharedStatInfo.craftInfo.slotIDs.Where((string e) => !string.IsNullOrEmpty(e)).Count();
		_recipeID = RecipeDefs.GetIDForCombination(SharedStatInfo.craftInfo.slotIDs);
		resultsEnumer = _craftUI.RefreshResultsSlot(_recipeID, slotsFilled, imageDelay);
		StartCoroutine(resultsEnumer);
		PlayerDefs.SetCraftRecipeLearned(_recipeID);
	}

	private void RestoreLoadedItemsToPlayerDefs(bool removeFromCraftInfo)
	{
		for (int i = 0; i < SharedStatInfo.craftInfo.slotIDs.Length; i++)
		{
			if (SharedStatInfo.craftInfo.slotIDs[i] != null)
			{
				int resultAmount;
				string resultText;
				ItemDefs.ObtainItem(SharedStatInfo.craftInfo.items, SharedStatInfo.craftInfo.slotIDs[i], 1, out resultAmount, out resultText);
			}
			if (removeFromCraftInfo)
			{
				SharedStatInfo.craftInfo.slotIDs[i] = null;
			}
		}
	}

	private void UIBackHandler()
	{
		RestoreLoadedItemsToPlayerDefs(true);
		SharedStatInfo.craftInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UITweetHandler()
	{
		if (!string.IsNullOrEmpty(_recipeID))
		{
			App.social.Share(string.Format(TextDefs.SOCIAL_TWEET_CRAFT, ItemDefs.Name(RecipeDefs.ResultID(_recipeID)), "http://hyperurl.co/zrn5s4"), true);
		}
	}

	private void UIRecipesHandler()
	{
		RestoreLoadedItemsToPlayerDefs(false);
		App.states.Set<CraftRecipeState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIAddHandler(int index)
	{
		indexWaiting = index;
		SharedStatInfo.bagInfo = new BagInfo(PlayerDefs.CurrentParty(), SharedStatInfo.craftInfo.items, eBagFromState.CRAFT, null);
		App.states.Set<BagState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIRemoveHandler(int index)
	{
		int resultAmount;
		string resultText;
		ItemDefs.ObtainItem(SharedStatInfo.craftInfo.items, SharedStatInfo.craftInfo.slotIDs[index], 1, out resultAmount, out resultText);
		SharedStatInfo.craftInfo.slotIDs[index] = null;
		StartCoroutine(_craftUI.TweenSlotEmpty(index));
		RefreshResultSlot(0f);
	}

	private void UICraftHandler(string recipeID)
	{
		string id = RecipeDefs.ResultID(recipeID);
		App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.CRAFT, id);
		List<RewardData> list = new List<RewardData>();
		list.Add(new RewardData(eRewardType.ITEM, id, 100f));
		List<RewardData> rewards = list;
		SharedStatInfo.craftInfo = null;
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
}
