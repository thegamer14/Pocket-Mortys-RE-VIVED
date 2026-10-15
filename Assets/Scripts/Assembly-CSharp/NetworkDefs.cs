using System;
using System.Collections.Generic;
using BestHTTP;
using JsonFx.Json;

public class NetworkDefs
{
	public const string ENVIRONMENT = "applecrumble";

	public const string STATIC_ENDPOINT_UNFORMATED = "http://static.bps-pmnet.com/init";

	public const string GAME_ENDPOINT_UNFORMATE = "https://{0}.game.bps-pmnet.com";

	public const int MAX_NODE_STATE_SEQUENCE_NUM = 255;

	public const string HEADER_KEY = "X-PMCLIENT";

	public const string HEADER_VALUE = "{0}/v2.0.3";

	public const int LOAD_BALANCER_FAIL_STATUS_CODE = 503;

	public const int LOAD_BALANCER_FAIL_RETRY_COUNT = 3;

	public const float LOAD_BALANCER_FAIL_RETRY_TIME = 0f;

	public const double HTTP_REQUEST_DELAYED_TIME = 0.5;

	public const float SESSION_KEEPALIVE_TIMEOUT_MULT = 3f;

	public const string WWW_SERVER_MESSAGE = "{0}.json";

	public const string EP_PLAYER_LOGIN = "/user/login";

	public const string EP_PLAYER_REGISTER = "/user/register";

	public const string EP_PLAYER_DETAILS = "/session/player-details";

	public const string EP_PLAYER_SET_ACTIVE_DECK = "/session/deck/set-active";

	public const string EP_PLAYER_EDIT_DECK = "/session/deck/edit";

	public const string EP_PLAYER_HEAL_DECK = "/session/heal";

	public const string EP_PLAYER_EDIT_ATTACKS = "/session/owned_morties/learn-attacks";

	public const string EP_PLAYER_RELEASE = "/session/owned_morties/release";

	public const string EP_PLAYER_USE_ITEM = "/session/use-item";

	public const string EP_PLAYER_BUY_ITEM = "/session/buy-item";

	public const string EP_PLAYER_LEARN_RECIPE = "/session/recipe/learn";

	public const string EP_PLAYER_CRAFT_RECIPE = "/session/recipe/use";

	public const string EP_PLAYER_USE_GACHA = "/session/gacha";

	public const string EP_PLAYER_STATE = "/session/state";

	public const string EP_PLAYER_IAP = "/iap";

	public const string EP_PLAYER_BUY_AVATAR = "/session/player-avatar/buy";

	public const string EP_PLAYER_USE_AVATAR = "/session/player-avatar/set-active";

	public const string EP_PLAYER_MIGRATE_AVATARS = "/session/player-avatar/migrate";

	public const string EP_PLAYER_ABSORB_MORTIES = "/session/owned_morties/absorb";

	public const string EP_PLAYER_GET_CHALLENGES = "/session/challenge/list";

	public const string EP_PLAYER_CLAIM_CHALLENGE = "/session/challenge/claim-reward";

	public const string EP_PLAYER_VIDEO_REWARD_PASS = "/session/incentive/reject";

	public const string ES_PLAYER_CHALLENGES_COMPLETE = "challenge:complete";

	public const string EP_FRIEND_REFRESH_DATA = "/session/friend/list";

	public const string EP_FRIEND_SEARCH_FOR_USER = "/session/friend/search";

	public const string EP_FRIEND_SEND_REQUEST = "/session/friend/request";

	public const string EP_FRIEND_RESPOND_TO_REQUEST = "/session/friend/response";

	public const string EP_FRIEND_REMOVE = "/session/friend/remove";

	public const string EP_FRIEND_REQUEST_MORTY = "/session/friend/request-morty";

	public const string EP_FRIEND_DONATE_MORTY = "/session/friend/donate-morty";

	public const string ES_FRIEND_DONATION_MORTY_RECEIVED = "friend:donation";

	public const string EP_SESSION_EMOTE_TO_USER = "/session/emote/p2p";

	public const string EP_SESSION_EMOTE_TO_ROOM = "/session/emote/room";

	public const string ES_SESSION_START = "session:start";

	public const string ES_SESSION_END = "session:end";

	public const string ES_SESSION_EMOTE_DIRECT = "emote:p2p";

	public const string ES_SESSION_EMOTE = "emote:room";

	public const string EP_ROOM_JOIN = "/session/join-room";

	public const string EP_ROOM_LEAVE = "/session/leave-room";

	public const string EP_ROOM_DETAILS = "/session/room-details";

	public const string ES_ROOM_USER_ADDED = "room:user-added";

	public const string ES_ROOM_USER_REMOVED = "room:user-removed";

	public const string ES_ROOM_USER_STATE_CHANGED = "room:user-state-changed";

	public const string ES_ROOM_USER_MODIFIED = "room:user-modified";

	public const string ES_ROOM_WILD_MORTY_ADDED = "room:wild-morty-added";

	public const string ES_ROOM_WILD_MORTY_REMOVED = "room:wild-morty-removed";

	public const string ES_ROOM_WILD_MORTY_STATE_CHANGED = "room:wild-morty-state-changed";

	public const string EP_ROOM_ITEM_PICK_UP = "/session/collect-pickup";

