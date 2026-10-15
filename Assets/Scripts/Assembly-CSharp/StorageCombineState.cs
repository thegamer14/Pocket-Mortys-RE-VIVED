using UnityEngine;

public class StorageCombineState : StateBase
{
	private StorageCombineUI _storageCombineUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/StorageCombine/{0}", "StorageCombine");
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
		_storageCombineUI = _ui as StorageCombineUI;
		_storageCombineUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_storageCombineUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_storageCombineUI.CombineEvent += UICombineHandler;
		_storageCombineUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storageCombineUI.CombineEvent -= UICombineHandler;
		_storageCombineUI.BackEvent -= UIBackHandler;
	}

	private void UICombineHandler(MortyData mortyData1, MortyData mortyData2)
	{
		string resultsText = null;
		MortyData mortyData3 = PlayerDefs.CombineMorties(mortyData1, mortyData2, out resultsText);
		if (mortyData3 != null)
		{
			App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.DAYCARE, "combined_morty");
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQEQ");
			App.saveData.Save(eSaveDataType.PLAYER);
			_storageCombineUI.CombineDatasComplete(mortyData3, resultsText);
		}
		else
		{
			_storageCombineUI.CombineDatasFailed(resultsText);
		}
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
