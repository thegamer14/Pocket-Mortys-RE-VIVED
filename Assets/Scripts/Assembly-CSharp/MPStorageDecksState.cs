using UnityEngine;

public class MPStorageDecksState : StateBase
{
	private MPStorageDecksUI _storageDecksUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPStorageDecks/{0}", "MPStorageDecks");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (MPSharedStateInfo.storageDecksInfo == null)
		{
			MPSharedStateInfo.storageDecksInfo = new MPStorageDecksInfo();
		}
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Storage/RaM204m12.BBQBackground.01.08.44.10.Ver1.0.FUTZED");
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_storageDecksUI = _ui as MPStorageDecksUI;
		_storageDecksUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_storageDecksUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_storageDecksUI.WithdrawEvent += UIWithdrawHandler;
		_storageDecksUI.EditEvent += UIEditHandler;
		_storageDecksUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storageDecksUI.WithdrawEvent -= UIWithdrawHandler;
		_storageDecksUI.EditEvent -= UIEditHandler;
		_storageDecksUI.BackEvent -= UIBackHandler;
	}

	private void UIEditHandler(int selectedDeckIndex = 0)
	{
		MPSharedStateInfo.storageDecksInfo.editDeckIndex = selectedDeckIndex;
		App.states.Set<MPStorageEditDeckState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIWithdrawHandler(int selectedDeckIndex = 0)
	{
		if (App.net.player.data.GetDeckCount(selectedDeckIndex) == 0)
		{
			_storageDecksUI.DisplayCannotWithdrawEmptyDeck();
			return;
		}
		if (App.net.player.data.GetDeckAliveCount(selectedDeckIndex) <= 0)
		{
			_storageDecksUI.DisplayCannotWithdrawFaintedDeck();
			return;
		}
		_storageDecksUI.DisplayWaitingToWithdraw();
		App.net.player.SetActiveDeck(selectedDeckIndex, delegate(eSetActiveDeckError error)
		{
			if (error == eSetActiveDeckError.NONE)
			{
				_storageDecksUI.UpdateAfterDeckWithdrawn();
			}
			else
			{
				_storageDecksUI.DisplayWithdrawError();
			}
		});
	}

	private void UIBackHandler(int selectedDeckIndex = 0)
	{
		MPSharedStateInfo.storageDecksInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
