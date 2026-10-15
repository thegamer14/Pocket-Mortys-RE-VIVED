using UnityEngine;
using UnityEngine.UI;

public class ImageSwitchUI : MonoBehaviour
{
	public Sprite[] sprites;

	private Image _image;

	private void Awake()
	{
		_image = GetComponent<Image>();
	}

	public void SetSprite(int index)
	{
		_image.sprite = sprites[index];
		_image.SetNativeSize();
	}
}
