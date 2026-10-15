using UnityEngine;
using UnityEngine.UI;

public class LocalizedAssetImage : LocalizedAsset
{
	public bool SetNativeSize = true;

	private Sprite _enSprite;

	private Image _image;

	protected override void LoadComponents()
	{
		_image = GetComponent<Image>();
		_enSprite = _image.sprite;
	}

	public void ResetEnglishAsset(Sprite newSprite)
	{
		if (_image == null)
		{
			LoadComponents();
		}
		_enSprite = newSprite;
		Localize();
	}

	protected override void Localize()
	{
		Sprite sprite = Resources.Load<Sprite>(LocalizationDefs.GetAssetSpriteResource(_enSprite.name));
		if (sprite != null)
		{
			_image.sprite = sprite;
		}
		else
		{
			_image.sprite = _enSprite;
		}
		if (SetNativeSize)
		{
			_image.SetNativeSize();
		}
	}
}
