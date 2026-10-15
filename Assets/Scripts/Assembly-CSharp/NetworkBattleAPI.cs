using System;
using System.Collections.Generic;
using JsonFx.Json;
using UnityEngine;

public class NetworkBattleAPI : MonoBehaviour
{
	public delegate void BattleStartedDelegate(string battleType, MPOpponentData opponent);

	public delegate void BattleTimerStartDelegate(MPBattleTimerData data);

	public delegate void BattleComeBackDelegate();

	public delegate void BattleMoveSelectDelegate();

	public delegate void BattleMoveSelectFinishedDelegate(eBattleMoveSelectError error);

	public delegate void BattleTurnResultDelegate();

	private const float kTimeOut = 180f;

	public const string kBattleTypePVP = "PvP";

	public const string kBattleTypePVF = "PvF";

	public const string kBattleTypePVWM = "PvWM";

	public eMPBattleTurnResult turnResult;

	public List<MPBattleActData> turnActDatas;

	public MPBattleTurnOpponentData turnPlayerData;

	public MPBattleTurnOpponentData turnEnemyData;

	private float timeOutCount;

	public string id { get; private set; }

	public MPOpponentData enemy { get; private set; }

	public MPBattleTimerData timerData { get; private set; }

	public bool isTimerExpired { get; private set; }

	public bool isTimerSkipped { get; private set; }

	public bool isTimerExpiredOrSkipped
	{
		get
		{
			return isTimerExpired || isTimerSkipped;
		}
	}

	public event BattleStartedDelegate StartEvent;

	public event BattleTimerStartDelegate MoveTimerStartedEvent;

	public event BattleComeBackDelegate AbandonedEvent;

	public event BattleMoveSelectDelegate MoveSelectedEvent;

	public event BattleMoveSelectFinishedDelegate MoveSelectFinishedEvent;

	public event BattleTurnResultDelegate TurnResultEvent;

	public void Disconnected()
	{
		DebugLog("Disconnected()");
		Leave();
	}

	private void Update()
	{
		UpdateTimeOut();
		UpdateBattleTurnTimer();
	}

	private void StartBattle(string battleType, MPOpponentData enemyData)
	{
		SetActiveAvailableHandlers(false);
		SetActiveInBattleHandlers(true);
		enemy = enemyData;
		turnResult = eMPBattleTurnResult.NONE;
		turnActDatas = null;
		turnPlayerData = null;
		turnEnemyData = null;
		if (this.StartEvent != null)
		{
			this.StartEvent(battleType, enemyData);
		}
	}

	private void EndBattle()
	{
		id = null;
		enemy = null;
		timerData = null;
		isTimerExpired = false;
		isTimerSkipped = false;
		SetActiveInBattleHandlers(false);
	}

