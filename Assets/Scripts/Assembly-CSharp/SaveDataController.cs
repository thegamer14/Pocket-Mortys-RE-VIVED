using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

public class SaveDataController : MonoBehaviour
{
	public delegate void CloudContainerConflictDelegate(List<CloudContainer> containers);

	public delegate void InitCloudContainerCompleteDelegate();

	protected const string _kSettingsFilePath = "Settings.dat";

	protected const string _kPlayerFilePath = "Player.dat";

	protected const string _kSegmentedWorldFilePath = "SegmentedWorld.dat";

	protected const string _kTournamentFilePath = "Tournament.dat";

	protected const string _kMultiplayerFilePath = "multiplayer.dat";

	protected const string _kCloudContainerFilePath = "CloudContainer.dat";

	protected const float _kCloudDownloadTimeout = 10f;

	private string _storagePathForLoading;

	private string _storagePathForSaving;

	private CloudContainer _activeCloudContainer;

	private CloudContainer _localCloudContainer;

	private ManualConflictResolver _manualResolver;

	private ISavedGameMetadata _currentCloudContainerMeta;

	public SettingsSaveData settings { get; private set; }

	public PlayerSaveData player { get; private set; }

	public SegmentedWorldSaveData segmentedWorld { get; private set; }

	public TournamentSaveData tournament { get; private set; }

	public MultiplayerSaveData multiplayer { get; private set; }

	public eInitCloudContainerStatus initCloudContainerStatus { get; private set; }

	private bool _cloudAvailable
	{
		get
		{
			return App.social != null && App.social.IsAuthenticated();
		}
	}

	public event CloudContainerConflictDelegate CloudContainerConflictEvent;

	public event InitCloudContainerCompleteDelegate InitCloudContainerCompleteEvent;

