using System.Collections.Generic;
using UnityEngine;

public class MPCraftState : StateBase
{
	private MPCraftUI _craftUI;

	private MPRewardData _reward;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPCraft/{0}", "MPCraft");
		_useBackgroundFade = true;
		_backgroundFadeColor = new Color(0f, 0f, 0f, 1f);
		_backgroundFadeDuration = 0f;
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
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
		_craftUI.TweenFromCache(MPSharedStateInfo.craftInfo.slotIds, MPSharedStateInfo.craftInfo.SlotsFilled(), MPSharedStateInfo.craftInfo.slotSelectedIndex, MPSharedStateInfo.craftInfo.recipeId);
		MPSharedStateInfo.craftInfo.slotSelectedIndex = -1;
		App.net.player.LearnRecipe(MPSharedStateInfo.craftInfo.recipeId);
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
		_craftUI = _ui as MPCraftUI;
		_craftUI.SetUp();
		for (int i = 0; i < MPSharedStateInfo.craftInfo.slotIds.Length; i++)
		{
			if (!string.IsNullOrEmpty(MPSharedStateInfo.craftInfo.slotIds[i]))
			{
				_craftUI.SetSlotFilled(i, MPSharedStateInfo.craftInfo.slotIds[i]);
			}
		}
		_craftUI.RefreshResultsSlot(MPSharedStateInfo.craftInfo.recipeId, MPSharedStateInfo.craftInfo.SlotsFilled(), 0f);
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
		_craftUI.TryCraftEvent += UITryCraftHandler;
		_craftUI.CraftFinishedEvent += UICraftFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_craftUI.BackEvent -= UIBackHandler;
		_craftUI.TweetEvent -= UITweetHandler;
		_craftUI.RecipesEvent -= UIRecipesHandler;
		_craftUI.AddEvent -= UIAddHandler;
		_craftUI.RemoveEvent -= UIRemoveHandler;
		_craftUI.TryCraftEvent -= UITryCraftHandler;
		_craftUI.CraftFinishedEvent -= UICraftFinishedHandler;
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.craftInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UITweetHandler()
	{
		if (!string.IsNullOrEmpty(MPSharedStateInfo.craftInfo.recipeId))
		{
			App.social.Share(string.Format(TextDefs.SOCIAL_TWEET_CRAFT, MPItemDefs.Name(MPRecipeDefs.ResultID(MPSharedStateInfo.craftInfo.recipeId)), "http://hyperurl.co/zrn5s4"), true);
		}
	}

	private void UIRecipesHandler()
	{
		App.states.Set<MPCraftRecipeState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIAddHandler(int index)
	{
		MPSharedStateInfo.craftInfo.slotSelectedIndex = index;
		MPSharedStateInfo.bagInfo = new MPBagInfo(MPSharedStateInfo.craftInfo.origin | eMPStateOrigin.CRAFT, MPSharedStateInfo.craftInfo.items);
		App.states.Set<MPBagState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIRemoveHandler(int index)
	{
		MPSharedStateInfo.craftInfo.ClearSlot(index);
		StartCoroutine(_craftUI.TweenSlotEmpty(index));
		_craftUI.RefreshResultsSlot(MPSharedStateInfo.craftInfo.recipeId, MPSharedStateInfo.craftInfo.SlotsFilled(), 0f);
		App.net.player.LearnRecipe(MPSharedStateInfo.craftInfo.recipeId);
	}

	private void UITryCraftHandler(string recipeId)
	{
		App.net.player.CraftRecipe(recipeId, delegate(MPRewardData reward, eCraftRecipeError error)
		{
			if (error == eCraftRecipeError.NONE)
			{
				_reward = reward;
				App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.CRAFT, MPRecipeDefs.ResultID(recipeId));
				MPSharedStateInfo.craftInfo.ClearAllSlots();
				MPSharedStateInfo.craftInfo.items = new Dictionary<string, int>(App.net.player.data.items);
				StartCoroutine(_craftUI.TryCraftSuccess());
			}
			else
			{
				_craftUI.TryCraftFailed(error);
			}
		});
	}

	private void UICraftFinishedHandler(string recipeID)
	{
		MPSharedStateInfo.rewardInfo = new MPRewardInfo(MPSharedStateInfo.craftInfo.origin | eMPStateOrigin.CRAFT, new MPRewardData[1] { _reward }, "craft");
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
}
