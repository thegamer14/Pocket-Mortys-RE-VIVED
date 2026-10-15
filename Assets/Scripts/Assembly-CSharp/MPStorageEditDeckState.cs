using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MPStorageEditDeckState : StateBase
{
	private MPStorageEditDeckUI _storagePartyEditUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPStorageEditDeck/{0}", "MPStorageEditDeck");
		base.showLoadingScreen = true;
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
		_storagePartyEditUI = _ui as MPStorageEditDeckUI;
		_storagePartyEditUI.SetUp(MPSharedStateInfo.storageDecksInfo.editDeckIndex, MPSharedStateInfo.storageDecksInfo.selectedMortyData);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_storagePartyEditUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_storagePartyEditUI.SelectedEvent += UIMortySelectedHandler;
		_storagePartyEditUI.StatsEvent += UIStatsHandler;
		_storagePartyEditUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storagePartyEditUI.SelectedEvent -= UIMortySelectedHandler;
		_storagePartyEditUI.StatsEvent -= UIStatsHandler;
		_storagePartyEditUI.BackEvent -= UIBackHandler;
	}

	private void SaveDeck(Action<bool> callback)
	{
		List<string> list = _storagePartyEditUI.deckMorties.Select((MPOpponentMortyData d) => d.netId).ToList();
		if (!list.SequenceEqual(App.net.player.data.GetDeckMortyIDs(MPSharedStateInfo.storageDecksInfo.editDeckIndex)))
		{
			if (MPSharedStateInfo.storageDecksInfo.editDeckIndex == App.net.player.data.activeDeckIndex && _storagePartyEditUI.deckMorties.FirstOrDefault((MPOpponentMortyData e) => !e.isFainted) == null)
			{
				_storagePartyEditUI.DisplayEditDeckErrorAllFainted();
				callback(false);
				return;
			}
			_storagePartyEditUI.DisplayWaitingToSet();
			App.net.player.EditDeck(MPSharedStateInfo.storageDecksInfo.editDeckIndex, list, delegate(eEditDeckError error)
			{
				if (error != eEditDeckError.NONE)
				{
					StartCoroutine(DisplayEditDeckErrorCo(error));
				}
				else
				{
					callback(true);
				}
			});
		}
		else
		{
			callback(true);
		}
	}

	private IEnumerator DisplayEditDeckErrorCo(eEditDeckError error)
	{
		yield return StartCoroutine(_storagePartyEditUI.DisplayEditDeckErrorCo(error));
		MPSharedStateInfo.storageDecksInfo.selectedMortyData = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIMortySelectedHandler(MPOpponentMortyData mortyData)
	{
		MPSharedStateInfo.storageDecksInfo.selectedMortyData = mortyData;
	}

	private void UIStatsHandler(MPOpponentMortyData mortyData)
	{
		SaveDeck(delegate(bool isSuccess)
		{
			if (isSuccess)
			{
				MPSharedStateInfo.mortyStatsInfo = new MPMortyStatsInfo(eMPStateOrigin.EDIT_DECK, mortyData, true);
				App.states.Set<MPMortyStatsState>(new FadeTransInfo(), new FadeTransInfo(), true);
			}
			else
			{
				_storagePartyEditUI.DisplayEditDeckErrorAllFainted();
			}
		});
	}

	private void UIBackHandler()
	{
		SaveDeck(delegate(bool isSuccess)
		{
			if (isSuccess)
			{
				MPSharedStateInfo.storageDecksInfo.selectedMortyData = null;
				App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
			}
			else
			{
				_storagePartyEditUI.DisplayEditDeckErrorAllFainted();
			}
		});
	}
}
