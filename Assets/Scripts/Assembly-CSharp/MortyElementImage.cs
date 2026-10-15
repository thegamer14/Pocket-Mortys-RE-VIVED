using UnityEngine;
using UnityEngine.UI;

public class MortyElementImage : MonoBehaviour
{
	private Image _image;

	private void Awake()
	{
		_image = GetComponent<Image>();
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}

	public bool Set(string elementType, bool isLarge = false)
	{
		Sprite sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/ElementType/{0}{1}", elementType, (!isLarge) ? string.Empty : "Large"));
		if (sprite != null)
		{
			SetActive(true);
			_image.sprite = sprite;
			_image.SetNativeSize();
			return true;
		}
		SetActive(false);
		return false;
	}
}
