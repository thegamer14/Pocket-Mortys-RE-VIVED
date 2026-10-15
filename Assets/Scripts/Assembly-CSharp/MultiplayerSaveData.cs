using System;
using System.Runtime.Serialization;

[Serializable]
public class MultiplayerSaveData : SaveDataBase
{
	private const string _kSecretIDKey = "secretIDKey";

	private const string _kUsernameKey = "usernameKey";

	private const string _kPlayerAvatarIDKey = "playerAvatarIDKey";

	private const string _kLastWorldIDKey = "lastWorldIdKey";

	private const string _kSpTutorialCompleteKey = "spTutorialCompleteKey";

	private const string _kSeenXpTutorialKey = "seenXpTutorialKey";

	private const string _kSeenXpMultiplierTutorialKey = "seenXpMultiplierTutorialKey";

	private const string _kLastServerMessageIdKey = "lastServerMessageIdKey";

	public string secretID;

	public string username;

	public string playerAvatarID;

	public string lastWorldId;

	public bool spTutorialComplete;

	public bool seenXpTutorial;

	public bool seenXpMultiplierTutorial;

	public string lastServerMessageId;

	protected override int _targetVersionID
	{
		get
		{
			return 1;
		}
	}

	public MultiplayerSaveData()
	{
	}

	public MultiplayerSaveData(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	protected override void SetDefaults()
	{
		base.SetDefaults();
		secretID = null;
		playerAvatarID = null;
		username = null;
		lastWorldId = "1";
		spTutorialComplete = false;
		seenXpTutorial = false;
		seenXpMultiplierTutorial = false;
		lastServerMessageId = null;
	}

	protected override void SerializeValues(SerializationInfo info, StreamingContext context)
	{
		base.SerializeValues(info, context);
		info.AddValue("secretIDKey", secretID, typeof(string));
		info.AddValue("usernameKey", username, typeof(string));
		info.AddValue("playerAvatarIDKey", playerAvatarID, typeof(string));
		info.AddValue("lastWorldIdKey", lastWorldId, typeof(string));
		info.AddValue("spTutorialCompleteKey", spTutorialComplete, typeof(bool));
		info.AddValue("seenXpTutorialKey", seenXpTutorial, typeof(bool));
		info.AddValue("seenXpMultiplierTutorialKey", seenXpMultiplierTutorial, typeof(bool));
		info.AddValue("lastServerMessageIdKey", lastServerMessageId, typeof(string));
	}

	protected override void DeserializeValues(SerializationInfo info, StreamingContext context)
	{
		base.DeserializeValues(info, context);
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			switch (current.Name)
			{
			case "secretIDKey":
				secretID = (string)current.Value;
				break;
			case "usernameKey":
				username = (string)current.Value;
				break;
			case "playerAvatarIDKey":
				playerAvatarID = (string)current.Value;
				break;
			case "lastWorldIdKey":
				lastWorldId = (string)current.Value;
				break;
			case "spTutorialCompleteKey":
				spTutorialComplete = (bool)current.Value;
				break;
			case "seenXpTutorialKey":
				seenXpTutorial = (bool)current.Value;
				break;
			case "seenXpMultiplierTutorialKey":
				seenXpMultiplierTutorial = (bool)current.Value;
				break;
			case "lastServerMessageIdKey":
				lastServerMessageId = (string)current.Value;
				break;
			}
		}
	}
}
