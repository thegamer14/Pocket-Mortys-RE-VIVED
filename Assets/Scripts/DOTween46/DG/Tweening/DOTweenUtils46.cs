using UnityEngine;

namespace DG.Tweening
{
	public static class DOTweenUtils46
	{
		public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
		{
			Vector2 vector = new Vector2(from.rect.width * 0.5f + from.rect.xMin, from.rect.height * 0.5f + from.rect.yMin);
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, from.position);
			screenPoint += vector;
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenPoint, null, out localPoint);
			Vector2 vector2 = new Vector2(to.rect.width * 0.5f + to.rect.xMin, to.rect.height * 0.5f + to.rect.yMin);
			return to.anchoredPosition + localPoint - vector2;
		}
	}
}
