using System.Collections;
using UnityEngine;

public class MortyStatsState : StateBase
{
	private MortyStatsUI _mortyStatsUI;

	private bool _isReleased;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MortyStats/{0}", "MortyStats");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		if (_isReleased)
		{
			StartCoroutine(DeactivateNextFrame());
		}
		_isReleased = false;
	}

	private IEnumerator DeactivateNextFrame()
	{
		yield return new WaitForEndOfFrame();
		SharedStatInfo.mortyStatsInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_mortyStatsUI = _ui as MortyStatsUI;
		_mortyStatsUI.SetUp(SharedStatInfo.mortyStatsInfo.selectedMortyData, SharedStatInfo.battleInfo != null);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_mortyStatsUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_mortyStatsUI.ReleaseEvent += UIReleaseHandler;
		_mortyStatsUI.AboutEvent += UIAboutHandler;
		_mortyStatsUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_mortyStatsUI.ReleaseEvent -= UIReleaseHandler;
		_mortyStatsUI.AboutEvent -= UIAboutHandler;
		_mortyStatsUI.BackEvent -= UIBackHandler;
	}

	private void UIReleaseHandler()
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		string text = string.Format(TextDefs.MORTY_STATS_RELEASE_DESCRIPTION, MortyDefs.Name(SharedStatInfo.mortyStatsInfo.selectedMortyData.id));
		genericPopupState.SetDisplayValues(text, new string[1] { TextDefs.MORTY_STATS_RELEASE_BTN });
		genericPopupState.ButtonClickedEvent += ReleaseConfirmButtonClickedHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIAboutHandler()
	{
		SharedStatInfo.mortydexEntryInfo = new MortydexEntryInfo(SharedStatInfo.mortyStatsInfo.selectedMortyData.id);
		App.states.Set<MortydexEntryState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		SharedStatInfo.mortyStatsInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void ReleaseConfirmButtonClickedHandler(int buttonIndex)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.ButtonClickedEvent -= ReleaseConfirmButtonClickedHandler;
		if (buttonIndex == 1)
		{
			string releaseFailMessage = null;
			if (!PlayerDefs.TryReleaseMorty(SharedStatInfo.mortyStatsInfo.selectedMortyData, out releaseFailMessage))
			{
				_mortyStatsUI.ReleaseFailed(releaseFailMessage);
				_isReleased = false;
			}
			else
			{
				if (SharedStatInfo.partyInfo != null)
				{
					SharedStatInfo.partyInfo.mortyDatas.Remove(SharedStatInfo.mortyStatsInfo.selectedMortyData);
				}
				App.saveData.Save(eSaveDataType.PLAYER);
				_isReleased = true;
			}
		}
		App.states.SetBack(new PopTransInfo());
	}
}
