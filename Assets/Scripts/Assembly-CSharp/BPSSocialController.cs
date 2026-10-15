using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Prime31;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BPSSocialController : MonoBehaviour
{
	public delegate void AuthenticateDelegate(bool success);

	public delegate void SubmitScoreDelegate(string leaderboardID);

	public delegate void LeaderboardLoadedCallback(bool success, ILeaderboard board);

	private struct LoadScoreRequest
	{
		public string leaderboardId;

		public LeaderboardLoadedCallback callbackPointer;
	}

	private const int kMaxPlayersLoadedFromLeaderboard = 10;

	private const string _kScreenshotFileName = "tweetScreenShot.png";

	public bool enableDebug;

	protected List<BPSSocialScoreInfo> _loadedUserScoreInfos;

	protected List<string> _completedAchievements = new List<string>();

	protected string _lastSubmissionLeaderboardId;

	private bool _loadScoreRequestPending;

	private List<LoadScoreRequest> _loadScoreRequests = new List<LoadScoreRequest>();

	private string _screenshotPath;

	private bool isProcessingShare;

	public bool isInitialized { get; private set; }

	public event AuthenticateDelegate AuthenticateEvent;

	public event SubmitScoreDelegate ScoreSubmittedSuccessEvent;

	public event SubmitScoreDelegate ScoreSubmittedFailedEvent;

	[DllImport("__Internal")]
	public static extern void _ReportAchievement(string achievementID, float progress);

	private void Awake()
	{
		enableDebug = TestDefs.DEBUG_BPS_SOCIAL;
		DebugLog("Awake()");
		_screenshotPath = Application.persistentDataPath + "/tweetScreenShot.png";
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			AwakeIOS();
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			AwakeAndroid();
		}
		AddEvents();
		isInitialized = true;
	}

	private void AwakeIOS()
	{
	}

	private void AwakeAndroid()
	{
		DebugLog("AwakeAndroid()");
		PlayGamesPlatform.DebugLogEnabled = TestDefs.DEBUG_GOOGLE_PLAY_GAMES;
		PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		PlayGamesPlatform.InitializeInstance(configuration);
		PlayGamesPlatform.Activate();
	}

	private void Start()
	{
		StartCoroutine(DeleteScreenshot());
	}

	public void Authenticate()
	{
		DebugLog("Authenticate()");
		if (Application.platform != RuntimePlatform.Android)
		{
			isInitialized = false;
			AuthenticateDelegate ev = AuthenticateEvent;
			if (ev != null)
			{
				ev(false);
			}
			return;
		}
		PlayGamesPlatform.DebugLogEnabled = enableDebug;
		PlayGamesPlatform.Instance.Authenticate(AuthenticationHandler);
	}

	public bool IsAuthenticated()
	{
		return isInitialized && Social.localUser.authenticated;
	}

	private void AddEvents()
	{
		TwitterManager.tweetSheetCompletedEvent += TweetSheetCompletedHandler;
	}

	private void RemoveEvents()
	{
		TwitterManager.tweetSheetCompletedEvent -= TweetSheetCompletedHandler;
	}

	private void OnDestroy()
	{
		DebugLog("OnDestroy()");
		RemoveEvents();
	}

	protected void DebugLog(string str)
	{
		if (enableDebug)
		{
		}
	}

	public void Share(string text, bool withScreenshot = false)
	{
		AndroidShare(text, withScreenshot);
	}

	public IEnumerator TakeTwitterScreenshot()
	{
		yield return StartCoroutine(DeleteScreenshot());
		ScreenCapture.CaptureScreenshot("tweetScreenShot.png");
		float timeout = 5f;
		while (!File.Exists(_screenshotPath))
		{
			DebugLog("Saving... " + timeout);
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				DebugLog("Screenshot save timed out");
				break;
			}
			yield return null;
		}
		DebugLog("Screenshot exists:" + File.Exists(_screenshotPath));
	}

	private IEnumerator DeleteScreenshot()
	{
		DebugLog("DeleteScreenshot()");
		if (File.Exists(_screenshotPath))
		{
			DebugLog("Found a screenshot to delete, deleting it");
			File.Delete(_screenshotPath);
			float timeout = 5f;
			while (File.Exists(_screenshotPath))
			{
				DebugLog("Deleting... " + timeout);
				timeout -= Time.deltaTime;
				if (timeout <= 0f)
				{
					DebugLog("Screenshot delete timed out");
					yield break;
				}
				yield return null;
			}
		}
		if (File.Exists(_screenshotPath))
		{
			DebugLog("Screenshot failed to delete");
		}
		else
		{
			DebugLog("Screenshot succesfully deleted");
		}
	}

	private void AndroidShare(string text, bool withScreenshot)
	{
		if (!isProcessingShare)
		{
			StartCoroutine(AndroidShareCo(text, withScreenshot));
		}
	}

	private IEnumerator AndroidShareCo(string text, bool withScreenshot)
	{
		DebugLog("SocialController: AndroidShare - " + text + ", withScreenshot: " + withScreenshot);
		isProcessingShare = true;
		yield return new WaitForEndOfFrame();
		if (withScreenshot)
		{
			yield return StartCoroutine(TakeTwitterScreenshot());
		}
		if (!Application.isEditor)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.content.Intent");
			androidJavaObject.Call<AndroidJavaObject>("setAction", new object[1] { androidJavaClass.GetStatic<string>("ACTION_SEND") });
			if (withScreenshot)
			{
				AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.net.Uri");
				AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("parse", new object[1] { "file://" + _screenshotPath });
				androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
				{
					androidJavaClass.GetStatic<string>("EXTRA_STREAM"),
					androidJavaObject2
				});
			}
			androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
			{
				androidJavaClass.GetStatic<string>("EXTRA_TEXT"),
				text
			});
			androidJavaObject.Call<AndroidJavaObject>("setType", new object[1] { "image/jpeg" });
			AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject3 = androidJavaClass3.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject4 = androidJavaClass.CallStatic<AndroidJavaObject>("createChooser", new object[2] { androidJavaObject, "Share Via" });
			androidJavaObject3.Call("startActivity", androidJavaObject4);
		}
		isProcessingShare = false;
	}

	private void IosShare(string text, bool withScreenshot)
	{
		if (!isProcessingShare)
		{
			StartCoroutine(IosShareCo(text, withScreenshot));
		}
	}

	private IEnumerator IosShareCo(string text, bool withScreenshot)
	{
		DebugLog("SocialController: IosShare - " + text + ", withScreenshot: " + withScreenshot);
		isProcessingShare = true;
		yield return new WaitForEndOfFrame();
		isProcessingShare = false;
	}

	private void completionHandler(string error, object result)
	{
		if (error != null)
		{
			DebugLog(error);
		}
		else
		{
			Prime31.Utils.logObject(result);
		}
	}

	public void SubmitScore(int score, string leaderboardID)
	{
		DebugLog("SubmitScore(Leaderboard: " + leaderboardID + ", score: " + score + ")");
		long score2 = score;
		Social.ReportScore(score2, leaderboardID, SubmitScoreHandler);
	}

	public void LoadLeaderboard(string leaderboardId, LeaderboardLoadedCallback callback)
	{
		DebugLog("BPSSocialController: LoadLeaderboard(" + leaderboardId + ")");
		if (!IsAuthenticated())
		{
			callback(false, null);
			return;
		}
		lock (_loadScoreRequests)
		{
			_loadScoreRequests.Add(new LoadScoreRequest
			{
				leaderboardId = leaderboardId,
				callbackPointer = callback
			});
			LoadNextLeaderboard();
		}
	}

	private void LoadNextLeaderboard()
	{
		LoadScoreRequest req;
		lock (_loadScoreRequests)
		{
			if (_loadScoreRequestPending || _loadScoreRequests.Count == 0)
			{
				return;
			}
			_loadScoreRequestPending = true;
			req = _loadScoreRequests[0];
			_loadScoreRequests.RemoveAt(0);
		}
		ILeaderboard board = Social.CreateLeaderboard();
		board.id = req.leaderboardId;
		board.userScope = UserScope.Global;
		board.LoadScores(delegate(bool scoresloaded)
		{
			req.callbackPointer(scoresloaded, board);
			lock (_loadScoreRequests)
			{
				_loadScoreRequestPending = false;
				LoadNextLeaderboard();
			}
		});
	}

	public void ShowLeaderboardUIOverlay(string leaderboardID, TimeScope timeScope = TimeScope.AllTime)
	{
		if (!IsAuthenticated())
		{
			Authenticate();
		}
		else
		{
			Social.ShowLeaderboardUI();
		}
	}

	public string GetLocalUserID()
	{
		return Social.localUser.id;
	}

	private void LoadUsersAchievements()
	{
		Social.LoadAchievements(LoadAchievementsHandler);
	}

	public void SubmitAchievement(string achievementID, float percentComplete = 100f)
	{
		DebugLog(string.Format("Submiting achievement: {0} with percent: {1}", achievementID, percentComplete));
		if (_completedAchievements.Contains(achievementID))
		{
			DebugLog("Canceled submitting achievement(" + achievementID + ") as its already been completed");
			return;
		}
		percentComplete = Mathf.Clamp(percentComplete, 0f, 100f);
		IAchievement achievement = Social.CreateAchievement();
		achievement.id = achievementID;
		achievement.percentCompleted = percentComplete;
		achievement.ReportProgress(delegate
		{
		});
		if (percentComplete == 100f)
		{
			DebugLog("Adding new complete achievement: " + achievementID);
			_completedAchievements.Add(achievementID);
		}
	}

	public void ShowAchievementUIOverlay()
	{
		if (!IsAuthenticated())
		{
			Authenticate();
		}
		else
		{
			Social.ShowAchievementsUI();
		}
	}

	public void ResetAchievements()
	{
	}

	private void ResetAllAchievementsHandler(bool success)
	{
		DebugLog("BPSSocialController: ResetAllAchievementsHandler() success: " + success);
		LoadUsersAchievements();
	}

	protected Hashtable[] ParseLeaderboardPluginStringIntoArrayOfHashtable(string param)
	{
		DebugLog("ParseLeaderboardPluginStringIntoArrayOfHashtable( string : " + param + " )");
		if (param == string.Empty)
		{
			return new Hashtable[0];
		}
		string[] array = param.Split("&"[0]);
		Hashtable[] array2 = new Hashtable[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			string[] array3 = text.Split("/"[0]);
			Hashtable hashtable = new Hashtable();
			foreach (string text2 in array3)
			{
				string[] array4 = text2.Split("="[0]);
				if (array4.Length >= 2)
				{
					hashtable.Add(array4[0], array4[1]);
				}
			}
			array2[i] = hashtable;
		}
		return array2;
	}

	private List<BPSSocialScoreInfo> ConvertHashtableListToBPSScoreInfoList(Hashtable[] scoresArray)
	{
		DebugLog("ConvertHashtableListToBPSScoreInfoList( count = " + scoresArray.Length + " )");
		List<BPSSocialScoreInfo> list = new List<BPSSocialScoreInfo>();
		foreach (Hashtable hashtable in scoresArray)
		{
			BPSSocialScoreInfo bPSSocialScoreInfo = ScriptableObject.CreateInstance<BPSSocialScoreInfo>();
			bPSSocialScoreInfo.rank = int.Parse(hashtable["playerrank"].ToString());
			bPSSocialScoreInfo.formattedRank = hashtable["playerrank"].ToString();
			bPSSocialScoreInfo.score = int.Parse(hashtable["playerscore"].ToString());
			bPSSocialScoreInfo.formattedScore = hashtable["playerscoreformatted"].ToString();
			bPSSocialScoreInfo.playerId = hashtable["playerid"].ToString();
			bPSSocialScoreInfo.displayName = string.Empty;
			list.Add(bPSSocialScoreInfo);
		}
		return list;
	}

	protected void AuthenticationHandler(bool isSuccess)
	{
		if (isSuccess)
		{
			DebugLog("AuthenticationSuccessHandler(isSuccess:" + isSuccess + ")");
			LoadUsersAchievements();
		}
		else
		{
			DebugLog("AuthenticationFailedHandler");
		}
		if (this.AuthenticateEvent != null)
		{
			this.AuthenticateEvent(isSuccess);
		}
	}

	protected void SubmitScoreHandler(bool isSuccessful)
	{
		if (isSuccessful)
		{
			DebugLog("SubmitScoreSuccessHandler");
			if (this.ScoreSubmittedSuccessEvent != null)
			{
				this.ScoreSubmittedSuccessEvent(null);
			}
		}
		else
		{
			DebugLog("SubmitScoreFailedHandler");
			if (this.ScoreSubmittedFailedEvent != null)
			{
				this.ScoreSubmittedFailedEvent(null);
			}
		}
		_lastSubmissionLeaderboardId = null;
	}

	protected void LoadAchievementsHandler(IAchievement[] achievements)
	{
		DebugLog("LoadAchievementsHandler (Count: " + achievements.Length + ")");
		_completedAchievements.Clear();
		foreach (IAchievement achievement in achievements)
		{
			if (achievement.completed)
			{
				DebugLog("Adding loaded and complete achievement: " + achievement.id);
				_completedAchievements.Add(achievement.id);
			}
		}
	}

	private void TweetSheetCompletedHandler(bool success)
	{
		StartCoroutine(DeleteScreenshot());
	}
}
