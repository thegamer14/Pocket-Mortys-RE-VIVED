using UnityEngine;

public class MPRegisterMortiesState : StateBase
{
	private MPRegisterMortiesUI _registerMortiesUI;

	private bool _tutorialShown;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPRegisterMorties/{0}", "MPRegisterMorties");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.ONBOARDING, "selected_mortys");
		StartCoroutine(ActivateComplete());
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (!_tutorialShown && MPSharedStateInfo.registrationInfo.mortyIDs == null)
		{
			App.states.Get<MPTutorialPopupState>().SetDisplayValues(string.Format(MPTextDefs.MP_REGISTER_MORTIES_TIP, 3));
			App.states.Set<MPTutorialPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
			_tutorialShown = true;
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_registerMortiesUI = _ui as MPRegisterMortiesUI;
		_registerMortiesUI.SetUp(MPSharedStateInfo.registrationInfo.mortyIDs);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_registerMortiesUI = null;
		_tutorialShown = false;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_registerMortiesUI.NextEvent += UINextHandler;
		_registerMortiesUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_registerMortiesUI.NextEvent -= UINextHandler;
		_registerMortiesUI.BackEvent -= UIBackHandler;
	}

	private void UINextHandler()
	{
		MPSharedStateInfo.registrationInfo.mortyIDs = _registerMortiesUI.GetSelectedMortyIds();
		App.states.Set<MPRegisterUsernameState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.registrationInfo.mortyIDs = _registerMortiesUI.GetSelectedMortyIds();
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
