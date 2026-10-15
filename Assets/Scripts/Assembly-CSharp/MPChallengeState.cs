using UnityEngine;

public class MPChallengeState : StateBase
{
	private MPChallengeUI _challengeUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPChallenge/{0}", "MPChallenge");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.net.player.GetChallenges();
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_challengeUI = _ui as MPChallengeUI;
		_challengeUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_challengeUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_challengeUI.ClaimEvent += UIClaimHandler;
		_challengeUI.BackEvent += UIBackHandler;
		App.net.player.GetChallengeEvent += NetPlayerGetChallengeHandler;
		App.net.player.ChallengeCompleteEvent += NetPlayerChallengeCompleteHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_challengeUI.ClaimEvent -= UIClaimHandler;
		_challengeUI.BackEvent -= UIBackHandler;
		App.net.player.GetChallengeEvent -= NetPlayerGetChallengeHandler;
		App.net.player.ChallengeCompleteEvent -= NetPlayerChallengeCompleteHandler;
	}

	private void UIClaimHandler(MPChallengeData data)
	{
		App.net.player.ClaimChallenge(data, delegate(MPRewardData rewardData, eClaimChallengeError error)
		{
			if (error == eClaimChallengeError.NONE)
			{
				MPSharedStateInfo.rewardInfo = new MPRewardInfo(eMPStateOrigin.CHALLENGE, new MPRewardData[1] { rewardData }, "Challenge");
				App.states.Set<MPRewardState>(new FadeTransInfo
				{
					duration = 0.05f,
					color = Color.white
				}, new FadeTransInfo
				{
					duration = 0.2f,
					color = Color.white
				}, true);
			}
			else
			{
				_challengeUI.DisplayClaimError(error);
			}
		});
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void NetPlayerGetChallengeHandler(eGetChallengesError error)
	{
		if (error == eGetChallengesError.NONE)
		{
			_challengeUI.SetScrollMenu(App.net.player.data.challenges);
		}
		else
		{
			_challengeUI.DisplayGetError(error);
		}
	}

	private void NetPlayerChallengeCompleteHandler()
	{
		_challengeUI.SetScrollMenu(App.net.player.data.challenges);
	}
}
