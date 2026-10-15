using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MPMortyLabAttacksState : StateBase
{
	private MPMortyLabAttacksUI _attacksUI;

	private GenericPopupState _popupState;

	private bool _inBuyCouponFlow;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPMortyLabAttacks/{0}", "MPMortyLabAttacks");
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
			if (App.net.player.data.coupons > 0)
			{
				DisplaySaveChangesPopup();
			}
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_attacksUI = _ui as MPMortyLabAttacksUI;
		_attacksUI.SetUp(MPSharedStateInfo.labsInfo.selectedMortyData);
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
		_popupState.SetDisplayValues(MPTextDefs.MP_MORTY_LAB_SAVE_CHANGES, new string[2]
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
		_popupState.SetDisplayValues(MPTextDefs.MP_MORTY_LAB_SAVE_CHANGES_BUY, new string[2]
		{
			MPTextDefs.MP_MORTY_LAB_SAVE_CHANGES_BUY_BTN_YES,
			TextDefs.MORTY_LAB_SAVE_CHANGES_BUY_BTN_NO
		});
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		_popupState.ButtonClickedEvent += BuyCouponButtonClickedHandler;
	}

	private void UIBackHandler()
	{
		bool flag = MPSharedStateInfo.labsInfo.selectedMortyData.attacks.Select((KeyValuePair<int, MPAttackData> kvp) => kvp.Key).ToList().SequenceEqual(_attacksUI.tempAttacks.Select((KeyValuePair<int, MPAttackData> kvp) => kvp.Key).ToList());
		if (flag)
		{
			foreach (KeyValuePair<int, MPAttackData> attack in MPSharedStateInfo.labsInfo.selectedMortyData.attacks)
			{
				if (attack.Value.id != _attacksUI.tempAttacks[attack.Key].id)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
		else if (_attacksUI.tempAttacks.Count == 0)
		{
			DisplayNoLearnedAttacksPopup();
		}
		else if (App.net.player.data.coupons > 0)
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
		App.states.Set<MPCouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
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
			_popupState.SetDisplayValues(MPTextDefs.MP_MORTY_LAB_SAVING_CHANGES, null, false, true);
			Dictionary<string, SortedList<int, MPAttackData>> dictionary = new Dictionary<string, SortedList<int, MPAttackData>>();
			dictionary.Add(MPSharedStateInfo.labsInfo.selectedMortyData.netId, _attacksUI.tempAttacks);
			App.net.player.EditMortyAttacks(dictionary, true, delegate(eEditAttacksError error)
			{
				if (error == eEditAttacksError.NONE)
				{
					_popupState.SetDisplayValues(TextDefs.MORTY_LAB_SAVE_CHANGES_CONFIRMATION, null);
				}
				else
				{
					_popupState.SetDisplayValues(MPTextDefs.MP_MORTY_LAB_SAVE_CHANGES_ERROR, null);
				}
				_popupState.ButtonClickedEvent += SaveConfirmedButtonClickedHandler;
			});
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
			App.states.Set<MPCouponBuyPopupState>(new PopTransInfo(), new PopTransInfo());
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
