using System;
using SimpleJSON;

public class AttackData
{
	public string id { get; private set; }

	public string name { get; private set; }

	public string element { get; private set; }

	public int pp { get; private set; }

	public string description { get; private set; }

	public AttackEffectData[] effectDatas { get; private set; }

	public bool isMultiHit { get; private set; }

	public AttackData(JSONNode jsonNode)
	{
		id = jsonNode["id"];
		element = jsonNode["elementtype"];
		pp = jsonNode["pp"].AsInt;
		Localize();
		SetUpEffectDatas(jsonNode["effects"]);
	}

	public void Localize()
	{
		name = LocalizationDefs.GetLocalizedString(eGooglePage.Attack, id, "name");
		description = LocalizationDefs.GetLocalizedString(eGooglePage.Attack, id, "description");
	}

	private void SetUpEffectDatas(string effectsString)
	{
		if (string.IsNullOrEmpty(effectsString))
		{
			effectDatas = new AttackEffectData[0];
			return;
		}
		effectsString = effectsString.Replace("},{", "|").Replace("{", string.Empty).Replace("}", string.Empty);
		string[] array = effectsString.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
		effectDatas = new AttackEffectData[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			effectDatas[i] = new AttackEffectData(array[i]);
		}
		isMultiHit = Array.FindAll(effectDatas, (AttackEffectData e) => e.type == eAttackEffectType.Hit && !e.toSelf).Length > 1;
	}
}
