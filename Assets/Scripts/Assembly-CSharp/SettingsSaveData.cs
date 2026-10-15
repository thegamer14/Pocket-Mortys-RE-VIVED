using System;
using System.Runtime.Serialization;

[Serializable]
public class SettingsSaveData : SaveDataBase
{
	private const string _kSoundFXKey = "SoundFXVolumeKey";

	private const string _kMusicKey = "MusicVolumeKey";

	private const string _kRemoveAdsKey = "RemoveAdsKey";

	private const string _kUpsightRewarRecieptsKey = "UpsightRewardReceiptsKey";

	private const string _kLanguageKey = "LanguageKey";

	private const string _kLanguageSetByUserKey = "LanguageSetByUserKey";

	public float soundFXVolume;

	public float musicVolume;

	public bool removeAds;

	public string[] upsightRewardReceipts;

	public eLanguage language;

	public bool languageSetByUser;

	protected override int _targetVersionID
	{
		get
		{
			return 1;
		}
	}

	public SettingsSaveData()
	{
	}

	public SettingsSaveData(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	protected override void SetDefaults()
	{
		base.SetDefaults();
		soundFXVolume = 1f;
		musicVolume = 1f;
		removeAds = false;
		upsightRewardReceipts = new string[0];
		language = eLanguage.EN;
		languageSetByUser = false;
	}

	protected override void SerializeValues(SerializationInfo info, StreamingContext context)
	{
		base.SerializeValues(info, context);
		info.AddValue("SoundFXVolumeKey", soundFXVolume, typeof(float));
		info.AddValue("MusicVolumeKey", musicVolume, typeof(float));
		info.AddValue("RemoveAdsKey", removeAds, typeof(bool));
		info.AddValue("UpsightRewardReceiptsKey", upsightRewardReceipts, typeof(string[]));
		info.AddValue("LanguageKey", language, typeof(eLanguage));
		info.AddValue("LanguageSetByUserKey", languageSetByUser, typeof(bool));
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
			case "SoundFXVolumeKey":
				soundFXVolume = (float)current.Value;
				break;
			case "MusicVolumeKey":
				musicVolume = (float)current.Value;
				break;
			case "RemoveAdsKey":
				removeAds = (bool)current.Value;
				break;
			case "UpsightRewardReceiptsKey":
				upsightRewardReceipts = (string[])current.Value;
				break;
			case "LanguageKey":
				language = (eLanguage)current.Value;
				break;
			case "LanguageSetByUserKey":
				languageSetByUser = (bool)current.Value;
				break;
			}
		}
	}

	protected override void UpdateToVersion2(SerializationInfo info)
	{
	}

	protected override void UpdateToVersion3(SerializationInfo info)
	{
	}

	protected override void UpdateToVersion4(SerializationInfo info)
	{
	}
}
