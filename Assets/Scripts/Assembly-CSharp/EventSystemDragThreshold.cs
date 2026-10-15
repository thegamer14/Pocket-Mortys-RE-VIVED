using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemDragThreshold : MonoBehaviour
{
	private const float inchToCm = 2.54f;

	[SerializeField]
	private EventSystem eventSystem;

	[SerializeField]
	private float dragThresholdCM = 0.5f;

	private void Start()
	{
		if (eventSystem == null)
		{
			eventSystem = GetComponent<EventSystem>();
		}
		SetDragThreshold();
	}

	private void SetDragThreshold()
	{
		if (eventSystem != null)
		{
			eventSystem.pixelDragThreshold = (int)(dragThresholdCM * Screen.dpi / 2.54f);
		}
	}
}
