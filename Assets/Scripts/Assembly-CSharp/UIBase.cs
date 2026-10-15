using UnityEngine;

public class UIBase : MonoBehaviour
{
	protected const float _backgroundFadeAlpha = 0.4f;

	protected CanvasGroup _canvasGroup;

	public RectTransform rectTransform { get; private set; }

	private void Awake()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		rectTransform = GetComponent<RectTransform>();
		LateAwake();
		SetEvents(true);
	}

	protected virtual void LateAwake()
	{
	}

	public virtual void SetLabels()
	{
	}

	private void SetEvents(bool isAdd)
	{
		if (isAdd)
		{
			AddEvents();
		}
		else
		{
			RemoveEvents();
		}
	}

	protected virtual void AddEvents()
	{
	}

	protected virtual void RemoveEvents()
	{
	}

	public virtual void Destroy()
	{
		SetEvents(false);
	}

	public virtual void ManualUpdate(float deltaTime)
	{
	}

	public virtual void ActivateInput(bool isActive)
	{
		_canvasGroup.interactable = isActive;
	}

	public virtual void SetParent(Transform parent)
	{
		rectTransform.SetParent(parent, false);
	}

	public virtual void SetSiblingOrder(int siblingIndex)
	{
		rectTransform.SetSiblingIndex(siblingIndex);
	}

	public virtual int GetSiblingOrder()
	{
		return rectTransform.GetSiblingIndex();
	}
}
