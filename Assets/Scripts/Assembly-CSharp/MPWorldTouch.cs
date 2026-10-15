using UnityEngine;
using UnityEngine.EventSystems;

public class MPWorldTouch : UIBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public delegate void TouchDownDelegate(Vector2 screenPoint);

	private Camera _uiCamera;

	public event TouchDownDelegate TouchDownEvent;

	protected override void Awake()
	{
		base.Awake();
		_uiCamera = GameObject.Find("MainCanvas").GetComponent<MainCanvas>().canvasCamera;
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		Vector2 screenPoint = _uiCamera.ScreenToViewportPoint(eventData.pressPosition);
		if (this.TouchDownEvent != null)
		{
			this.TouchDownEvent(screenPoint);
		}
	}
}
