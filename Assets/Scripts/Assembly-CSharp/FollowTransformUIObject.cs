using UnityEngine;

public class FollowTransformUIObject : MonoBehaviour
{
	public Transform targetTransform;

	public Vector3 offsetByWorldUnits = Vector3.zero;

	private Camera _worldCamera;

	private RectTransform _thisRect;

	private MainCanvas _mainCanvas;

	private void Awake()
	{
		_mainCanvas = GameObject.Find("MainCanvas").GetComponent<MainCanvas>();
		_thisRect = base.transform as RectTransform;
		_worldCamera = Camera.main;
	}

	private void LateUpdate()
	{
		if (!(targetTransform == null))
		{
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_worldCamera, targetTransform.position + offsetByWorldUnits);
			Vector3 worldPoint;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_mainCanvas.rectTransform, screenPoint, _mainCanvas.canvasCamera, out worldPoint))
			{
				_thisRect.position = worldPoint;
			}
		}
	}
}
