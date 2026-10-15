using UnityEngine;

public class MainCanvas : MonoBehaviour
{
	public Canvas canvas { get; private set; }

	public Camera canvasCamera { get; private set; }

	public RectTransform rectTransform { get; private set; }

private void Awake()
  	{
  		if (!CheckForDuplicates())
  		{
  			Object.DontDestroyOnLoad(base.gameObject);
  			canvas = GetComponent<Canvas>();
  			rectTransform = GetComponent<RectTransform>();
		canvasCamera = base.transform.FindChild("CanvasCamera").GetComponent<Camera>();
			if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
			{
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			}
		}
  	}
  
	private bool CheckForDuplicates()
	{
		MainCanvas[] array = Object.FindObjectsOfType<MainCanvas>();
		if (array.Length > 1)
		{
			Object.Destroy(base.gameObject);
			return true;
		}
		return false;
	}
}
