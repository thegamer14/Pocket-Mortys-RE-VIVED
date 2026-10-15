using System.Linq;
using UnityEngine;

public class MortyLabAttacksState : StateBase
{
	private MortyLabAttacksUI _attacksUI;

	private GenericPopupState _popupState;

	private bool _inBuyCouponFlow;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MortyLabAttacks/{0}", "MortyLabAttacks");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		_inBuyCouponFlow = false;
		StartCoroutine(ActivateComplete());
	}

	public override void SetAsCurrentComplete()
	{
		if (_inBuyCouponFlow)
		{
			_inBuyCouponFlow = false;
			if (PlayerDefs.CouponCount > 0)
			{
				DisplaySaveChangesPopup();
			}
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_attacksUI = _ui as MortyLabAttacksUI;
		_attacksUI.SetUp(SharedStatInfo.labsInfo.selectedMortyData);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_attacksUI = null;
		_popupState = null;
		_inBuyCouponFlow = false;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_attacksUI.BackEvent += UIBackHandler;
		_attacksUI.BuyCouponsBtnEvent += UIBuyCouponsBtnHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_attacksUI.BackEvent -= UIBackHandler;
		_attacksUI.BuyCouponsBtnEvent -= UIBuyCouponsBtnHandler;
	}

	private void DisplayNoLearnedAttacksPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.MORTY_LAB_NO_ATTACKS_LEARNED, new string[2]
		{
			TextDefs.MORTY_LAB_NO_ATTACKS_LEARNED_BTN_CONTINUE,
			TextDefs.MORTY_LAB_NO_ATTACKS_LEARNED_BTN_DISCARD
		}, false);
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		_popupState.ButtonClickedEvent += NoLearnedAttacksButtonClickedHandler;
	}

	private void DisplaySaveChangesPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.MORTY_LAB_SAVE_CHANGES, new string[2]
		{
			TextDefs.MORTY_LAB_SAVE_CHANGES_BTN_YES,
			TextDefs.MORTY_LAB_SAVE_CHANGES_BTN_NO
		});
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		_popupState.ButtonClickedEvent += SaveChangesButtonClickedHandler;
	}

	private void DisplayBuyCouponPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.MORTY_LAB_SAVE_CHANGES_BUY, new string[2]
		{
			TextDefs.MORTY_LAB_SAVE_CHANGES_BUY_BTN_YES,
			TextDefs.MORTY_LAB_SAVE_CHANGES_BUY_BTN_NO
		});
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		_popupState.ButtonClickedEvent += BuyCouponButtonClickedHandler;
	}

	private void UIBackHandler()
	{
		if (SharedStatInfo.labsInfo.selectedMortyData.attackIDs.SequenceEqual(_attacksUI.tempAttackIDs))
		{
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
			return;
		}
		bool flag = false;
		for (int i = 0; i < _attacksUI.tempAttackIDs.Count; i++)
		{
			if (flag)
			{
				break;
			}
			if (!string.IsNullOrEmpty(_attacksUI.tempAttackIDs[i]))
			{
				flag = true;
			}
		}
		if (!flag)
		{
			DisplayNoLearnedAttacksPopup();
		}
		else if (PlayerDefs.CouponCount > 0)
		{
			DisplaySaveChangesPopup();
		}
		else
		{
			DisplayBuyCouponPopup();
		}
	}

	private void UIBuyCouponsBtnHandler()
	{
		App.states.Set<CouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void NoLearnedAttacksButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= NoLearnedAttacksButtonClickedHandler;
		switch (buttonIndex)
		{
		case 1:
			App.states.SetBack(new PopTransInfo());
			break;
		case 2:
			StartCoroutine(App.states.SetBackFromPopup(new FadeTransInfo(), new FadeTransInfo()));
			break;
		}
	}

	private void SaveChangesButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= SaveChangesButtonClickedHandler;
		switch (buttonIndex)
		{
		case 0:
			App.states.SetBack(new PopTransInfo());
			break;
		case 1:
		{
			for (int i = 0; i < _attacksUI.tempAttackIDs.Count; i++)
			{
				string text = _attacksUI.tempAttackIDs[i];
				if (text != null && !SharedStatInfo.labsInfo.selectedMortyData.attackIDs.Contains(text))
				{
					App.upsight.CustomAnalyticEvent(eSubType1.MORTY_LABS, eSubType2.MORTY_NAME, text);
				}
			}
			PlayerDefs.RemoveCoupon(1);
			SharedStatInfo.labsInfo.selectedMortyData.attackIDs = _attacksUI.tempAttackIDs;
			SharedStatInfo.labsInfo.selectedMortyData.RestoreAllPPFull();
			App.saveData.Save(eSaveDataType.PLAYER);
			_popupState.ButtonClickedEvent += SaveConfirmedButtonClickedHandler;
			_popupState.SetDisplayValues(TextDefs.MORTY_LAB_SAVE_CHANGES_CONFIRMATION, null);
			break;
		}
		case 2:
			StartCoroutine(App.states.SetBackFromPopup(new FadeTransInfo(), new FadeTransInfo()));
			break;
		}
	}

	private void BuyCouponButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= BuyCouponButtonClickedHandler;
		switch (buttonIndex)
		{
		case 0:
			App.states.SetBack(new PopTransInfo());
			break;
		case 1:
			_inBuyCouponFlow = true;
			App.states.Set<CouponBuyPopupState>(new PopTransInfo(), new PopTransInfo());
			break;
		case 2:
			StartCoroutine(App.states.SetBackFromPopup(new FadeTransInfo(), new FadeTransInfo()));
			break;
		}
	}

	private void SaveConfirmedButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= SaveConfirmedButtonClickedHandler;
		StartCoroutine(App.states.SetBackFromPopup(new FadeTransInfo(), new FadeTransInfo()));
	}
}
