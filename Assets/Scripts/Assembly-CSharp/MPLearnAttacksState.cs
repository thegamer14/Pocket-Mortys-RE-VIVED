using System.Collections.Generic;
using UnityEngine;

public class MPLearnAttacksState : StateBase
{
	private MPLearnAttacksUI _learnAttacksUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPLearnAttacks/{0}", "MPLearnAttacks");
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
		_learnAttacksUI = _ui as MPLearnAttacksUI;
		_learnAttacksUI.SetUp(MPSharedStateInfo.learnAttacksInfo.backgroundId, MPSharedStateInfo.learnAttacksInfo.GetAttackToLearnData());
	}

	protected override void Destroy()
	{
		base.Destroy();
		_learnAttacksUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_learnAttacksUI.FinishedEvent += UIFinishedEventHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_learnAttacksUI.FinishedEvent -= UIFinishedEventHandler;
	}

	private void UIFinishedEventHandler(Dictionary<string, SortedList<int, MPAttackData>> newAttacks)
	{
		App.net.player.EditMortyAttacks(newAttacks, false, delegate
		{
			if (MPSharedStateInfo.learnAttacksInfo.IfOrigin(eMPStateOrigin.BATTLE))
			{
				if (MPSharedStateInfo.learnAttacksInfo.battlePlayerData.IsRewardStateRequired())
				{
					MPSharedStateInfo.rewardInfo = new MPRewardInfo(MPSharedStateInfo.learnAttacksInfo.origin | eMPStateOrigin.LEARN_ATTACK, MPSharedStateInfo.learnAttacksInfo.battlePlayerData, "battle_over");
					MPSharedStateInfo.learnAttacksInfo = null;
					App.states.Set<MPRewardState>(new FadeTransInfo
					{
						duration = 0.05f,
						color = Color.white
					}, new FadeTransInfo
					{
						duration = 0.2f,
						color = Color.white
					});
				}
				else if (MPSharedStateInfo.learnAttacksInfo.battlePlayerData.IsPlayerLevelStateRequired())
				{
					MPSharedStateInfo.playerLevelInfo = new MPPlayerLevelInfo(MPSharedStateInfo.learnAttacksInfo.origin | eMPStateOrigin.LEARN_ATTACK, MPSharedStateInfo.worldInfo.worldId, MPSharedStateInfo.learnAttacksInfo.battlePlayerData);
					MPSharedStateInfo.learnAttacksInfo = null;
					App.states.Set<MPPlayerLevelState>(new FadeTransInfo
					{
						duration = 0.05f,
						color = Color.white
					}, new FadeTransInfo
					{
						duration = 0.2f,
						color = Color.white
					});
				}
				else if (MPSharedStateInfo.learnAttacksInfo.battlePlayerData.IsVideoRewardStateRequired())
				{
					MPSharedStateInfo.videoRewardMeeseekInfo = new MPVideoRewardMeeseekInfo(MPSharedStateInfo.learnAttacksInfo.origin | eMPStateOrigin.LEARN_ATTACK, MPSharedStateInfo.learnAttacksInfo.battlePlayerData.videoRewardMeeseeksData);
					MPSharedStateInfo.learnAttacksInfo = null;
					App.states.Set<MPVideoRewardMeeseekState>(new FadeTransInfo(), new FadeTransInfo());
				}
				else
				{
					MPSharedStateInfo.learnAttacksInfo = null;
					App.states.Get<InterstitialState>().nextStateType = typeof(MPWorldState);
					App.states.Get<InterstitialState>().placementID = eUpsightPlacementID.BATTLE_OVER;
					App.states.Set<InterstitialState>(new FadeTransInfo(), null, false, eStateAction.DEACTIVATE_ALL);
				}
			}
			else if (MPSharedStateInfo.learnAttacksInfo.IfOrigin(eMPStateOrigin.PARTY))
			{
				MPSharedStateInfo.learnAttacksInfo = null;
				App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
			}
		});
	}
}
