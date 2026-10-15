using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackButton : Button, IPointerDownHandler, IEventSystemHandler
{
	private MenuBtnBase _menuBtn;

	protected override void Awake()
	{
		_menuBtn = GetComponent<MenuBtnBase>();
	}

	protected override void Start()
	{
		if (Application.isPlaying)
		{
			App.states.BackButtonEvent += BackButtonHandler;
		}
	}

	protected override void OnDestroy()
	{
		if (Application.isPlaying)
		{
			App.states.BackButtonEvent -= BackButtonHandler;
		}
	}

	public void BackButtonHandler()
	{
		if (App.states.currentState != null && App.states.currentState.isInputActive && base.gameObject.activeInHierarchy)
		{
			if (_menuBtn != null)
			{
				_menuBtn.OnPointerDown(null);
				_menuBtn.OnPointerUp(null);
			}
			else
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.submitHandler);
			}
		}
	}
}
