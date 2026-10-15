using UnityEngine;

public class MPHealingCenterState : StateBase
{
	private MPHealingCenterUI _healingCenterUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPHealingCenter/{0}", "MPHealingCenter");
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
		_healingCenterUI = _ui as MPHealingCenterUI;
		_healingCenterUI.SetUp(MPSharedStateInfo.healingCenterInfo.isRevive);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_healingCenterUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_healingCenterUI.TryHealEvent += UITryHealHandler;
		_healingCenterUI.HealAnimationCompleteEvent += UIHealAnimationCompleteHandler;
		_healingCenterUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_healingCenterUI.TryHealEvent -= UITryHealHandler;
		_healingCenterUI.HealAnimationCompleteEvent -= UIHealAnimationCompleteHandler;
		_healingCenterUI.BackEvent -= UIBackHandler;
	}

	private void UITryHealHandler()
	{
		App.net.player.HealActiveMortyDeck(delegate(eHealActiveMortyDeckError error)
		{
			if (error == eHealActiveMortyDeckError.NONE)
			{
				App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.HEALING_CENTER, "use_healing_center");
				StartCoroutine(_healingCenterUI.HealCo());
			}
			else
			{
				_healingCenterUI.HealFailed(error);
			}
		});
	}

	private void UIHealAnimationCompleteHandler()
	{
		MPSharedStateInfo.healingCenterInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.healingCenterInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
