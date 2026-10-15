using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBootcampResultsState : StateBase
{
	private StorageBootcampResultsUI _resultUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/StorageBootcampResults/{0}", "StorageBootcampResults");
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
		_resultUI = _ui as StorageBootcampResultsUI;
		_resultUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_resultUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_resultUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_resultUI.BackEvent -= UIBackHandler;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		StartCoroutine(RewardXPToMorties());
	}

	private IEnumerator RewardXPToMorties()
	{
		List<MortyData> mortyDatas = BootcampDefs.GetMortiesForBootcamp(SharedStatInfo.storageBootcampInfo.selectedBootcampIndex);
		foreach (MortyData mortyData in mortyDatas)
		{
			yield return StartCoroutine(_resultUI.RewardXP(mortyData, BootcampDefs.GetXPRewardForBootcamp(SharedStatInfo.storageBootcampInfo.selectedBootcampIndex, true)));
		}
		BootcampDefs.SetBootcampIdle(SharedStatInfo.storageBootcampInfo.selectedBootcampIndex);
		App.saveData.Save(eSaveDataType.PLAYER);
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIBackHandler()
	{
		App.saveData.Save(eSaveDataType.PLAYER);
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
