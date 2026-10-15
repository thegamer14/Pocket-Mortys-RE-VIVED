using System;

public class AttackEffectData
{
	public eAttackEffectType type { get; private set; }

	public bool toSelf { get; private set; }

	public float accuracy { get; private set; }

	public bool accuracyContinue { get; private set; }

	public float power { get; private set; }

	public float percent { get; private set; }

	public string stat { get; private set; }

	public AttackEffectData(string jasonString)
	{
		type = eAttackEffectType.None;
		toSelf = false;
		accuracy = -1f;
		accuracyContinue = false;
		power = 0f;
		percent = 1f;
		stat = null;
		string[] array = jasonString.Split(new string[1] { ", " }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length < 2)
			{
				continue;
			}
			switch (array2[0])
			{
			case "Type":
				type = (eAttackEffectType)Enum.Parse(typeof(eAttackEffectType), array2[1]);
				break;
			case "ToSelf":
				toSelf = bool.Parse(array2[1]);
				break;
			case "Accuracy":
				accuracy = float.Parse(array2[1]);
				if (array2.Length > 2)
				{
					accuracyContinue = bool.Parse(array2[2]);
				}
				break;
			case "Power":
				power = float.Parse(array2[1]);
				break;
			case "Percent":
				percent = float.Parse(array2[1]);
				break;
			case "Stat":
				stat = array2[1];
				break;
			}
		}
	}
}