	public const string ES_ROOM_PICKUP_ADDED = "room:pickup-added";

	public const string ES_ROOM_PICKUP_REMOVED = "room:pickup-removed";

	public const string EP_BATTLE_INVITE_REQUEST = "/session/battle-invite/create";

	public const string EP_BATTLE_INVITE_REPLY = "/session/battle-invite/response";

	public const string EP_BATTLE_INVITE_REVOKE = "/session/battle-invite/revoke";

	public const string EP_BATTLE_WILD_MORTY = "/session/battle-wild-morty";

	public const string ES_BATTLE_INVITE_RECEIVED = "battle-invite:received";

	public const string ES_BATTLE_INVITE_DECLINED = "battle-invite:declined";

	public const string ES_BATTLE_INVITE_REVOKED = "battle-invite:revoked";

	public const string EP_BATTLE_SELECTING_MOVE = "/session/battle/ready";

	public const string EP_BATTLE_MOVE_SELECTED = "/session/battle/move";

	public const string ES_BATTLE_START = "battle:start";

	public const string ES_BATTLE_ABANDONED = "battle:abandoned";

	public const string ES_BATTLE_MOVE_TIMER = "battle:move-timer-started";

	public const string ES_BATTLE_TURN_RESULT = "battle:turn-result";

	public const string ES_VIDEO_REWARD_RECEIVED = "fyber:reward";

	public const string ES_VIDEO_REWARD_MEESEEK_RECEIVED = "incentive:reward";

	public static string gameEndPoint { get; private set; }

	public static void SetEnvironmentEndPoints()
	{
		gameEndPoint = string.Format("https://{0}.game.bps-pmnet.com", "applecrumble");
	}

	public static eErrorType ParseHTTPResponse<eErrorType, ResponseType>(NetworkHTTPRequest request, NetworkHTTPResponse response, out ResponseType responseObject) where eErrorType : struct, IConvertible where ResponseType : class
	{
		Type typeFromHandle = typeof(eErrorType);
		eErrorType val = default(eErrorType);
		responseObject = (ResponseType)null;
		val = ParseHTTPError<eErrorType>(request, response);
		if (Enum.IsDefined(typeFromHandle, "NONE") && val.Equals(Enum.Parse(typeFromHandle, "NONE", true)))
		{
			try
			{
				responseObject = (ResponseType)JsonReader.Deserialize(response.jsonStr, typeof(ResponseType));
			}
			catch (Exception)
			{
				responseObject = (ResponseType)null;
				if (Enum.IsDefined(typeFromHandle, "INVALID_RESPONSE"))
				{
					val = (eErrorType)Enum.Parse(typeFromHandle, "INVALID_RESPONSE", true);
				}
			}
		}
		return val;
	}

	public static eErrorType ParseHTTPError<eErrorType>(NetworkHTTPRequest request, NetworkHTTPResponse response) where eErrorType : struct, IConvertible
	{
		Type typeFromHandle = typeof(eErrorType);
		eErrorType result = default(eErrorType);
		if (response == null)
		{
			if (Enum.IsDefined(typeFromHandle, "NO_RESPONSE"))
			{
				result = (eErrorType)Enum.Parse(typeFromHandle, "NO_RESPONSE", true);
			}
		}
		else if (!response.isSuccess)
		{
			try
			{
				string value = (JsonReader.Deserialize<Dictionary<string, object>>(response.jsonStr)["error"] as Dictionary<string, object>)["code"] as string;
				if (Enum.IsDefined(typeFromHandle, value))
				{
					result = (eErrorType)Enum.Parse(typeFromHandle, value, true);
				}
			}
			catch (Exception)
			{
			}
		}
		else if (Enum.IsDefined(typeFromHandle, "NONE"))
		{
			result = (eErrorType)Enum.Parse(typeFromHandle, "NONE", true);
		}
		return result;
	}

	public static eHTTPReponseState HTTPRequestStateToNetworkReponseState(HTTPRequestStates state)
	{
		switch (state)
		{
		case HTTPRequestStates.Initial:
			return eHTTPReponseState.Initial;
		case HTTPRequestStates.Queued:
			return eHTTPReponseState.Queued;
		case HTTPRequestStates.Processing:
			return eHTTPReponseState.Processing;
		case HTTPRequestStates.Finished:
			return eHTTPReponseState.Finished;
		case HTTPRequestStates.Error:
			return eHTTPReponseState.Error;
		case HTTPRequestStates.Aborted:
			return eHTTPReponseState.Aborted;
		case HTTPRequestStates.ConnectionTimedOut:
			return eHTTPReponseState.ConnectionTimedOut;
		case HTTPRequestStates.TimedOut:
			return eHTTPReponseState.TimedOut;
		default:
			return eHTTPReponseState.Unkown;
		}
	}

	public static string CombineUri(params string[] uriParts)
	{
		string text = string.Empty;
		if (uriParts != null && uriParts.Length > 0)
		{
			char[] trimChars = new char[2] { '\\', '/' };
			text = (uriParts[0] ?? string.Empty).TrimEnd(trimChars);
			for (int i = 1; i < uriParts.Length; i++)
			{
				text = string.Format("{0}/{1}", text.TrimEnd(trimChars), (uriParts[i] ?? string.Empty).TrimStart(trimChars));
			}
		}
		return text;
	}
}
