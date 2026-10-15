using UnityEngine;

public class StorageDecksState : StateBase
{
	private StorageDecksUI _storageDecksUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/StorageDecks/{0}", "StorageDecks");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (SharedStatInfo.storageDecksInfo == null)
		{
			SharedStatInfo.storageDecksInfo = new StorageDecksInfo();
		}
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Storage/RaM204m12.BBQBackground.01.08.44.10.Ver1.0.FUTZED");
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_storageDecksUI = _ui as StorageDecksUI;
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
		SharedStatInfo.storageDecksInfo.editDeckIndex = selectedDeckIndex;
		App.states.Set<StorageEditDeckState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIWithdrawHandler(int selectedDeckIndex = 0)
	{
		string failMessage;
		if (PlayerDefs.SetPartyMortiesFromDeck(selectedDeckIndex, out failMessage))
		{
			_storageDecksUI.UpdateAfterDeckWithdrawn();
			App.saveData.Save(eSaveDataType.PLAYER);
		}
		else
		{
			_storageDecksUI.DisplayDialogue(failMessage);
		}
	}

	private void UIBackHandler(int selectedDeckIndex = 0)
	{
		SharedStatInfo.storageDecksInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
