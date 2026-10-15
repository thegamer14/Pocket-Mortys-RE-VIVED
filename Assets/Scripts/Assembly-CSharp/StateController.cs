using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateController : MonoBehaviour
{
	public delegate void StateControllerDelegate();

	private const int _kBackStackLimit = 20;

	private List<KeyValuePair<UILayer, StateBase>> _uiLayerToState = new List<KeyValuePair<UILayer, StateBase>>();

	private Dictionary<StateBase, string> _states;

	private StateBase _currentState;

	private List<StateBase> _backStack;

	private List<StateBase> _statesWaitingToActivate;

	private List<StateBase> _statesWaitingToDeactivate;

	private List<StateBase> _statesWaitingToCache;

	public StateBase currentState
	{
		get
		{
			return _currentState;
		}
	}

	public MainCanvas mainCanvas { get; private set; }

	public event StateControllerDelegate BackButtonEvent;

	private void Awake()
	{
		_states = new Dictionary<StateBase, string>();
		_currentState = null;
		_backStack = new List<StateBase>();
		_statesWaitingToDeactivate = new List<StateBase>();
		_statesWaitingToActivate = new List<StateBase>();
		_statesWaitingToCache = new List<StateBase>();
		mainCanvas = GameObject.Find("MainCanvas").GetComponent<MainCanvas>();
		AddStates();
		AddStateEvents(true);
	}

	public void SetStartingState()
	{
		KeyValuePair<StateBase, string> keyValuePair = _states.FirstOrDefault((KeyValuePair<StateBase, string> e) => e.Value == SceneManager.GetActiveScene().name);
		if (!keyValuePair.Equals(default(KeyValuePair<StateBase, string>)))
		{
			StartCoroutine(SetCurrentCo(keyValuePair.Key, null, new FadeTransInfo(), eStateAction.DEACTIVATE_ALL));
		}
	}

	private void AddStates()
	{
		_states.Add(base.gameObject.AddComponent<WorldState>(), "WorldScene");
		_states.Add(base.gameObject.AddComponent<MPWorldState>(), "WorldScene");
		_states.Add(base.gameObject.AddComponent<AppUpdateState>(), null);
		_states.Add(base.gameObject.AddComponent<BadgeCollectionState>(), null);
		_states.Add(base.gameObject.AddComponent<BagState>(), null);
		_states.Add(base.gameObject.AddComponent<BattleState>(), null);
		_states.Add(base.gameObject.AddComponent<BlackedOutState>(), null);
		_states.Add(base.gameObject.AddComponent<CouponBuyPopupState>(), null);
		_states.Add(base.gameObject.AddComponent<CouponUseState>(), null);
		_states.Add(base.gameObject.AddComponent<CourierFlapState>(), null);
		_states.Add(base.gameObject.AddComponent<CraftState>(), null);
		_states.Add(base.gameObject.AddComponent<CraftRecipeState>(), null);
		_states.Add(base.gameObject.AddComponent<HealingCenterState>(), null);
		_states.Add(base.gameObject.AddComponent<CreditsState>(), null);
		_states.Add(base.gameObject.AddComponent<GenericPopupState>(), null);
		_states.Add(base.gameObject.AddComponent<InterstitialState>(), null);
		_states.Add(base.gameObject.AddComponent<IapFlowState>(), null);
		_states.Add(base.gameObject.AddComponent<MainMenuState>(), "MainMenuScene");
		_states.Add(base.gameObject.AddComponent<MortydexState>(), null);
		_states.Add(base.gameObject.AddComponent<MortydexEntryState>(), null);
		_states.Add(base.gameObject.AddComponent<MortyLabState>(), null);
		_states.Add(base.gameObject.AddComponent<MortyLabPartyState>(), null);
		_states.Add(base.gameObject.AddComponent<MortyLabAttacksState>(), null);
		_states.Add(base.gameObject.AddComponent<MortyStatsState>(), null);
		_states.Add(base.gameObject.AddComponent<PartyState>(), null);
		_states.Add(base.gameObject.AddComponent<PauseState>(), null);
		_states.Add(base.gameObject.AddComponent<QuestBookState>(), null);
		_states.Add(base.gameObject.AddComponent<RestorePurchasesState>(), null);
		_states.Add(base.gameObject.AddComponent<RewardState>(), null);
		_states.Add(base.gameObject.AddComponent<SalesmanRickState>(), null);
		_states.Add(base.gameObject.AddComponent<SalesmanRickItemsState>(), null);
		_states.Add(base.gameObject.AddComponent<SalesmanRickCustomizationState>(), null);
		_states.Add(base.gameObject.AddComponent<SettingsState>(), null);
		_states.Add(base.gameObject.AddComponent<LanguageState>(), null);
		_states.Add(base.gameObject.AddComponent<SplashState>(), "SplashScene");
		_states.Add(base.gameObject.AddComponent<StorageState>(), null);
		_states.Add(base.gameObject.AddComponent<StorageCombineState>(), null);
		_states.Add(base.gameObject.AddComponent<StorageEditDeckState>(), null);
		_states.Add(base.gameObject.AddComponent<StorageDecksState>(), null);
		_states.Add(base.gameObject.AddComponent<StorageBootcampState>(), null);
		_states.Add(base.gameObject.AddComponent<StorageBootcampResultsState>(), null);
		_states.Add(base.gameObject.AddComponent<StorageEditBootcampState>(), null);
		_states.Add(base.gameObject.AddComponent<StoryState>(), "StoryScene");
		_states.Add(base.gameObject.AddComponent<TournamentAnnouncerTableState>(), null);
		_states.Add(base.gameObject.AddComponent<TournamentArenaState>(), "WorldScene");
		_states.Add(base.gameObject.AddComponent<TournamentBlackedOutState>(), null);
		_states.Add(base.gameObject.AddComponent<TournamentDirtyRickState>(), null);
		_states.Add(base.gameObject.AddComponent<TournamentEventSelectState>(), null);
		_states.Add(base.gameObject.AddComponent<TournamentPartySelectState>(), null);
		_states.Add(base.gameObject.AddComponent<TrophyCollectionState>(), null);
		_states.Add(base.gameObject.AddComponent<VideoRewardState>(), null);
		_states.Add(base.gameObject.AddComponent<SaveConflictState>(), null);
		_states.Add(base.gameObject.AddComponent<InvestmentState>(), null);
		_states.Add(base.gameObject.AddComponent<MPBagState>(), null);
		_states.Add(base.gameObject.AddComponent<MPBattleState>(), null);
		_states.Add(base.gameObject.AddComponent<MPBlackedOutState>(), null);
		_states.Add(base.gameObject.AddComponent<MPChallengeState>(), null);
		_states.Add(base.gameObject.AddComponent<MPCouponUseState>(), null);
		_states.Add(base.gameObject.AddComponent<MPCouponBuyPopupState>(), null);
		_states.Add(base.gameObject.AddComponent<MPCraftState>(), null);
		_states.Add(base.gameObject.AddComponent<MPCraftRecipeState>(), null);
		_states.Add(base.gameObject.AddComponent<MPFriendState>(), null);
		_states.Add(base.gameObject.AddComponent<MPFriendAddState>(), null);
		_states.Add(base.gameObject.AddComponent<MPFriendDonateMortyState>(), null);
		_states.Add(base.gameObject.AddComponent<MPFriendRequestMortyState>(), null);
		_states.Add(base.gameObject.AddComponent<MPHealingCenterState>(), null);
		_states.Add(base.gameObject.AddComponent<MPLearnAttacksState>(), null);
		_states.Add(base.gameObject.AddComponent<MPMortydexState>(), null);
		_states.Add(base.gameObject.AddComponent<MPMortydexEntryState>(), null);
		_states.Add(base.gameObject.AddComponent<MPMortyLabState>(), null);
		_states.Add(base.gameObject.AddComponent<MPMortyLabPartyState>(), null);
		_states.Add(base.gameObject.AddComponent<MPMortyLabAttacksState>(), null);
		_states.Add(base.gameObject.AddComponent<MPMortyStatsState>(), null);
		_states.Add(base.gameObject.AddComponent<MPPartyState>(), null);
		_states.Add(base.gameObject.AddComponent<MPPauseState>(), null);
		_states.Add(base.gameObject.AddComponent<MPPlayerLevelState>(), null);
		_states.Add(base.gameObject.AddComponent<MPRewardState>(), null);
		_states.Add(base.gameObject.AddComponent<MPRegisterUsernameState>(), null);
		_states.Add(base.gameObject.AddComponent<MPRegisterAvatarState>(), null);
		_states.Add(base.gameObject.AddComponent<MPRegisterMortiesState>(), null);
		_states.Add(base.gameObject.AddComponent<MPStorageState>(), null);
		_states.Add(base.gameObject.AddComponent<MPStorageDecksState>(), null);
		_states.Add(base.gameObject.AddComponent<MPStorageEditDeckState>(), null);
		_states.Add(base.gameObject.AddComponent<MPTutorialPopupState>(), null);
		_states.Add(base.gameObject.AddComponent<MPUserSignInState>(), null);
		_states.Add(base.gameObject.AddComponent<MPSalesmanRickState>(), null);
		_states.Add(base.gameObject.AddComponent<MPSalesmanRickItemsState>(), null);
		_states.Add(base.gameObject.AddComponent<MPSalesmanRickCustomizationState>(), null);
		_states.Add(base.gameObject.AddComponent<MPServerMessageState>(), null);
		_states.Add(base.gameObject.AddComponent<MPStorageCombineState>(), null);
		_states.Add(base.gameObject.AddComponent<MPVideoRewardMeeseekState>(), null);
		_states.Add(base.gameObject.AddComponent<MPVideoRewardState>(), null);
		_states.Add(base.gameObject.AddComponent<MPWorldMapState>(), null);
		_states.Add(base.gameObject.AddComponent<MPWorldSelectState>(), null);
	}

	private void AddStateEvents(bool isAdd)
	{
		StateBase[] array = _states.Keys.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (isAdd)
			{
				array[i].TransitionInCompleteEvent += StateTransitionInCompleteHandler;
				array[i].TransitionOutCompleteEvent += StateTransitionOutCompleteHandler;
			}
			else
			{
				array[i].TransitionInCompleteEvent -= StateTransitionInCompleteHandler;
				array[i].TransitionOutCompleteEvent -= StateTransitionOutCompleteHandler;
			}
		}
	}

	private void OnDestroy()
	{
		AddStateEvents(false);
	}

	public void ManualUpdate(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape) && this.BackButtonEvent != null)
		{
			this.BackButtonEvent();
		}
		_currentState = GetStateAtUILayerIndex(_uiLayerToState.Count - 1);
		if (_currentState != null)
		{
			_currentState.ManualUpdate(deltaTime);
		}
	}

	public void Set<T>(TransInfo transitionOutInfo = null, TransInfo transitionInInfo = null, bool addToBackStack = false, eStateAction stateAction = eStateAction.DEACTIVATE_CURRENT) where T : StateBase
	{
		Set(typeof(T), transitionOutInfo, transitionInInfo, addToBackStack, stateAction);
	}

	public void Set(Type stateType, TransInfo transitionOutInfo = null, TransInfo transitionInInfo = null, bool addToBackStack = false, eStateAction stateAction = eStateAction.DEACTIVATE_CURRENT)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		if (stateAction == eStateAction.DEACTIVATE_ALL)
		{
			_backStack.Clear();
		}
		if (_currentState != null && addToBackStack)
		{
			AddToBackStack(_currentState);
		}
		StartCoroutine(SetCurrentCo(Get(stateType), transitionOutInfo, transitionInInfo, stateAction));
	}

	public IEnumerator Unset(TransInfo transitionOutInfo = null, TransInfo transitionInInfo = null)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		yield return StartCoroutine(UnsetCurrentCo(transitionOutInfo, transitionInInfo));
		_currentState = GetStateAtUILayerIndex(_uiLayerToState.Count - 1);
	}

	public void SetBack(TransInfo transitionOutInfo = null, TransInfo transitionInInfo = null, eStateAction stateAction = eStateAction.DEACTIVATE_CURRENT)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		StateBase stateBase = TakeFromBackStack(_backStack.Count - 1);
		if (stateBase != null)
		{
			StartCoroutine(SetCurrentCo(stateBase, transitionOutInfo, transitionInInfo, stateAction));
		}
	}

	public IEnumerator SetBackFromPopup(TransInfo transitionOutInfo = null, TransInfo transitionInInfo = null)
	{
		RemoveLastBackstack();
		yield return StartCoroutine(Unset(new PopTransInfo()));
		SetBack(transitionOutInfo, transitionInInfo);
	}

	private IEnumerator SetCurrentCo(StateBase stateIn, TransInfo transitionOutInfo, TransInfo transitionInInfo, eStateAction stateAction)
	{
		if (stateIn.IsActive() && stateAction == eStateAction.ADD_CURRENT_TO_CACHE)
		{
			if (!TestDefs.DEBUG_STATE_CONTROLLER)
			{
			}
			yield break;
		}
		_statesWaitingToActivate.Add(stateIn);
		int uiLayerIndex = GetUILayerIndexForNewState(stateAction);
		switch (stateAction)
		{
		case eStateAction.ADD_CURRENT_TO_CACHE:
			if (_currentState != null && _currentState.IsActive())
			{
				yield return StartCoroutine(CacheCo(_currentState, transitionOutInfo));
			}
			break;
		case eStateAction.DEACTIVATE_ALL:
			yield return StartCoroutine(DeactivateAllCo(stateIn, transitionOutInfo));
			break;
		case eStateAction.DEACTIVATE_CURRENT:
			yield return StartCoroutine(DeactivateCo(_currentState, transitionOutInfo));
			break;
		}
		if (stateIn.status == eStateStatus.CACHED)
		{
			stateIn.ShowUILayer();
			stateIn.TransitionSetUp(transitionInInfo);
		}
		else if (_uiLayerToState.Count >= uiLayerIndex)
		{
			_uiLayerToState[uiLayerIndex].Key.gameObject.SetActive(true);
			if (stateIn.showLoadingScreen)
			{
				_uiLayerToState[_uiLayerToState.Count - 1].Key.ShowLoadingScreen(true);
			}
		}
		yield return new WaitForEndOfFrame();
		yield return Resources.UnloadUnusedAssets();
		GC.Collect();
		App.sound.UnloadNonPlayingSFX();
		yield return new WaitForEndOfFrame();
		if (_statesWaitingToActivate.Contains(stateIn))
		{
			_currentState = stateIn;
			if (stateIn.status == eStateStatus.CACHED)
			{
				stateIn.Uncache(transitionInInfo);
			}
			else
			{
				if (!string.IsNullOrEmpty(_states[stateIn]))
				{
					yield return StartCoroutine(SwitchScenes(_states[stateIn]));
				}
				_uiLayerToState[uiLayerIndex].Key.Reset();
				ActivateState(stateIn, uiLayerIndex, transitionInInfo, stateAction);
			}
			_statesWaitingToActivate.Remove(stateIn);
		}
		else if (!TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		if (_statesWaitingToActivate.Count == 0)
		{
			DestoryUnusedUILayers();
		}
		RefreshActiveUILayers();
		stateIn.SetAsCurrentComplete();
	}

	private IEnumerator UnsetCurrentCo(TransInfo transOutInfo, TransInfo transInInfo)
	{
		if (_currentState == null)
		{
			yield break;
		}
		StateBase unsetState = _currentState;
		int uiLayerIndex = GetUILayerIndexForState(unsetState);
		yield return StartCoroutine(DeactivateCo(unsetState, transOutInfo));
		DestoryUnusedUILayers();
		if (uiLayerIndex >= _uiLayerToState.Count - 1)
		{
			StateBase stateBelow = GetStateAtUILayerIndex(uiLayerIndex - 1);
			if (stateBelow != null && stateBelow.status == eStateStatus.CACHED)
			{
				stateBelow.Uncache(transInInfo);
			}
		}
	}

	private void ActivateState(StateBase state, int uiLayerIndex, TransInfo transitionInInfo, eStateAction stateAction)
	{
		if (state.IsActive())
		{
			if (!TestDefs.DEBUG_STATE_CONTROLLER)
			{
			}
			return;
		}
		if (uiLayerIndex >= _uiLayerToState.Count)
		{
			uiLayerIndex = GetUILayerIndexForNewState(stateAction);
		}
		_uiLayerToState[uiLayerIndex] = new KeyValuePair<UILayer, StateBase>(_uiLayerToState[uiLayerIndex].Key, state);
		state.Activate(_uiLayerToState[uiLayerIndex].Key.uiContainerRectTrans, transitionInInfo);
	}

	private IEnumerator CacheCo(StateBase state, TransInfo transitionOutInfo)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER && !(state == null))
		{
			state.GetType().ToString();
		}
		if (!(state == null))
		{
			if (!_statesWaitingToCache.Contains(state))
			{
				_statesWaitingToCache.Add(state);
			}
			state.Cache(transitionOutInfo);
			while (_statesWaitingToCache.Count > 0 && transitionOutInfo != null)
			{
				yield return null;
			}
		}
	}

	private IEnumerator DeactivateAllCo(StateBase stateIn, TransInfo transitionOutInfo)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER && !(_currentState == null))
		{
			_currentState.GetType().ToString();
		}
		_statesWaitingToDeactivate.Clear();
		_statesWaitingToActivate.Clear();
		_statesWaitingToCache.Clear();
		_statesWaitingToActivate.Add(stateIn);
		for (int i = 0; i < _uiLayerToState.Count; i++)
		{
			StateBase stateAtUILayerIndex = GetStateAtUILayerIndex(i);
			if (stateAtUILayerIndex != null)
			{
				stateAtUILayerIndex.StopTransition();
			}
		}
		yield return StartCoroutine(DeactivateCo(_currentState, transitionOutInfo));
		for (int j = 0; j < _uiLayerToState.Count; j++)
		{
			StateBase stateAtUILayerIndex2 = GetStateAtUILayerIndex(j);
			if (stateAtUILayerIndex2 != null && stateAtUILayerIndex2.status != eStateStatus.DEACTIVE)
			{
				if (!_statesWaitingToDeactivate.Contains(stateAtUILayerIndex2))
				{
					_statesWaitingToDeactivate.Add(stateAtUILayerIndex2);
				}
				stateAtUILayerIndex2.Deactivate(null);
			}
		}
		yield return new WaitForEndOfFrame();
	}

	private IEnumerator DeactivateCo(StateBase state, TransInfo transitionOutInfo)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER && !(state == null))
		{
			state.GetType().ToString();
		}
		if (!(state == null) && !state.IsTransitioningOut())
		{
			if (!_statesWaitingToDeactivate.Contains(state))
			{
				_statesWaitingToDeactivate.Add(state);
			}
			state.Deactivate(transitionOutInfo);
			while (_statesWaitingToDeactivate.Count > 0)
			{
				yield return null;
			}
		}
	}

	private void AddToBackStack(StateBase state)
	{
		if (_backStack.Count >= 20)
		{
			_backStack.RemoveAt(0);
		}
		_backStack.Add(state);
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
			DebugBackStack();
		}
	}

	private StateBase TakeFromBackStack(int index)
	{
		if (_backStack.Count <= index)
		{
			return null;
		}
		StateBase result = _backStack[index];
		_backStack.RemoveAt(index);
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
			DebugBackStack();
		}
		return result;
	}

	public void RemoveLastBackstack()
	{
		_backStack.RemoveAt(_backStack.Count - 1);
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
			DebugBackStack();
		}
	}

	private void DebugBackStack()
	{
		string text = string.Empty;
		for (int i = 0; i < _backStack.Count; i++)
		{
			text = string.Concat(text, _backStack[i].GetType(), " > ");
		}
	}

	private int GetUILayerIndexForNewState(eStateAction action)
	{
		if (_uiLayerToState.Count == 0)
		{
			return CreateNewUILayer();
		}
		switch (action)
		{
		case eStateAction.ADD_CURRENT_TO_CACHE:
			return CreateNewUILayer();
		case eStateAction.DEACTIVATE_ALL:
			return 0;
		case eStateAction.DEACTIVATE_CURRENT:
			return _uiLayerToState.Count - 1;
		default:
			return -1;
		}
	}

	private int CreateNewUILayer()
	{
		Transform transform = UI.Instantiate(string.Format("UI/Utils/{0}", "UILayer"), mainCanvas.transform, eUISibling.LAST);
		transform.gameObject.name = "UILayer (" + _uiLayerToState.Count + ")";
		_uiLayerToState.Add(new KeyValuePair<UILayer, StateBase>(transform.GetComponent<UILayer>(), null));
		return _uiLayerToState.Count - 1;
	}

	public UILayer GetUILayerForState(StateBase state)
	{
		for (int i = 0; i < _uiLayerToState.Count; i++)
		{
			if (_uiLayerToState[i].Value == state)
			{
				return _uiLayerToState[i].Key;
			}
		}
		return null;
	}

	private int GetUILayerIndexForState(StateBase state)
	{
		for (int i = 0; i < _uiLayerToState.Count; i++)
		{
			if (_uiLayerToState[i].Value == state)
			{
				return i;
			}
		}
		return -1;
	}

	private StateBase GetStateAtUILayerIndex(int uiLayerIndex)
	{
		if (uiLayerIndex >= 0 && uiLayerIndex < _uiLayerToState.Count)
		{
			return _uiLayerToState[uiLayerIndex].Value;
		}
		return null;
	}

	private void RefreshActiveUILayers()
	{
		bool flag = false;
		bool flag2 = false;
		for (int num = _uiLayerToState.Count - 1; num >= 0; num--)
		{
			if (num == _uiLayerToState.Count - 1)
			{
				_uiLayerToState[num].Value.ShowUILayer();
			}
			else if (flag2 || flag)
			{
				_uiLayerToState[num].Key.SetTransitionFade(null);
				_uiLayerToState[num].Value.HideUILayer();
			}
			else
			{
				_uiLayerToState[num].Value.ShowUILayer();
			}
			flag = _uiLayerToState[num].Value.hideStateBellow;
			if (flag)
			{
				flag2 = true;
			}
		}
	}

	private void DestoryUnusedUILayers()
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		for (int num = _uiLayerToState.Count - 1; num >= 1; num--)
		{
			if (_uiLayerToState[num].Value == null || _uiLayerToState[num].Value.status == eStateStatus.DEACTIVE)
			{
				_uiLayerToState[num].Key.DestroySelf();
				_uiLayerToState.RemoveAt(num);
			}
		}
	}

	public T Get<T>() where T : StateBase
	{
		KeyValuePair<StateBase, string> keyValuePair = _states.FirstOrDefault((KeyValuePair<StateBase, string> e) => e.Key.GetType() == typeof(T));
		if (!keyValuePair.Equals(default(KeyValuePair<StateBase, string>)))
		{
			return keyValuePair.Key as T;
		}
		return (T)null;
	}

	public StateBase Get(Type stateType)
	{
		KeyValuePair<StateBase, string> keyValuePair = _states.FirstOrDefault((KeyValuePair<StateBase, string> e) => e.Key.GetType() == stateType);
		if (!keyValuePair.Equals(default(KeyValuePair<StateBase, string>)))
		{
			return keyValuePair.Key;
		}
		return null;
	}

	private IEnumerator SwitchScenes(string levelName)
	{
		if (levelName != null)
		{
			yield return SceneManager.LoadSceneAsync(levelName);
		}
	}

	public void TransitionFadeSetUp(StateBase state, FadeTransInfo info)
	{
		UILayer uILayerForState = GetUILayerForState(state);
		uILayerForState.SetTransitionFade(info.color);
	}

	public IEnumerator TransitionFade(StateBase state, eTransitionDirection transitionDirection, FadeTransInfo info)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		UILayer uiLayer = GetUILayerForState(state);
		yield return StartCoroutine(uiLayer.TransitionFade(transitionDirection, info));
	}

	public void BackgroundFade(StateBase state, eTransitionDirection transitionDirection, Color color, float duration)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		UILayer uILayerForState = GetUILayerForState(state);
		uILayerForState.BackgroundFade(transitionDirection, color, duration);
	}

	private void StateTransitionInCompleteHandler(StateBase state)
	{
		if (GetUILayerIndexForState(state) < _uiLayerToState.Count - 1)
		{
			StartCoroutine(CacheCo(state, null));
		}
	}

	private void StateTransitionOutCompleteHandler(StateBase state)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		_statesWaitingToDeactivate.Remove(state);
		_statesWaitingToCache.Remove(state);
	}

	public void FlashCurrentState(float outDuration = 0f)
	{
		UILayer uILayerForState = GetUILayerForState(_currentState);
		uILayerForState.Flash(outDuration);
	}
}
