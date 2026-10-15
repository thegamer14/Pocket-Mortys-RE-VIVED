using UnityEngine;

public class LocalizedAsset : MonoBehaviour
{
	private void Awake()
	{
		LoadComponents();
		Localize();
		LocalizationDefs.LanguageChangedEvent += LanguageChangedHandler;
	}

	protected virtual void LoadComponents()
	{
	}

	private void OnDestroy()
	{
		LocalizationDefs.LanguageChangedEvent -= LanguageChangedHandler;
	}

	protected virtual void Localize()
	{
	}

	public void LanguageChangedHandler()
	{
		Localize();
	}
}
