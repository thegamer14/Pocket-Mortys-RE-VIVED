using System.Collections.Generic;
using UnityEngine;

public class MPStorageCombineState : StateBase
{
	private MPStorageCombineUI _storageCombineUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPStorageCombine/{0}", "MPStorageCombine");
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
		_storageCombineUI = _ui as MPStorageCombineUI;
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
		_storageCombineUI.AbsorbEvent += UIAbosrbHandler;
		_storageCombineUI.StatsEvent += UIStatsHandler;
		_storageCombineUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storageCombineUI.AbsorbEvent -= UIAbosrbHandler;
		_storageCombineUI.StatsEvent -= UIStatsHandler;
		_storageCombineUI.BackEvent -= UIBackHandler;
	}

	private void UIAbosrbHandler(MPOpponentMortyData evolveMorty, List<string> absorbMortyNetIds)
	{
		_storageCombineUI.DisplayWaitingToAbsorb();
		int preEvolvePoints = evolveMorty.evolutionPoints;
		string preEvolveMortyId = evolveMorty.id;
		App.net.player.AbsorbMorties(evolveMorty.netId, absorbMortyNetIds, delegate(eAbsorbMortysError error, MPOpponentMortyData mortyData, bool isMortyNew, List<string> removedMortyNetIds)
		{
			if (error == eAbsorbMortysError.NONE)
			{
				App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.DAYCARE, "combined_morty");
				if (isMortyNew)
				{
					StartCoroutine(_storageCombineUI.DisplayAbsorbResultEvolved(mortyData, preEvolveMortyId, MPMortyDefs.EvolutionRequirement(preEvolveMortyId) - preEvolvePoints));
				}
				else
				{
					StartCoroutine(_storageCombineUI.DisplayAbsorbResultGainedPoints(mortyData, mortyData.evolutionPoints - preEvolvePoints, removedMortyNetIds));
				}
			}
			else
			{
				_storageCombineUI.DisplayAbsorbError();
			}
		});
	}

	private void UIStatsHandler(MPOpponentMortyData mortyData)
	{
		MPSharedStateInfo.mortyStatsInfo = new MPMortyStatsInfo(eMPStateOrigin.COMBINE, mortyData, false);
		App.states.Set<MPMortyStatsState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.mortyStatsInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
