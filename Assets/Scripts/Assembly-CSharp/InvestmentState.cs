using System;
using System.Collections;
using UnityEngine;

public class InvestmentState : StateBase
{
	private InvestmentUI _investmentUI;

	private GenericPopupState _popupState;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Investment/{0}", "Investment");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Storage/RaM204m12.BBQBackground.01.08.44.10.Ver1.0.FUTZED");
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_investmentUI = _ui as InvestmentUI;
		_investmentUI.SetUp();
		RefreshServerTimestamp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_investmentUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_investmentUI.BeginEvent += UIBeginHandler;
		_investmentUI.ResultsEvent += UIResultsHandler;
		_investmentUI.BackEvent += UIBackHandler;
		_investmentUI.GetMoreCoinEvent += UIGetMoreCoinHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_investmentUI.BeginEvent -= UIBeginHandler;
		_investmentUI.ResultsEvent -= UIResultsHandler;
		_investmentUI.BackEvent -= UIBackHandler;
		_investmentUI.GetMoreCoinEvent -= UIGetMoreCoinHandler;
	}

	private void DisplayBeginInvestmentCheckPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.INVESTMENT_BEGIN_WAIT_POPUP, null, false, true);
		_popupState.TransitionInCompleteEvent += BeginInvestmentCheckAnimInCompleteHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void DisplayInvestmentBegunPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.ButtonClickedEvent += InvestmentBegunButtonClickedHandler;
		_popupState.SetDisplayValues(TextDefs.INVESTMENT_BEGUN_POPUP, null);
	}

	private void DisplayViewResultsServerCheckPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.INVESTMENT_RESULTS_WAIT_POPUP, null, false, true);
		_popupState.TransitionInCompleteEvent += ViewResultsServerCheckAnimInCompleteHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void RefreshServerTimestamp()
	{
		int num = UnityEngine.Random.Range(0, 999999999);
		StartCoroutine(TimeServer.GetServerTimeJSONURL(string.Format("http://www.adultswim.com/videos/api/v1/_server_time?a=pm_{0}", num), "utc_timestamp"));
	}

	private IEnumerator BeginInvestmentServerCheck()
	{
		int urlRandomizer = UnityEngine.Random.Range(0, 999999999);
		CoroutineWithData cd = new CoroutineWithData(this, TimeServer.GetServerTimeJSONURL(string.Format("http://www.adultswim.com/videos/api/v1/_server_time?a=pm_{0}", urlRandomizer), "utc_timestamp"));
		yield return cd.coroutine;
		double timestamp = Convert.ToDouble(cd.result);
		if (timestamp > -1.0)
		{
			InvestmentDefs.BeginInvestment(_investmentUI.selectedIndex, timestamp);
			App.saveData.Save(eSaveDataType.PLAYER);
			DisplayInvestmentBegunPopup();
			_investmentUI.InvestmentBegun();
		}
		else
		{
			_popupState.ButtonClickedEvent += CantConnectButtonClickedHandler;
			_popupState.SetDisplayValues(TextDefs.INVESTMENT_BEGIN_CANNOT_CONNECT_POPUP, null);
		}
	}

	private IEnumerator ViewResultsServerCheck()
	{
		int urlRandomizer = UnityEngine.Random.Range(0, 999999999);
		CoroutineWithData cd = new CoroutineWithData(this, TimeServer.GetServerTimeJSONURL(string.Format("http://www.adultswim.com/videos/api/v1/_server_time?a=pm_{0}", urlRandomizer), "utc_timestamp"));
		yield return cd.coroutine;
		double timestamp = Convert.ToDouble(cd.result);
		if (timestamp > -1.0)
		{
			if (InvestmentDefs.GetInvestmentState(_investmentUI.selectedIndex) == eInvestmentState.RESULT_AVAILABLE)
			{
				InvestmentDefs.CollectInvestment(_investmentUI.selectedIndex);
				_investmentUI.InvestmentCollected(_investmentUI.selectedIndex);
				App.saveData.Save(eSaveDataType.PLAYER);
				App.sound.PlaySFX("Audio/SFX/Coins");
				App.states.SetBack(new PopTransInfo());
			}
			else
			{
				_popupState.ButtonClickedEvent += NotCompleteButtonClickedHandler;
				_popupState.SetDisplayValues(TextDefs.INVESTMENT_NOT_COMPLETE_POPUP, null);
			}
		}
		else
		{
			_popupState.ButtonClickedEvent += CantConnectButtonClickedHandler;
			_popupState.SetDisplayValues(TextDefs.INVESTMENT_BEGIN_CANNOT_CONNECT_POPUP, null);
		}
	}

	private void UIBeginHandler(int investmentIndex = 0)
	{
		if (PlayerDefs.CoinCount < InvestmentDefs.GetInvestmentDeposit(investmentIndex))
		{
			_investmentUI.NotEnoughCoin();
		}
		else
		{
			DisplayBeginInvestmentCheckPopup();
		}
	}

	private void UIResultsHandler(int investmentIndex = 0)
	{
		DisplayViewResultsServerCheckPopup();
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIGetMoreCoinHandler()
	{
		App.states.Set<VideoRewardState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void BeginInvestmentCheckAnimInCompleteHandler(StateBase state)
	{
		_popupState.TransitionInCompleteEvent -= BeginInvestmentCheckAnimInCompleteHandler;
		StartCoroutine(BeginInvestmentServerCheck());
	}

	private void CantConnectButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= CantConnectButtonClickedHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void NotCompleteButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= NotCompleteButtonClickedHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void InvestmentBegunButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= InvestmentBegunButtonClickedHandler;
		App.states.SetBack(new PopTransInfo());
		_investmentUI.InvestmentBegun();
	}

	private void ViewResultsServerCheckAnimInCompleteHandler(StateBase state)
	{
		_popupState.TransitionInCompleteEvent -= ViewResultsServerCheckAnimInCompleteHandler;
		StartCoroutine(ViewResultsServerCheck());
	}
}
