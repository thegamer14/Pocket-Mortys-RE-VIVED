using System.Collections.Generic;
using UnityEngine;

public class MPRegisterAvatarState : StateBase
{
	private MPRegisterAvatarUI _customizationUI;

	private bool _tutorialShown;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPRegisterAvatar/{0}", "MPRegisterAvatar");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.ONBOARDING, "picked_avatar");
		if (MPSharedStateInfo.registrationInfo == null)
		{
			MPSharedStateInfo.registrationInfo = new MPRegistrationInfo();
		}
		StartCoroutine(ActivateComplete());
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (!_tutorialShown && MPSharedStateInfo.registrationInfo.avatarID == null)
		{
			App.states.Get<MPTutorialPopupState>().SetDisplayValues(MPTextDefs.MP_REGISTER_AVATAR_TIP);
			App.states.Set<MPTutorialPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
			_tutorialShown = true;
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_customizationUI = _ui as MPRegisterAvatarUI;
		_customizationUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_customizationUI = null;
		_tutorialShown = false;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_customizationUI.UseEvent += UIUseHandler;
		_customizationUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_customizationUI.UseEvent -= UIUseHandler;
		_customizationUI.BackEvent -= UIBackHandler;
	}

	private void UIUseHandler(string id)
	{
		MPSharedStateInfo.registrationInfo.avatarID = id;
		List<string> userRegisterMortyIDs = MPMortyDefs.GetUserRegisterMortyIDs();
		if (userRegisterMortyIDs.Count <= 3)
		{
			MPSharedStateInfo.registrationInfo.mortyIDs = userRegisterMortyIDs;
			App.states.Set<MPRegisterUsernameState>(new FadeTransInfo(), new FadeTransInfo(), true);
		}
		else
		{
			App.states.Set<MPRegisterMortiesState>(new FadeTransInfo(), new FadeTransInfo(), true);
		}
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.registrationInfo = null;
		App.states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.DEACTIVATE_ALL);
	}
}
