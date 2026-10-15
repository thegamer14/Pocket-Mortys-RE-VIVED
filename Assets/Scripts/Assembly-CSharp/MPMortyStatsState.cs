using UnityEngine;

public class MPMortyStatsState : StateBase
{
	private MPMortyStatsUI _mortyStatsUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPMortyStats/{0}", "MPMortyStats");
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
		_mortyStatsUI = _ui as MPMortyStatsUI;
		_mortyStatsUI.SetUp(MPSharedStateInfo.mortyStatsInfo.mortyData, MPSharedStateInfo.mortyStatsInfo.allowRelease);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_mortyStatsUI = null;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (MPSharedStateInfo.mortyStatsInfo.IfOrigin(eMPStateOrigin.BATTLE) && App.states.Get<MPBattleState>().waitingToPlayTurnActDatas)
		{
			NetBattleComeBackHandler();
		}
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_mortyStatsUI.ReleaseEvent += UIReleaseHandler;
		_mortyStatsUI.AboutEvent += UIAboutHandler;
		_mortyStatsUI.BackEvent += UIBackHandler;
		App.net.battle.TurnResultEvent += NetBattleComeBackHandler;
		App.net.battle.AbandonedEvent += NetBattleComeBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_mortyStatsUI.ReleaseEvent -= UIReleaseHandler;
		_mortyStatsUI.AboutEvent -= UIAboutHandler;
		_mortyStatsUI.BackEvent -= UIBackHandler;
		App.net.battle.TurnResultEvent -= NetBattleComeBackHandler;
		App.net.battle.AbandonedEvent -= NetBattleComeBackHandler;
	}

	private void UIReleaseHandler()
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		string text = string.Format(TextDefs.MORTY_STATS_RELEASE_DESCRIPTION, MPMortyDefs.Name(MPSharedStateInfo.mortyStatsInfo.mortyData.id));
		genericPopupState.SetDisplayValues(text, new string[1] { TextDefs.MORTY_STATS_RELEASE_BTN });
		genericPopupState.ButtonClickedEvent += ReleaseConfirmButtonClickedHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIAboutHandler()
	{
		MPSharedStateInfo.mortydexEntryInfo = new MPMortydexEntryInfo(MPSharedStateInfo.mortyStatsInfo.origin | eMPStateOrigin.MORTY_STATS, MPSharedStateInfo.mortyStatsInfo.mortyData.id);
		App.states.Set<MPMortydexEntryState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.mortyStatsInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void ReleaseConfirmButtonClickedHandler(int buttonIndex)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.ButtonClickedEvent -= ReleaseConfirmButtonClickedHandler;
		if (buttonIndex == 1)
		{
			genericPopupState.SetDisplayValues(MPTextDefs.MP_MORTY_STATS_RELEASING_POPUP_TITLE, null, false, true);
			App.net.player.ReleaseMorty(MPSharedStateInfo.mortyStatsInfo.mortyData.netId, delegate(eReleaseMortyError error)
			{
				if (error == eReleaseMortyError.NONE)
				{
					StartCoroutine(App.states.SetBackFromPopup(new FadeTransInfo(), new FadeTransInfo()));
				}
				else
				{
					App.states.SetBack(new PopTransInfo());
					_mortyStatsUI.DisplayReleaseError(error);
				}
			});
		}
		else
		{
			App.states.SetBack(new PopTransInfo());
		}
	}

	private void NetBattleComeBackHandler()
	{
		if (IsActiveAndNotTransitioning())
		{
			App.states.RemoveLastBackstack();
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}
}
