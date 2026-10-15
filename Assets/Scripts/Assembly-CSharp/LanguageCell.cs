using UnityEngine.UI;

public class LanguageCell : CellBase
{
	private Text _label;

	protected override void Awake()
	{
		base.Awake();
		_label = base.transform.FindChild("Label").GetComponent<Text>();
	}

	public void SetUp(eLanguage language)
	{
		base.SetUp(language.ToString());
		_label.text = LocalizationDefs.GetLanguageName(language);
	}
}
