using UnityEngine;

public class TournamentDirtyRickState : StateBase
{
	private TournamentDirtyRickUI _dirtyRickUI;

	private int _currentCost;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/TournamentDirtyRick/{0}", "TournamentDirtyRick");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Storage/RaM204m12.BBQBackground.01.08.44.10.Ver1.0.FUTZED", 0.6f);
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0f);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_dirtyRickUI = _ui as TournamentDirtyRickUI;
		_currentCost = TournamentDefs.GetItemStashCurrentCost();
		_dirtyRickUI.SetUp(_currentCost);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_dirtyRickUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_dirtyRickUI.BuyEvent += UIBuyHandler;
		_dirtyRickUI.BuyCompleteEvent += UIBuyCompleteHandler;
		_dirtyRickUI.BackEvent += UIBackHandler;
		_dirtyRickUI.GetCoinsEvent += UIGetCoinsHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_dirtyRickUI.BuyEvent -= UIBuyHandler;
		_dirtyRickUI.BuyCompleteEvent -= UIBuyCompleteHandler;
		_dirtyRickUI.BackEvent -= UIBackHandler;
		_dirtyRickUI.GetCoinsEvent -= UIGetCoinsHandler;
	}

	private void UIBuyHandler()
	{
		bool flag = PlayerDefs.CoinCount >= _currentCost;
		if (flag)
		{
			PlayerDefs.RemoveCoin(_currentCost);
			TournamentDefs.SetItemStashActive(true);
			App.saveData.Save(eSaveDataType.PLAYER | eSaveDataType.TOURNAMENT);
			App.upsight.CustomAnalyticEvent(eSubType1.TOURNAMENTS, eSubType2.RICK_OR_TREAT, "rick_or_treat_purchased");
		}
		StartCoroutine(_dirtyRickUI.Buy(flag));
	}

	private void UIBuyCompleteHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIGetCoinsHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.STORE_MENU, "get_schmeckles");
		App.states.Set<VideoRewardState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}
}
