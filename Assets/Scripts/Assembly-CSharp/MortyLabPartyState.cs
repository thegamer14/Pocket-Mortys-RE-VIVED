using UnityEngine;

public class MortyLabPartyState : StateBase
{
	private MortyLabPartyUI _partyUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MortyLabParty/{0}", "MortyLabParty");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (SharedStatInfo.labsInfo == null)
		{
			SharedStatInfo.labsInfo = new LabsInfo();
		}
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_partyUI = _ui as MortyLabPartyUI;
		_partyUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_partyUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_partyUI.MortySelectedEvent += UIMortySelectedHandler;
		_partyUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_partyUI.MortySelectedEvent -= UIMortySelectedHandler;
		_partyUI.BackEvent -= UIBackHandler;
	}

	private void UIMortySelectedHandler(MortyData mortyData)
	{
		SharedStatInfo.labsInfo.selectedMortyData = mortyData;
		App.states.Set<MortyLabAttacksState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		SharedStatInfo.labsInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