	public void SelectingMove()
	{
		DebugLog(string.Format("SelectingMove()"));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/battle/ready";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			battle_id = id
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eBattleReadyError eBattleReadyError2 = NetworkDefs.ParseHTTPError<eBattleReadyError>(request, response);
			if (eBattleReadyError2 == eBattleReadyError.NONE)
			{
				StartTimeOut();
			}
			else
			{
				App.net.session.Disconnect(eNetSessionDisconnectType.BATTLE_READY_FAILED);
			}
		});
	}

	public void SelectMove(eMPBattleMoveType type, string mortyNetId, string moveId)
	{
		DebugLog(string.Format("SelectMove(type = {0}, mortyNetId = {1}, moveId = {2})", type, mortyNetId, moveId));
		if (this.MoveSelectedEvent != null)
		{
			this.MoveSelectedEvent();
		}
		timerData = null;
		isTimerSkipped = true;
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/battle/move";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			battle_id = id,
			type = type.ToString(),
			owned_morty_id = mortyNetId,
			move_id = moveId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eBattleMoveSelectError eBattleMoveSelectError2 = NetworkDefs.ParseHTTPError<eBattleMoveSelectError>(request, response);
			if (eBattleMoveSelectError2 == eBattleMoveSelectError.NONE && type == eMPBattleMoveType.ITEM)
			{
				App.upsight.CustomAnalyticEvent(eSubType1.ITEMS, eSubType2.USE, moveId);
			}
			if (this.MoveSelectFinishedEvent != null)
			{
				this.MoveSelectFinishedEvent(eBattleMoveSelectError2);
			}
		});
	}

	public void UpdateBattleTurnTimer()
	{
		if (timerData != null && !timerData.ManualUpdate())
		{
			timerData = null;
			isTimerExpired = true;
		}
	}

	private void StartTimeOut()
	{
		StopTimeOut();
		timeOutCount = 180f;
	}

	private void StopTimeOut()
	{
		timeOutCount = 0f;
	}

	private void UpdateTimeOut()
	{
		if (timeOutCount > 0f)
		{
			timeOutCount = Mathf.Max(0f, timeOutCount - Time.deltaTime);
			if (timeOutCount <= 0f)
			{
				App.net.session.Disconnect(eNetSessionDisconnectType.BATTLE_TIMED_OUT);
			}
		}
	}

	public void Leave()
	{
		App.net.session.Off("battle:start", BattleStartHandler);
		App.net.session.Off("battle:move-timer-started", BattleMoveTimeHandler);
		App.net.session.Off("battle:turn-result", BattleTurnResultHandler);
		id = null;
		enemy = null;
		timerData = null;
		isTimerExpired = false;
		isTimerSkipped = false;
	}

	public void SetActiveAvailableHandlers(bool isAvailable)
	{
		DebugLog(string.Format("SetActiveAvailableHandlers({0})", isAvailable));
		if (isAvailable)
		{
			App.net.session.On("battle:start", BattleStartHandler);
		}
		else
		{
			App.net.session.Off("battle:start", BattleStartHandler);
		}
	}

	public void SetActiveInBattleHandlers(bool isAvailable)
	{
		DebugLog(string.Format("SetActiveInBattleHandlers({0})", isAvailable));
		if (isAvailable)
		{
			App.net.session.On("battle:move-timer-started", BattleMoveTimeHandler);
			App.net.session.On("battle:turn-result", BattleTurnResultHandler);
		}
		else
		{
			App.net.session.Off("battle:move-timer-started", BattleMoveTimeHandler);
			App.net.session.Off("battle:turn-result", BattleTurnResultHandler);
		}
	}

	private void BattleStartHandler(NetworkEventSourceResponse response)
	{
		BattleStartResponse battleStartResponse = JsonReader.Deserialize<BattleStartResponse>(response.jsonStr);
		id = battleStartResponse.battle_id;
		App.net.player.data.SetFromDict(battleStartResponse.player);
		MPOpponentData mPOpponentData = null;
		switch (battleStartResponse.battle_type)
		{
		default:
			mPOpponentData = new MPUserData(battleStartResponse.opponent);
			mPOpponentData.type = eMPOpponentType.USER;
			break;
		case "PvWM":
			mPOpponentData = new MPOpponentData(battleStartResponse.opponent);
			mPOpponentData.type = eMPOpponentType.WILD_MORTY;
			break;
		}
		StartBattle(battleStartResponse.battle_type, mPOpponentData);
	}

	public void BattleMoveTimeHandler(NetworkEventSourceResponse response)
	{
		BattleMoveTimeResponse battleMoveTimeResponse = JsonReader.Deserialize<BattleMoveTimeResponse>(response.jsonStr);
		timerData = new MPBattleTimerData(battleMoveTimeResponse.timeout);
		if (this.MoveTimerStartedEvent != null)
		{
			this.MoveTimerStartedEvent(timerData);
		}
	}

	public void BattleTurnResultHandler(NetworkEventSourceResponse response)
	{
		BattleTurnResultResponse battleTurnResultResponse = JsonReader.Deserialize<BattleTurnResultResponse>(response.jsonStr);
		if (battleTurnResultResponse.battle_abandoned && battleTurnResultResponse.outcome == eMPBattleTurnResult.WIN)
		{
			turnResult = eMPBattleTurnResult.ABANDONED;
		}
		else
		{
			turnResult = battleTurnResultResponse.outcome;
		}
		timerData = null;
		isTimerExpired = false;
		isTimerSkipped = false;
		StopTimeOut();
		turnActDatas = new List<MPBattleActData>();
		for (int i = 0; i < battleTurnResultResponse.turn_datas.Length; i++)
		{
			switch ((eMPBattleActType)Enum.Parse(typeof(eMPBattleActType), Convert.ToString(battleTurnResultResponse.turn_datas[i]["type"])))
			{
			default:
				turnActDatas.Add(new MPBattleActData(battleTurnResultResponse.turn_datas[i]));
				break;
			case eMPBattleActType.ATTACK:
				turnActDatas.Add(new MPBattleActAttackData(battleTurnResultResponse.turn_datas[i]));
				break;
			case eMPBattleActType.ITEM:
				turnActDatas.Add(new MPBattleActItemData(battleTurnResultResponse.turn_datas[i]));
				break;
			case eMPBattleActType.SWITCH:
				turnActDatas.Add(new MPBattleActSwitchData(battleTurnResultResponse.turn_datas[i]));
				break;
			case eMPBattleActType.POISONED:
				turnActDatas.Add(new MPBattleActData(battleTurnResultResponse.turn_datas[i]));
				break;
			case eMPBattleActType.PARALYSED:
				turnActDatas.Add(new MPBattleActParalysedData(battleTurnResultResponse.turn_datas[i]));
				break;
			}
		}
		Dictionary<string, object> dict = (Dictionary<string, object>)battleTurnResultResponse.player_datas["player"];
		turnPlayerData = new MPBattleTurnOpponentData(dict);
		Dictionary<string, object> dict2 = (Dictionary<string, object>)battleTurnResultResponse.player_datas["opponent"];
		turnEnemyData = new MPBattleTurnOpponentData(dict2);
		if (turnResult == eMPBattleTurnResult.WIN || turnResult == eMPBattleTurnResult.LOSE || turnResult == eMPBattleTurnResult.DRAW || turnResult == eMPBattleTurnResult.CAUGHT || turnResult == eMPBattleTurnResult.RUN || turnResult == eMPBattleTurnResult.ABANDONED)
		{
			EndBattle();
		}
		if (turnPlayerData.videoRewardMeeseeksData != null)
		{
			App.fyber.RequestVideo(turnPlayerData.videoRewardMeeseeksData);
		}
		if (this.TurnResultEvent != null)
		{
			this.TurnResultEvent();
		}
		if (turnResult == eMPBattleTurnResult.ABANDONED && this.AbandonedEvent != null)
		{
			this.AbandonedEvent();
		}
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_BATTLE_API)
		{
		}
	}
}
