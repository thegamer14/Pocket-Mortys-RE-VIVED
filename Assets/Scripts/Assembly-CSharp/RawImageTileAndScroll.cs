using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RawImageTileAndScroll : MonoBehaviour
{
	public float scrollSpeedX;

	public float scrollSpeedY;

	public bool scaleOnUpdate;

	private RawImage _image;

	private RectTransform _rectTrans;

	private void Start()
	{
		_image = GetComponent<RawImage>();
		_rectTrans = GetComponent<RectTransform>();
		RefreshTextureScale();
	}

	private void Update()
	{
		if (scaleOnUpdate)
		{
			RefreshTextureScale();
		}
		float x = MathExt.Wrap(_image.uvRect.x + scrollSpeedX * 0.1f * _image.uvRect.width * Time.deltaTime, 0f, 1f);
		float y = MathExt.Wrap(_image.uvRect.y + scrollSpeedY * 0.1f * _image.uvRect.height * Time.deltaTime, 0f, 1f);
		_image.uvRect = new Rect(x, y, _image.uvRect.width, _image.uvRect.height);
	}

	private void RefreshTextureScale()
	{
		_image.uvRect = new Rect(_image.uvRect.x, _image.uvRect.y, _rectTrans.rect.width / (float)_image.mainTexture.width, _rectTrans.rect.height / (float)_image.mainTexture.height);
	}

	private void OnRenderObject()
	{
	}
}
