using UnityEngine;

public class BagState : StateBase
{
	private BagUI _bagUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Bag/{0}", "Bag");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (SharedStatInfo.bagInfo == null)
		{
			SharedStatInfo.bagInfo = new BagInfo(PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems(), eBagFromState.NONE, null);
		}
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_bagUI = _ui as BagUI;
		MortyData selectedMorty = ((SharedStatInfo.partyInfo == null) ? null : SharedStatInfo.partyInfo.selectedMortyData);
		_bagUI.SetUp(SharedStatInfo.bagInfo.items, SharedStatInfo.bagInfo.selectedID, selectedMorty);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_bagUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_bagUI.ItemSelectedEvent += UIItemSelectedHandler;
		_bagUI.UseEvent += UIUseHandler;
		_bagUI.GiveEvent += UIGiveHandler;
		_bagUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_bagUI.ItemSelectedEvent -= UIItemSelectedHandler;
		_bagUI.UseEvent -= UIUseHandler;
		_bagUI.GiveEvent -= UIGiveHandler;
		_bagUI.BackEvent -= UIBackHandler;
	}

	private void UIItemSelectedHandler(CellBase selectedCell)
	{
		SharedStatInfo.bagInfo.selectedID = selectedCell.id;
	}

	private void UIUseHandler()
	{
		if (SharedStatInfo.craftInfo != null)
		{
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
		else if (SharedStatInfo.partyInfo == null)
		{
			if (!ItemDefs.UseOnSelf(SharedStatInfo.bagInfo.selectedID))
			{
				if (SharedStatInfo.battleInfo != null)
				{
					SharedStatInfo.partyInfo = new PartyInfo(SharedStatInfo.battleInfo.player.mortyDatas, SharedStatInfo.battleInfo.player.items, SharedStatInfo.battleInfo.player.currentMortyData);
					App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
				}
				else if (SharedStatInfo.bagInfo.selectedID == "ItemCourierFlap")
				{
					string resultText;
					ItemDefs.ApplyItemToMorty("ItemCourierFlap", PlayerDefs.CurrentItems(), null, null, null, out resultText);
					App.saveData.Save(eSaveDataType.PLAYER);
					SharedStatInfo.bagInfo = null;
					App.states.Set<CourierFlapState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
				}
			}
			else
			{
				SharedStatInfo.partyInfo = new PartyInfo(SharedStatInfo.bagInfo.mortyDatas, SharedStatInfo.bagInfo.items);
				App.states.Set<PartyState>(new FadeTransInfo(), new FadeTransInfo(), true);
			}
		}
		else if (SharedStatInfo.partyInfo.selectedMortyData != null)
		{
			SharedStatInfo.partyInfo.startingMortyData = SharedStatInfo.partyInfo.selectedMortyData;
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}

	private void UIGiveHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIBackHandler()
	{
		if (SharedStatInfo.bagInfo.questGiverNode != null)
		{
			SharedStatInfo.bagInfo.selectedID = null;
		}
		else
		{
			SharedStatInfo.bagInfo = null;
		}
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
