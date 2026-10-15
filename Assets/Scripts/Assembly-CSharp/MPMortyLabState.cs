using UnityEngine;

public class MPMortyLabState : StateBase
{
	private MPMortyLabUI _mortyLabsUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPMortyLab/{0}", "MPMortyLab");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Store/RaM202m01.HumanMuzak.01005513.Ver1.0.Clean", 0.6f);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_mortyLabsUI = _ui as MPMortyLabUI;
	}

	protected override void Destroy()
	{
		base.Destroy();
		_mortyLabsUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_mortyLabsUI.ModifyEvent += UIModifyHandler;
		_mortyLabsUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_mortyLabsUI.ModifyEvent -= UIModifyHandler;
		_mortyLabsUI.BackEvent -= UIBackHandler;
	}

	private void UIModifyHandler()
	{
		App.states.Set<MPMortyLabPartyState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.labsInfo = null;
		App.sound.StopSoundtrack(0.5f);
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