	private void Awake()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		}
		settings = new SettingsSaveData();
		player = new PlayerSaveData();
		segmentedWorld = new SegmentedWorldSaveData();
		tournament = new TournamentSaveData();
		multiplayer = new MultiplayerSaveData();
		_storagePathForSaving = GetInternalStoragePath();
		if (string.IsNullOrEmpty(_storagePathForSaving))
		{
			_storagePathForSaving = Application.persistentDataPath;
		}
		HotFixOutdatedAndroidLocalCloudContainer();
		if (Application.isEditor)
		{
			_storagePathForSaving = GetEditorSaveDataLocation();
			Directory.CreateDirectory(_storagePathForSaving);
		}
	}

	private void OnDestroy()
	{
	}

	private string GetNewestAppStoragePathForFile(string filePath)
	{
		string internalStoragePath = GetInternalStoragePath();
		string externalStoragePath = GetExternalStoragePath();
		DebugLog("internalPath=" + internalStoragePath);
		DebugLog("externalPath=" + externalStoragePath);
		if (internalStoragePath == Application.persistentDataPath)
		{
			DebugLog("internalPath == Application.persistentDataPath");
		}
		if (externalStoragePath == Application.persistentDataPath)
		{
			DebugLog("externalPath == Application.persistentDataPath");
		}
		if (string.Equals(internalStoragePath, externalStoragePath))
		{
			DebugLog("Internal and external paths are equal, returning external");
			return externalStoragePath;
		}
		if (string.IsNullOrEmpty(internalStoragePath) && string.IsNullOrEmpty(externalStoragePath))
		{
			DebugLog("Both storage paths are null or empty, returning null");
			return null;
		}
		if (!string.IsNullOrEmpty(internalStoragePath) && string.IsNullOrEmpty(externalStoragePath))
		{
			DebugLog("Only internal path is not null, returning it.");
			return internalStoragePath;
		}
		if (string.IsNullOrEmpty(internalStoragePath) && !string.IsNullOrEmpty(externalStoragePath))
		{
			DebugLog("Only external path is not null, returning it.");
			return externalStoragePath;
		}
		string path = internalStoragePath + "/" + filePath;
		string path2 = externalStoragePath + "/" + filePath;
		bool flag = File.Exists(path);
		bool flag2 = File.Exists(path2);
		if (!flag && !flag2)
		{
			DebugLog(filePath + " not found in either location, returning null");
			return null;
		}
		if (flag && !flag2)
		{
			DebugLog(filePath + " only found in internal storage, returning internal path");
			return internalStoragePath;
		}
		if (!flag && flag2)
		{
			DebugLog(filePath + " only found in external storage, returning internal path");
			return externalStoragePath;
		}
		if (flag && flag2)
		{
			DebugLog(filePath + " found in both locations, determining newest.");
			DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(path);
			DateTime lastWriteTimeUtc2 = File.GetLastWriteTimeUtc(path2);
			DebugLog("internalWriteTime = " + lastWriteTimeUtc.ToString());
			DebugLog("externalWriteTime = " + lastWriteTimeUtc2.ToString());
			if (lastWriteTimeUtc.Ticks > lastWriteTimeUtc2.Ticks)
			{
				DebugLog(filePath + " in internal is newer, returning internal path");
				return internalStoragePath;
			}
			DebugLog(filePath + " in external is newer, returning internal path");
			return externalStoragePath;
		}
		DebugLog("Got to end of GetNewestStoragePath() without returning anything, returning null");
		return null;
	}

	private string GetInternalStoragePath()
	{
		if (string.IsNullOrEmpty("com.turner.pocketmorties"))
		{
		}
		if (Application.isEditor)
		{
			return GetEditorSaveDataLocation();
		}
		DebugLog("Application.persistentDataPath = " + Application.persistentDataPath);
		string text = null;
		try
		{
			IntPtr javaClass = AndroidJNI.FindClass("android/content/ContextWrapper");
			IntPtr methodID = AndroidJNIHelper.GetMethodID(javaClass, "getFilesDir", "()Ljava/io/File;");
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					IntPtr obj = AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, new jvalue[0]);
					IntPtr javaClass2 = AndroidJNI.FindClass("java/io/File");
					IntPtr methodID2 = AndroidJNIHelper.GetMethodID(javaClass2, "getAbsolutePath", "()Ljava/lang/String;");
					text = AndroidJNI.CallStringMethod(obj, methodID2, new jvalue[0]);
					if (text != null)
					{
						DebugLog("Got internal path: " + text);
					}
					else
					{
						DebugLog("failed to get internal path, using fallback");
						text = "/data/data/" + "com.turner.pocketmorties" + "/files";
					}
				}
			}
		}
		catch (Exception ex)
		{
			DebugLog("GetInternalStoragePath() exception: " + ex.ToString());
		}
		return text;
	}

	private string GetExternalStoragePath()
	{
		if (string.IsNullOrEmpty("com.turner.pocketmorties"))
		{
		}
		if (Application.isEditor)
		{
			return GetEditorSaveDataLocation();
		}
		DebugLog("Application.persistentDataPath = " + Application.persistentDataPath);
		string text = null;
		try
		{
			IntPtr clazz = AndroidJNI.FindClass("android/os/Environment");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getExternalStorageDirectory", "()Ljava/io/File;");
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					IntPtr obj = AndroidJNI.CallStaticObjectMethod(androidJavaObject.GetRawObject(), staticMethodID, new jvalue[0]);
					IntPtr javaClass = AndroidJNI.FindClass("java/io/File");
					IntPtr methodID = AndroidJNIHelper.GetMethodID(javaClass, "getAbsolutePath", "()Ljava/lang/String;");
					text = AndroidJNI.CallStringMethod(obj, methodID, new jvalue[0]);
					if (text != null)
					{
						text = text + "/Android/data/" + "com.turner.pocketmorties" + "/files";
						DebugLog("Got external path: " + text);
					}
					else
					{
						DebugLog("External path not found");
					}
				}
			}
		}
		catch (Exception ex)
		{
			DebugLog("GetExternalStoragePath() exception: " + ex.ToString());
		}
		return text;
	}

	public void Save(eSaveDataType type)
	{
		DebugLog(string.Format("Save ({0})", type));
		if (type == eSaveDataType.ALL)
		{
			SerializeObjectToFile(settings, "Settings.dat");
			SerializeObjectToFile(player, "Player.dat");
			SerializeObjectToFile(segmentedWorld, "SegmentedWorld.dat");
			SerializeObjectToFile(tournament, "Tournament.dat");
			SerializeObjectToFile(multiplayer, "multiplayer.dat");
		}
		else
		{
			if ((type & eSaveDataType.SETTINGS) == eSaveDataType.SETTINGS)
			{
				SerializeObjectToFile(settings, "Settings.dat");
			}
			if ((type & eSaveDataType.PLAYER) == eSaveDataType.PLAYER)
			{
				SerializeObjectToFile(player, "Player.dat");
			}
			if ((type & eSaveDataType.SEGMENTED_WORLD) == eSaveDataType.SEGMENTED_WORLD)
			{
				SerializeObjectToFile(segmentedWorld, "SegmentedWorld.dat");
			}
			if ((type & eSaveDataType.TOURNAMENT) == eSaveDataType.TOURNAMENT)
			{
				SerializeObjectToFile(tournament, "Tournament.dat");
			}
			if ((type & eSaveDataType.MULTIPLAYER) == eSaveDataType.MULTIPLAYER)
			{
				SerializeObjectToFile(multiplayer, "multiplayer.dat");
			}
		}
		SaveActiveCloudContainerLocalAndCloud();
	}

	private void SaveActiveCloudContainerLocalAndCloud()
	{
		DebugLog("SaveActiveCloudContainerLocalAndCloud()");
		if (_activeCloudContainer == null)
		{
			DebugLog("!!!!! SaveActiveCloudContainerLocalAndCloud() _activeCloudContainer is null, the local and cloud container is not getting saved !!!!!!");
			return;
		}
		_activeCloudContainer.lastModifiedTimestamp = DateTime.UtcNow.Ticks;
		DebugLog("SaveActiveCloudContainerLocalAndCloud() _activeCloudContainer.lastModifiedTimestamp set to " + _activeCloudContainer.lastModifiedTimestamp + " " + new DateTime(_activeCloudContainer.lastModifiedTimestamp).ToLocalTime().ToString());
		_activeCloudContainer.settings = settings;
		_activeCloudContainer.player = player;
		_activeCloudContainer.segmentedWorld = segmentedWorld;
		_activeCloudContainer.tournament = tournament;
		_activeCloudContainer.multiplayer = multiplayer;
		SerializeObjectToFile(_activeCloudContainer, "CloudContainer.dat");
		SaveActiveCloudContainerToCloud();
	}

	public void InitCloudContainer()
	{
		DebugLog("InitCloudContainer()");
		initCloudContainerStatus = eInitCloudContainerStatus.STARTED;
		if (_localCloudContainer == null)
		{
			DebugLog("InitCloudContainer() Saving a _localCloudContainer as one doesnt exist.");
			_localCloudContainer = GetCloudContainerFromLocalData();
			SerializeObjectToFile(_localCloudContainer, "CloudContainer.dat");
		}
		bool flag = _localCloudContainer.player != null && _localCloudContainer.player.storiesSeen.Contains("GarageIntro");
		if (!_cloudAvailable)
		{
			if (App.social == null)
			{
				DebugLog("App.social is null");
			}
			DebugLog("App.social.IsAuthenticated():" + App.social.IsAuthenticated());
			DebugLog("InitCloudContainer() cloud not available, completing with local cloud container.");
			CompeleteInitCloudContainerWithContainer(_localCloudContainer);
			return;
		}
		try
		{
			DebugLog("InitCloudContainer() attempting to open the cloud file with manual resolution.");
			initCloudContainerStatus = eInitCloudContainerStatus.OPENING;
			PlayGamesPlatform.Instance.SavedGame.OpenWithManualConflictResolution("CloudContainer.dat", DataSource.ReadNetworkOnly, true, OpenCloudContainerConflictHandler, OpenCloudContainerCompleteHandler);
		}
		catch (Exception ex)
		{
			DebugLog("InitCloudContainer() failed to open the cloud container, continuing with local cloud data. error: " + ex.Message);
			CompeleteInitCloudContainerWithContainer(_localCloudContainer);
		}
	}

	private void OpenCloudContainerConflictHandler(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
	{
		DebugLog("OpenCloudContainerConflictHandler()");
		CloudContainer cloudContainer = ByteDataToObject(originalData) as CloudContainer;
		CloudContainer cloudContainer2 = ByteDataToObject(unmergedData) as CloudContainer;
		DateTime dateTime = new DateTime(original.LastModifiedTimestamp.ToLocalTime().Ticks);
		DebugLog("original LastModifiedTimestamp: " + dateTime.ToString());
		DateTime dateTime2 = new DateTime(unmerged.LastModifiedTimestamp.ToLocalTime().Ticks);
		DebugLog("unmerged LastModifiedTimestamp: " + dateTime2.ToString());
		_manualResolver = new ManualConflictResolver(resolver, original, cloudContainer, unmerged, cloudContainer2);
		if (cloudContainer == null && cloudContainer2 == null)
		{
			DebugLog("originalContainer and unmergedContainer are invalid, calling TryResolveConflicWithContainer(_localCloudContainer)");
			TryResolveConflicWithContainer(_localCloudContainer);
			return;
		}
		List<CloudContainer> list = new List<CloudContainer>();
		if (cloudContainer != null)
		{
			cloudContainer.containerType = eCloudContainerType.ORIGINAL;
			list.Add(cloudContainer);
		}
		if (cloudContainer2 != null)
		{
			cloudContainer2.containerType = eCloudContainerType.UNMERGED;
			list.Add(cloudContainer2);
		}
		if (list.Count == 0)
		{
			_localCloudContainer.containerType = eCloudContainerType.LOCAL;
			list.Add(_localCloudContainer);
		}
		else if (_localCloudContainer.player != null && _localCloudContainer.player.storiesSeen.Contains("GarageIntro"))
		{
			_localCloudContainer.containerType = eCloudContainerType.LOCAL;
			list.Add(_localCloudContainer);
		}
		string text = "OpenCloudContainerConflictHandler() - Containers in conflict list: " + list.Count + "\n";
		foreach (CloudContainer item in list)
		{
			text = text + item.containerType.ToString() + "\n";
		}
		DebugLog(text);
		if (list.Count == 1)
		{
			DebugLog("OpenCloudContainerConflictHandler() - Only 1 container in the conflict list, trying to resolve with it: " + list[0].containerType);
			TryResolveConflicWithContainer(list[0]);
			return;
		}
		DebugLog("OpenCloudContainerConflictHandler() - Multiple containers in the conflict list, dispatching conflict event");
		initCloudContainerStatus = eInitCloudContainerStatus.AWAITING_CONFLICT_RESOLVE;
		if (this.CloudContainerConflictEvent != null)
		{
			this.CloudContainerConflictEvent(list);
		}
	}

	private void OpenCloudContainerCompleteHandler(SavedGameRequestStatus openStatus, ISavedGameMetadata meta)
	{
		DebugLog("OpenCloudContainerCompleteHandler()");
		_manualResolver = null;
		if (openStatus == SavedGameRequestStatus.Success)
		{
			PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(meta, delegate(SavedGameRequestStatus readStatus, byte[] data)
			{
				if (readStatus == SavedGameRequestStatus.Success)
				{
					_currentCloudContainerMeta = meta;
					DebugLog("OpenCloudContainerCompleteHandler() Attempting to cast to CloudContainer.");
					CloudContainer cloudContainer = ByteDataToObject(data) as CloudContainer;
					if (cloudContainer != null)
					{
						cloudContainer.containerType = eCloudContainerType.NON_CONFLICTING;
						int num = ((cloudContainer.player != null && cloudContainer.player.badges != null) ? cloudContainer.player.badges.Count : 0);
						int num2 = ((cloudContainer.player != null) ? cloudContainer.player.coins : 0);
						string text = string.Format("Badges: {0}, Schmeckels: {1}", num, num2);
						DebugLog("OpenCloudContainerCompleteHandler() cast to CloudContainer success. " + text);
						if (cloudContainer.lastModifiedTimestamp == _localCloudContainer.lastModifiedTimestamp)
						{
							DebugLog("OpenCloudContainerCompleteHandler() Local and cloud containers match, completing with local.");
							CompeleteInitCloudContainerWithContainer(_localCloudContainer);
						}
						else if (_localCloudContainer.player == null || !_localCloudContainer.player.storiesSeen.Contains("GarageIntro"))
						{
							DebugLog("OpenCloudContainerCompleteHandler() Local container has not meaningful progress, completing with cloud.");
							CompeleteInitCloudContainerWithContainer(cloudContainer);
						}
						else
						{
							DebugLog(string.Format("OpenCloudContainerCompleteHandler() Local container modified timestamp ({0}) differs from cloud container modified timestamp ({1}), requesting conflict resolution.", _localCloudContainer.lastModifiedTimestamp, cloudContainer.lastModifiedTimestamp));
							initCloudContainerStatus = eInitCloudContainerStatus.AWAITING_CONFLICT_RESOLVE;
							if (this.CloudContainerConflictEvent != null)
							{
								this.CloudContainerConflictEvent(new List<CloudContainer> { _localCloudContainer, cloudContainer });
							}
						}
					}
					else
					{
						DebugLog("OpenCloudContainerCompleteHandler() cast to CloudContainer failed, completing with local data.");
						CompeleteInitCloudContainerWithContainer(_localCloudContainer);
					}
				}
				else
				{
					DebugLog("OpenCloudContainerCompleteHandler() Failed to read data from opened save game, completing with local data.");
					CompeleteInitCloudContainerWithContainer(_localCloudContainer);
				}
			});
		}
		else
		{
			DebugLog("OpenCloudContainerCompleteHandler() Failed to open (" + openStatus.ToString() + "), completing with local data.");
			CompeleteInitCloudContainerWithContainer(_localCloudContainer);
		}
	}

	public void TryResolveConflicWithContainer(CloudContainer cloudContainer)
	{
		DebugLog("TryResolveConflicWithContainer()");
		if (cloudContainer == _localCloudContainer)
		{
			DebugLog("TryResolveConflicWithContainer() - localCloudContainer was chosen.");
			if (_manualResolver != null)
			{
				DebugLog("TryResolveConflicWithContainer() - deleting conflicting files from _manualResolver.");
				PlayGamesPlatform.Instance.SavedGame.Delete(_manualResolver.originalMeta);
				PlayGamesPlatform.Instance.SavedGame.Delete(_manualResolver.unmergedMeta);
				_manualResolver = null;
			}
			CompeleteInitCloudContainerWithContainer(_localCloudContainer);
		}
		else if (_manualResolver != null)
		{
			if (cloudContainer.Equals(_manualResolver.originalObj))
			{
				DebugLog("TryResolveConflicWithContainer() Resolving with original.");
				initCloudContainerStatus = eInitCloudContainerStatus.RESOLVING;
				_manualResolver.resolver.ChooseMetadata(_manualResolver.originalMeta);
			}
			else
			{
				DebugLog("TryResolveConflicWithContainer() Resolving with unmerged.");
				initCloudContainerStatus = eInitCloudContainerStatus.RESOLVING;
				_manualResolver.resolver.ChooseMetadata(_manualResolver.unmergedMeta);
			}
		}
		else
		{
			DebugLog("TryResolveConflicWithContainer() A non-conflicting cloud container was chosen, completing with it.");
			CompeleteInitCloudContainerWithContainer(cloudContainer);
		}
	}

	private void CompeleteInitCloudContainerWithContainer(CloudContainer cloudContainer)
	{
		DebugLog("CompeleteInitCloudContainerWithContainer()");
		SetActiveCloudContainer(cloudContainer);
		initCloudContainerStatus = eInitCloudContainerStatus.COMPLETE;
		if (this.InitCloudContainerCompleteEvent != null)
		{
			this.InitCloudContainerCompleteEvent();
		}
	}

	private void SetActiveCloudContainer(CloudContainer cloudContainer)
	{
		DebugLog("SetActiveCloudContainer()");
		OverwriteContainerMultiplayerSaveWithLocal(cloudContainer);
		_activeCloudContainer = cloudContainer;
		ExtractCloudContainerToDisk(cloudContainer);
		Load();
		SharedStatInfo.ResetAll();
		SaveActiveCloudContainerLocalAndCloud();
	}

	private void OverwriteContainerMultiplayerSaveWithLocal(CloudContainer cloudContainer)
	{
		DebugLog("OverwriteContainerMultiplayerSaveWithLocal()");
		if (_localCloudContainer != null && _localCloudContainer.multiplayer != null && _localCloudContainer.multiplayer.secretID != null && (cloudContainer.multiplayer == null || cloudContainer.multiplayer.secretID == null))
		{
			string arg = ((cloudContainer == null || cloudContainer.multiplayer == null) ? null : cloudContainer.multiplayer.username);
			string arg2 = ((_localCloudContainer == null || _localCloudContainer.multiplayer == null) ? null : _localCloudContainer.multiplayer.username);
			string str = string.Format("OverwriteContainerMultiplayerSaveWithLocal() - The multiplayer data in the chosen container (username:{0}) will be overwritten with the local multiplayer data (username:{1}).", arg, arg2);
			DebugLog(str);
			cloudContainer.multiplayer = _localCloudContainer.multiplayer;
		}
	}

	public void Load()
	{
		DebugLog("Load()");
		_storagePathForLoading = GetNewestAppStoragePathForFile("Player.dat");
		if (string.IsNullOrEmpty(_storagePathForLoading))
		{
			_storagePathForLoading = Application.persistentDataPath;
		}
		if (Application.isEditor)
		{
			_storagePathForLoading = GetEditorSaveDataLocation();
		}
		object obj = DeserializeFileToObject("Settings.dat");
		if (obj != null)
		{
			settings = (SettingsSaveData)obj;
			SerializeObjectToFile(settings, "Settings.dat");
		}
		object obj2 = DeserializeFileToObject("Player.dat");
		if (obj2 != null)
		{
			player = (PlayerSaveData)obj2;
			SerializeObjectToFile(player, "Player.dat");
		}
		object obj3 = DeserializeFileToObject("SegmentedWorld.dat");
		if (obj3 != null)
		{
			segmentedWorld = (SegmentedWorldSaveData)obj3;
			SerializeObjectToFile(segmentedWorld, "SegmentedWorld.dat");
		}
		object obj4 = DeserializeFileToObject("Tournament.dat");
		if (obj4 != null)
		{
			tournament = (TournamentSaveData)obj4;
			SerializeObjectToFile(tournament, "Tournament.dat");
		}
		object obj5 = DeserializeFileToObject("multiplayer.dat");
		if (obj5 != null)
		{
			multiplayer = (MultiplayerSaveData)obj5;
			SerializeObjectToFile(multiplayer, "multiplayer.dat");
		}
		object obj6 = DeserializeFileToObject("CloudContainer.dat");
		if (obj6 != null)
		{
			DebugLog("Load() - Found a local cloud container, setting it as _localCloudContainer.");
			_localCloudContainer = (CloudContainer)obj6;
			_localCloudContainer.containerType = eCloudContainerType.LOCAL;
			SerializeObjectToFile(_localCloudContainer, "CloudContainer.dat");
		}
	}

	public void Delete(eSaveDataType type)
	{
		DebugLog(string.Format("Delete ({0})", type));
		if (type == eSaveDataType.ALL)
		{
			DeleteFileFromStorage("Settings.dat");
			DeleteFileFromStorage("Player.dat");
			DeleteFileFromStorage("SegmentedWorld.dat");
			DeleteFileFromStorage("Tournament.dat");
			DeleteFileFromStorage("multiplayer.dat");
			settings = new SettingsSaveData();
			player = new PlayerSaveData();
			segmentedWorld = new SegmentedWorldSaveData();
			tournament = new TournamentSaveData();
			multiplayer = new MultiplayerSaveData();
			return;
		}
		if ((type & eSaveDataType.SETTINGS) == eSaveDataType.SETTINGS)
		{
			DeleteFileFromStorage("Settings.dat");
			settings = new SettingsSaveData();
		}
		if ((type & eSaveDataType.PLAYER) == eSaveDataType.PLAYER)
		{
			DeleteFileFromStorage("Player.dat");
			player = new PlayerSaveData();
		}
		if ((type & eSaveDataType.SEGMENTED_WORLD) == eSaveDataType.SEGMENTED_WORLD)
		{
			DeleteFileFromStorage("SegmentedWorld.dat");
			segmentedWorld = new SegmentedWorldSaveData();
		}
		if ((type & eSaveDataType.TOURNAMENT) == eSaveDataType.TOURNAMENT)
		{
			DeleteFileFromStorage("Tournament.dat");
			tournament = new TournamentSaveData();
		}
		if ((type & eSaveDataType.MULTIPLAYER) == eSaveDataType.MULTIPLAYER)
		{
			DeleteFileFromStorage("multiplayer.dat");
			multiplayer = new MultiplayerSaveData();
		}
	}

	private void DeleteFileFromStorage(string file)
	{
		File.Delete(GetInternalStoragePath() + "/" + file);
		File.Delete(GetExternalStoragePath() + "/" + file);
	}

	public static void DeleteFromPersistantData(string file)
	{
		if (Application.isEditor)
		{
			File.Delete(GetEditorSaveDataLocation() + "/" + file);
		}
		else
		{
			File.Delete(Application.persistentDataPath + "/" + file);
		}
	}

	public void RunHotfixes()
	{
		bool flag = false;
		if (HotFixTournamentMissingTickets())
		{
			flag = true;
		}
		if (flag)
		{
			Save(eSaveDataType.ALL);
		}
	}

	private bool HotFixTournamentMissingTickets()
	{
		if (tournament.hotFixMissingTicketsResolved)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < TournamentDefs.orderedTournamentIDs.Count; i++)
		{
			string text = TournamentDefs.orderedTournamentIDs[i];
			string[] array = TournamentDefs.TournamentEventIDs(text);
			for (int j = 0; j < array.Length; j++)
			{
				string eventID = array[j];
				TournamentEventData eventData = TournamentDefs.GetEventData(text, eventID);
				if (eventData == null || !eventData.isComplete)
				{
					continue;
				}
				List<RewardData> list = TournamentDefs.EventRewards(array[j]);
				for (int k = 0; k < list.Count; k++)
				{
					RewardData rewardData = list[k];
					if (rewardData.type == eRewardType.TOURNAMENT_TICKET && !player.caughtMorties.Contains(rewardData.id) && !tournament.ownedTickets.Contains(text))
					{
						tournament.ownedTickets.Add(rewardData.id);
						result = true;
					}
				}
			}
		}
		tournament.hotFixMissingTicketsResolved = true;
		return result;
	}

	private void HotFixOutdatedAndroidLocalCloudContainer()
	{
		DebugLog("HotFixOutdatedAndroidLocalCloudContainer()");
		_storagePathForLoading = GetNewestAppStoragePathForFile("Player.dat");
		if (!File.Exists(_storagePathForLoading + "/CloudContainer.dat"))
		{
			DebugLog("HotFixOutdatedAndroidLocalCloudContainer() no local cloud container found, skipping hotfix.");
			return;
		}
		List<string> list = new List<string>();
		list.Add("Settings.dat");
		list.Add("Player.dat");
		list.Add("SegmentedWorld.dat");
		list.Add("Tournament.dat");
		list.Add("multiplayer.dat");
		List<string> list2 = list;
		DateTime dateTime = new DateTime(0L).ToUniversalTime();
		foreach (string item in list2)
		{
			if (File.Exists(_storagePathForLoading + "/" + item))
			{
				DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(_storagePathForLoading + "/" + item);
				if (lastWriteTimeUtc > dateTime)
				{
					DebugLog("HotFixOutdatedAndroidLocalCloudContainer() " + item + " is the latest save so far, written at " + lastWriteTimeUtc.ToLocalTime().ToString());
					dateTime = lastWriteTimeUtc;
				}
			}
		}
		DateTime lastWriteTimeUtc2 = File.GetLastWriteTimeUtc(_storagePathForLoading + "/CloudContainer.dat");
		DebugLog("HotFixOutdatedAndroidLocalCloudContainer() localCloudContainerWriteTime: " + lastWriteTimeUtc2.ToLocalTime().ToString() + " mostRecentDataWriteTime: " + dateTime.ToLocalTime().ToString());
		if (lastWriteTimeUtc2 < dateTime)
		{
			DebugLog("HotFixOutdatedAndroidLocalCloudContainer() outdated local cloud container found, creating and saving an up to date one now.");
			CloudContainer cloudContainer = new CloudContainer();
			cloudContainer.settings = DeserializeFileToObject<SettingsSaveData>("Settings.dat");
			cloudContainer.player = DeserializeFileToObject<PlayerSaveData>("Player.dat");
			cloudContainer.segmentedWorld = DeserializeFileToObject<SegmentedWorldSaveData>("SegmentedWorld.dat");
			cloudContainer.tournament = DeserializeFileToObject<TournamentSaveData>("Tournament.dat");
			cloudContainer.multiplayer = DeserializeFileToObject<MultiplayerSaveData>("multiplayer.dat");
			cloudContainer.lastModifiedTimestamp = dateTime.Ticks;
			SerializeObjectToFile(cloudContainer, "CloudContainer.dat");
		}
		else
		{
			DebugLog("HotFixOutdatedAndroidLocalCloudContainer() localCloudContainer was up to date, no fix made.");
		}
		DebugLog("HotFixOutdatedAndroidLocalCloudContainer() Complete");
	}

	public void SerializeObjectToFile(object obj, string filePath)
	{
		string text = _storagePathForSaving + "/" + filePath;
		DebugLog("Serializing obj to " + text);
		if (obj == null)
		{
			DebugLog(" - trying to serialize a null obj to " + text + ", aborting serialize");
			return;
		}
		if (obj is CloudContainer)
		{
			CloudContainer cloudContainer = obj as CloudContainer;
			DebugLog("SerializeObjectToFile() saving local cloudContainer with timestamp " + cloudContainer.lastModifiedTimestamp + " " + new DateTime(cloudContainer.lastModifiedTimestamp).ToLocalTime().ToString());
		}
		FileStream fileStream = new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(fileStream, obj);
		fileStream.Close();
		fileStream.Dispose();
	}

	public object DeserializeFileToObject(string filePath)
	{
		object result = null;
		string text = _storagePathForLoading + "/" + filePath;
		DebugLog("Deserializing obj from " + text);
		if (File.Exists(text))
		{
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				FileStream fileStream = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.None);
				result = binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
				fileStream.Dispose();
			}
			catch (Exception)
			{
			}
		}
		return result;
	}

	public T DeserializeFileToObject<T>(string filePath)
	{
		object obj = DeserializeFileToObject(filePath);
		if (obj != null && obj is T)
		{
			return (T)obj;
		}
		return default(T);
	}

	public string DeserializeFileToString(string filePath)
	{
		string text = _storagePathForLoading + "/" + filePath;
		DebugLog(string.Format("DeserializeFileToString({0})", text));
		if (File.Exists(text))
		{
			using (FileStream fileStream = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				byte[] array = new byte[fileStream.Length];
				int num = (int)fileStream.Length;
				int num2 = 0;
				while (num > 0)
				{
					int num3 = fileStream.Read(array, num2, num);
					if (num3 == 0)
					{
						break;
					}
					num2 += num3;
					num -= num3;
				}
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				byte[] array2 = array;
				foreach (byte b in array2)
				{
					stringBuilder.Append(b.ToString("X2"));
					if (flag)
					{
						stringBuilder.Append(" ");
					}
					flag = !flag;
				}
				return stringBuilder.ToString().ToLower();
			}
		}
		DebugLog("File does not exist at " + text);
		return null;
	}

	private void SerializeObjectToCloudFile(object obj, string filePath, string description, ISavedGameMetadata meta, Action<bool> successCallback)
	{
		DebugLog("SerializeObjectToCloudFile(" + filePath + ")");
		if (obj == null)
		{
			DebugLog("SerializeObjectToCloudFile() failed: obj is null");
			if (successCallback != null)
			{
				successCallback(false);
			}
			return;
		}
		if (string.IsNullOrEmpty(filePath))
		{
			DebugLog("SerializeObjectToCloudFile() failed: filePath is null or empty");
			if (successCallback != null)
			{
				successCallback(false);
			}
			return;
		}
		if (!_cloudAvailable)
		{
			DebugLog("SerializeObjectToCloudFile() failed: _cloudAvailable is false");
			if (successCallback != null)
			{
				successCallback(false);
			}
			return;
		}
		if (meta != null && meta.IsOpen)
		{
			DebugLog("SerializeObjectToCloudFile() meta is already open, committing.");
			CommitObjectToOpenCloudFile(obj, filePath, description, meta, successCallback);
			return;
		}
		DebugLog("SerializeObjectToCloudFile() meta is closed, openning.");
		try
		{
			PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(filePath, DataSource.ReadNetworkOnly, ConflictResolutionStrategy.UseOriginal, delegate(SavedGameRequestStatus status, ISavedGameMetadata gamemeta)
			{
				if (status == SavedGameRequestStatus.Success)
				{
					_currentCloudContainerMeta = gamemeta;
					DebugLog("SerializeObjectToCloudFile() Meta successfully openned, committing.");
					CommitObjectToOpenCloudFile(obj, filePath, description, gamemeta, successCallback);
				}
				else
				{
					DebugLog("SerializeObjectToCloudFile() Could not save to google cloud as save failed to open with status: " + status);
					if (successCallback != null)
					{
						successCallback(false);
					}
				}
			});
		}
		catch (Exception ex)
		{
			DebugLog("SerializeObjectToCloudFile() exception: " + ex.Message);
			if (successCallback != null)
			{
				successCallback(false);
			}
		}
	}

	private void CommitObjectToOpenCloudFile(object obj, string filePath, string description, ISavedGameMetadata meta, Action<bool> successCallback)
	{
		DebugLog("CommitObjectToOpenCloudFile() " + filePath);
		long ticks = ((player != null) ? ((long)player.totalGameTime) : 0);
		SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedPlayedTime(new TimeSpan(ticks)).WithUpdatedDescription(description).Build();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, obj);
		if (obj is CloudContainer)
		{
			CloudContainer cloudContainer = obj as CloudContainer;
			DebugLog("CommitObjectToOpenCloudFile() cloudContainer.lastModifiedTimestamp: " + cloudContainer.lastModifiedTimestamp + " " + new DateTime(cloudContainer.lastModifiedTimestamp).ToLocalTime().ToString());
		}
		try
		{
			PlayGamesPlatform.Instance.SavedGame.CommitUpdate(meta, updateForMetadata, memoryStream.ToArray(), delegate(SavedGameRequestStatus commitStatus, ISavedGameMetadata gameMeta)
			{
				DebugLog(string.Concat("CommitObjectToOpenCloudFile() committed status: ", commitStatus, ")"));
				if (successCallback != null)
				{
					successCallback(commitStatus == SavedGameRequestStatus.Success);
				}
			});
		}
		catch (Exception ex)
		{
			DebugLog("CommitObjectToOpenCloudFile() exception: " + ex.Message);
			if (successCallback != null)
			{
				successCallback(false);
			}
		}
		memoryStream.Dispose();
		memoryStream.Close();
	}

	private object ByteDataToObject(byte[] data)
	{
		object result = null;
		try
		{
			MemoryStream memoryStream = new MemoryStream(data);
			if (memoryStream.Length > 0)
			{
				result = new BinaryFormatter().Deserialize(memoryStream);
			}
			memoryStream.Dispose();
			memoryStream.Close();
		}
		catch (Exception ex)
		{
			DebugLog("ByteDataToObject() exception: " + ex.Message);
		}
		return result;
	}

	private void DeleteCloudFile(string filename)
	{
		DebugLog("DeleteCloudFile() " + filename);
		if (!_cloudAvailable)
		{
			return;
		}
		try
		{
			PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(filename, DataSource.ReadNetworkOnly, ConflictResolutionStrategy.UseLongestPlaytime, delegate(SavedGameRequestStatus status, ISavedGameMetadata game)
			{
				if (status == SavedGameRequestStatus.Success)
				{
					PlayGamesPlatform.Instance.SavedGame.Delete(game);
				}
			});
		}
		catch (Exception ex)
		{
			DebugLog("DeleteCloudFile() exception: " + ex.Message);
		}
	}

	private CloudContainer GetCloudContainerFromLocalData()
	{
		CloudContainer cloudContainer = new CloudContainer();
		cloudContainer.settings = settings;
		cloudContainer.player = player;
		cloudContainer.segmentedWorld = segmentedWorld;
		cloudContainer.tournament = tournament;
		cloudContainer.multiplayer = multiplayer;
		cloudContainer.containerType = eCloudContainerType.LOCAL;
		return cloudContainer;
	}

	public void ExtractCloudContainerToDisk(CloudContainer cloudContainer)
	{
		DebugLog("ExtractCloudContainerToDisk()");
		if (cloudContainer != null)
		{
			SerializeObjectToFile(cloudContainer.settings, "Settings.dat");
			SerializeObjectToFile(cloudContainer.player, "Player.dat");
			SerializeObjectToFile(cloudContainer.segmentedWorld, "SegmentedWorld.dat");
			SerializeObjectToFile(cloudContainer.tournament, "Tournament.dat");
			SerializeObjectToFile(cloudContainer.multiplayer, "multiplayer.dat");
		}
	}

	public void SaveActiveCloudContainerToCloud(Action<bool> saveCallback = null)
	{
		DebugLog("SaveActiveCloudContainerToCloud() _cloudAvailable:" + _cloudAvailable);
		if (_activeCloudContainer == null)
		{
			DebugLog("_activeCloudContainer is null");
			if (saveCallback != null)
			{
				saveCallback(false);
			}
			return;
		}
		if (!_cloudAvailable)
		{
			DebugLog("_cloudAvailable is false");
			if (saveCallback != null)
			{
				saveCallback(false);
			}
			return;
		}
		_activeCloudContainer.settings = settings;
		_activeCloudContainer.player = player;
		_activeCloudContainer.segmentedWorld = segmentedWorld;
		_activeCloudContainer.tournament = tournament;
		_activeCloudContainer.multiplayer = multiplayer;
		int num = ((player != null && player.badges != null) ? player.badges.Count : 0);
		int num2 = ((player != null) ? player.coins : 0);
		string description = string.Format("Badges: {0}, Schmeckels: {1}", num, num2);
		SerializeObjectToCloudFile(_activeCloudContainer, "CloudContainer.dat", description, _currentCloudContainerMeta, delegate(bool success)
		{
			if (saveCallback != null)
			{
				saveCallback(success);
			}
		});
	}

	public void EmailCloudContainerToSupport()
	{
		string text = MyEscapeURL("Pocket Mortys Support");
		string text2 = "If you're experiencing an issue with Pocket Mortys, please describe the problem in as much detail as possible in this space and send this email on. Thanks!\n\n\n\n\n-----DO NOT EDIT BELOW THIS LINE-----\n\n";
		string text3 = string.Concat("Device Name: ", SystemInfo.deviceName, "\nDevice Model: ", SystemInfo.deviceModel, "\n Device Type: ", SystemInfo.deviceType, "\n Operation System: ", SystemInfo.operatingSystem, "\n Memory Size: ", SystemInfo.systemMemorySize, "\n App Version: v2.0.3\n\n");
		string text4 = "Cloud Container:\n\n" + DeserializeFileToString("CloudContainer.dat") + "\n\n";
		string text5 = "---- END ----";
		string text6 = MyEscapeURL(text2 + text3 + text4 + text5);
		string text7 = "adultswimdotcom@gmail.com,pocketmortys@bigpixelstudios.co.uk";
		Application.OpenURL("mailto:" + text7 + "?subject=" + text + "&body=" + text6);
	}

	private string MyEscapeURL(string url)
	{
		return WWW.EscapeURL(url).Replace("+", "%20");
	}

	private static string GetEditorSaveDataLocation()
	{
		return Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName + "/SaveData";
	}

	protected void DebugLog(string str)
	{
		if (TestDefs.DEBUG_SAVE_CONTROLLER)
		{
		}
	}
}
