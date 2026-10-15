using UnityEngine;

public class UI
{
	public static RectTransform Instantiate(string resource, Transform parent = null, eUISibling sibling = eUISibling.NONE, int siblingIndex = 0)
	{
		Object obj = Resources.Load(resource);
		if (obj == null)
		{
			return null;
		}
		return Instantiate(obj, parent, sibling, siblingIndex);
	}

	public static RectTransform Instantiate(Object resourceObj, Transform parent = null, eUISibling sibling = eUISibling.NONE, int siblingIndex = 0)
	{
		GameObject gameObject = Object.Instantiate(resourceObj) as GameObject;
		RectTransform component = gameObject.GetComponent<RectTransform>();
		if (parent != null)
		{
			component.SetParent(parent, false);
		}
		switch (sibling)
		{
		case eUISibling.FIRST:
			component.SetAsFirstSibling();
			break;
		case eUISibling.LAST:
			component.SetAsLastSibling();
			break;
		case eUISibling.INDEX:
			component.SetSiblingIndex(siblingIndex);
			break;
		}
		return component;
	}
}
