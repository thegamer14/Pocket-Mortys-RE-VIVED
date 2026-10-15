using UnityEngine;

public class HealingCenterState : StateBase
{
	private HealingCenterUI _healingCenterUI;

	public bool isRevive;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/HealingCenter/{0}", "HealingCenter");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Store/RaM202m01.HumanMuzak.01005513.Ver1.0.Clean", 0.8f);
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0f);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_healingCenterUI = _ui as HealingCenterUI;
		_healingCenterUI.SetUp(isRevive);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_healingCenterUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_healingCenterUI.HealEvent += UIHealHandler;
		_healingCenterUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_healingCenterUI.HealEvent -= UIHealHandler;
		_healingCenterUI.BackEvent -= UIBackHandler;
	}

	private void UIHealHandler()
	{
		PlayerDefs.CurrentPartyReviveAllToFull();
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.HEALING_CENTER, "use_healing_center");
		App.saveData.Save(eSaveDataType.PLAYER);
		if (isRevive)
		{
			isRevive = false;
			SharedStatInfo.worldInfo.worldID = "Citadel";
			SharedStatInfo.worldInfo.entranceID = "CitadelHealingCenter";
			App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
		}
		else
		{
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
