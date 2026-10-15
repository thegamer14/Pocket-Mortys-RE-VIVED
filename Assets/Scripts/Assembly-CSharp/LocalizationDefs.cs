using System;
using SimpleJSON;
using UnityEngine;

public class LocalizationDefs
{
	public delegate void LocalizationDelegate();

	public static int eLanguageCount = Enum.GetNames(typeof(eLanguage)).Length;

	private static JSONNode _infoJSON;

	private static LocalizedMaterialList _materialList;

	public static event LocalizationDelegate LanguageChangedEvent;

	public static void SetUp()
	{
		LoadInNewLocalizationJSON(GetCurrentLanguage());
		GameObject gameObject = (GameObject)Resources.Load("LocalizedAssets/MaterialList");
		_materialList = gameObject.GetComponent<LocalizedMaterialList>();
		_materialList.LoadComponents();
		_materialList.Localize();
	}

	private static void LoadInNewLocalizationJSON(eLanguage language)
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/Text/{0}", GetLanguageCode(language))).text);
	}

	public static string GetLocalizedString(eGooglePage page, string id, string tag)
	{
		return _infoJSON[GetPageResourceName(page)][id][tag];
	}

	public static string GetAssetSpriteResource(string id)
	{
		return string.Format("LocalizedAssets/{0}/Sprites/{1}", GetCurrentLanguageCode(), id);
	}

	public static string GetAssetTextureResource(string id)
	{
		return string.Format("LocalizedAssets/{0}/Textures/{1}", GetCurrentLanguageCode(), id);
	}

	public static eLanguage GetCurrentLanguage()
	{
		if (!Application.isPlaying)
		{
			return eLanguage.EN;
		}
		return App.saveData.settings.language;
	}

	public static void SetLanguage(eLanguage language)
	{
		if (GetCurrentLanguage() != language)
		{
			App.saveData.settings.language = language;
			LoadInNewLocalizationJSON(language);
			_materialList.Localize();
			AttackDefs.Localize();
			App.saveData.Save(eSaveDataType.SETTINGS);
			if (LocalizationDefs.LanguageChangedEvent != null)
			{
				LocalizationDefs.LanguageChangedEvent();
			}
		}
	}

	public static string GetLanguageName(eLanguage language)
	{
		switch (language)
		{
		default:
			return string.Empty;
		case eLanguage.EN:
			return TextDefs.LANGUAGE_ENGLISH;
		case eLanguage.FR:
			return TextDefs.LANGUAGE_FRENCH;
		case eLanguage.IT:
			return TextDefs.LANGUAGE_ITALIAN;
		case eLanguage.DE:
			return TextDefs.LANGUAGE_GERMAN;
		case eLanguage.ES:
			return TextDefs.LANGUAGE_SPANISH;
		case eLanguage.PT_BR:
			return TextDefs.LANGUAGE_BRAZILIAN_PORTUGUESE;
		case eLanguage.ZH_TW:
			return TextDefs.LANGUAGE_CHINESE_TRADITIONAL;
		case eLanguage.ZH_CN:
			return TextDefs.LANGUAGE_CHINESE_SIMPLIFIED;
		case eLanguage.JP:
			return TextDefs.LANGUAGE_JAPANESE;
		case eLanguage.KO:
			return TextDefs.LANGUAGE_KOREAN;
		}
	}

	public static eLanguage GetLocalizeLanguageForSystemLanguage(out bool foundLanguage)
	{
		foundLanguage = true;
		switch (Application.systemLanguage)
		{
		default:
			foundLanguage = false;
			return eLanguage.EN;
		case SystemLanguage.English:
			return eLanguage.EN;
		case SystemLanguage.French:
			return eLanguage.FR;
		case SystemLanguage.Italian:
			return eLanguage.IT;
		case SystemLanguage.German:
			return eLanguage.DE;
		case SystemLanguage.Spanish:
			return eLanguage.ES;
		case SystemLanguage.Portuguese:
			return eLanguage.PT_BR;
		case SystemLanguage.ChineseTraditional:
			return eLanguage.ZH_TW;
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
			return eLanguage.ZH_CN;
		case SystemLanguage.Japanese:
			return eLanguage.JP;
		case SystemLanguage.Korean:
			return eLanguage.KO;
		}
	}

	public static string GetCurrentLanguageCode()
	{
		return GetLanguageCode(GetCurrentLanguage());
	}

	public static string GetLanguageCode(eLanguage language)
	{
		switch (language)
		{
		default:
			return string.Empty;
		case eLanguage.EN:
			return "EN";
		case eLanguage.FR:
			return "FR";
		case eLanguage.IT:
			return "IT";
		case eLanguage.DE:
			return "DE";
		case eLanguage.ES:
			return "ES";
		case eLanguage.PT_BR:
			return "PT_BR";
		case eLanguage.ZH_TW:
			return "ZH_TW";
		case eLanguage.ZH_CN:
			return "ZH_CN";
		case eLanguage.JP:
			return "JP";
		case eLanguage.KO:
			return "KO";
		}
	}

	public static string GetLanguageCodeGoogle(eLanguage language)
	{
		switch (language)
		{
		default:
			return string.Empty;
		case eLanguage.EN:
			return "en";
		case eLanguage.FR:
			return "fr";
		case eLanguage.IT:
			return "it";
		case eLanguage.DE:
			return "de";
		case eLanguage.ES:
			return "es";
		case eLanguage.PT_BR:
			return "ptbr";
		case eLanguage.ZH_TW:
			return "zhtw";
		case eLanguage.ZH_CN:
			return "zhcn";
		case eLanguage.JP:
			return "jp";
		case eLanguage.KO:
			return "ko";
		}
	}

	private static string GetPageResourceName(eGooglePage page)
	{
		switch (page)
		{
		default:
			return string.Empty;
		case eGooglePage.TextDefs:
			return "TextDefs";
		case eGooglePage.World:
			return "World";
		case eGooglePage.Interactions:
			return "Interactions";
		case eGooglePage.SignPost:
			return "SignPost";
		case eGooglePage.PlayerAvatar:
			return "PlayerAvatar";
		case eGooglePage.Morty:
			return "Morty";
		case eGooglePage.Attack:
			return "Attack";
		case eGooglePage.NPC:
			return "NPC";
		case eGooglePage.Trainer:
			return "Trainer";
		case eGooglePage.Item:
			return "Item";
		case eGooglePage.Recipe:
			return "Recipe";
		case eGooglePage.Quest:
			return "Quest";
		case eGooglePage.Tournament:
			return "Tournament";
		case eGooglePage.TournamentEvent:
			return "TournamentEvent";
		case eGooglePage.TournamentTrainer:
			return "TournamentTrainer";
		case eGooglePage.TournamentDialogue:
			return "TournamentDialogue";
		case eGooglePage.Challenge:
			return "Challenge";
		}
	}

	public static bool TryAutoSelectLanguage()
	{
		if (!App.saveData.settings.languageSetByUser)
		{
			bool foundLanguage = false;
			eLanguage localizeLanguageForSystemLanguage = GetLocalizeLanguageForSystemLanguage(out foundLanguage);
			eLanguage currentLanguage = GetCurrentLanguage();
			SetLanguage(localizeLanguageForSystemLanguage);
			if (!foundLanguage)
			{
				return false;
			}
			if (localizeLanguageForSystemLanguage != currentLanguage)
			{
				if (PlayerDefs.BadgeCount > 0)
				{
					return false;
				}
				return true;
			}
			return true;
		}
		return true;
	}
}
