using System;
using System.Collections.Generic;

public class MPServerMessageData
{
	private const string _kDictKeyText = "body";

	private const string _kDictKeyDownFrom = "down_from";

	private const string _kDictKeyValidFrom = "valid_from";

	private const string _kDictKeyValidUntil = "valid_until";

	private const string _kDictKeyShowOnce = "show_once";

	public string text;

	public long downFrom;

	public long validFrom;

	public long validUntil;

	public string showOnce;

	public MPServerMessageData()
	{
	}

	public MPServerMessageData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict);
	}

	private void SetFromDict(Dictionary<string, object> dict)
	{
		if (dict == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dict)
		{
			switch (item.Key)
			{
			case "body":
				SetTextFromDict(ConvertExt.ToDictionary(item.Value));
				break;
			case "down_from":
				downFrom = Convert.ToInt64(item.Value);
				break;
			case "valid_from":
				validFrom = Convert.ToInt64(item.Value);
				break;
			case "valid_until":
				validUntil = Convert.ToInt64(item.Value);
				break;
			case "show_once":
				showOnce = Convert.ToString(item.Value);
				break;
			}
		}
	}

	private void SetTextFromDict(Dictionary<string, object> dict)
	{
		if (dict != null)
		{
			string languageCode = LocalizationDefs.GetLanguageCode(App.saveData.settings.language);
			if (dict.ContainsKey(languageCode))
			{
				text = Convert.ToString(dict[languageCode]);
			}
			languageCode = LocalizationDefs.GetLanguageCode(eLanguage.EN);
			if (text == null && dict.ContainsKey(languageCode))
			{
				text = Convert.ToString(dict[languageCode]);
			}
		}
	}
}
