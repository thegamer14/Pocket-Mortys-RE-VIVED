using UnityEngine;

public class MPMortyLabPartyState : StateBase
{
	private MPMortyLabPartyUI _partyUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPMortyLabParty/{0}", "MPMortyLabParty");
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
		_partyUI = _ui as MPMortyLabPartyUI;
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

	private void UIMortySelectedHandler(MPOpponentMortyData mortyData)
	{
		MPSharedStateInfo.labsInfo.selectedMortyData = mortyData;
		App.states.Set<MPMortyLabAttacksState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
