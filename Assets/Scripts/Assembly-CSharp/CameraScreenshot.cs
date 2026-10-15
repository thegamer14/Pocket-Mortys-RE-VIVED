using System.IO;
using UnityEngine;

public class CameraScreenshot : MonoBehaviour
{
	private const string _kFileExtention = ".png";

	public string folderPath;

	public int resWidth = 3840;

	public int resHeight = 2160;

	private Camera _camera;

	private string _fileName = string.Empty;

	private bool _takeScreenshot;

	private void Start()
	{
		_camera = GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		if (_takeScreenshot)
		{
			RenderTexture renderTexture = new RenderTexture(resWidth, resHeight, 24);
			_camera.targetTexture = renderTexture;
			Texture2D texture2D = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
			_camera.Render();
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, resWidth, resHeight), 0, 0);
			_camera.targetTexture = null;
			RenderTexture.active = null;
			Object.Destroy(renderTexture);
			byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes(folderPath + "/" + _fileName + ".png", bytes);
			_takeScreenshot = false;
		}
	}

	public void Take(string fileName = null, string folderPath = null)
	{
		if (folderPath != null)
		{
			this.folderPath = folderPath;
		}
		_fileName = fileName;
		_takeScreenshot = true;
	}
}
