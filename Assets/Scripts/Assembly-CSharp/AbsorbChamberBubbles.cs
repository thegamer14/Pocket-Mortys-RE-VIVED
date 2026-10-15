using UnityEngine;

public class AbsorbChamberBubbles : MonoBehaviour
{
	public float speed = 150f;

	public float speedMult = 1f;

	private RectTransform _rectTrans;

	private RectTransform _parentRectTrans;

	private void Awake()
	{
		_rectTrans = base.gameObject.GetComponent<RectTransform>();
		_parentRectTrans = base.transform.parent.GetComponent<RectTransform>();
	}

	private void Update()
	{
		_rectTrans.anchoredPosition += new Vector2(0f, speed * speedMult * Time.deltaTime);
		if (_rectTrans.anchoredPosition.y > _parentRectTrans.rect.height)
		{
			_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, 0f - _rectTrans.sizeDelta.y);
		}
	}
}
