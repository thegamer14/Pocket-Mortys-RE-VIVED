using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleTutorialTipUI : MonoBehaviour
{
	private Image _background;

	private RectTransform _descriptionRect;

	private Text _description;

	private Image _arrow;

	protected virtual void Awake()
	{
		_background = base.transform.FindChild("Background").GetComponent<Image>();
		_descriptionRect = base.transform.FindChild("Box").GetComponent<RectTransform>();
		_description = _descriptionRect.FindChild("Label").GetComponent<Text>();
		_arrow = base.transform.FindChild("Arrow").GetComponent<Image>();
	}

	public virtual void SetUp(string description, Vector2 descriptionPosition, Vector2 arrowPosition, int arrowRotation)
	{
		_background.rectTransform.localPosition = new Vector3(0f, descriptionPosition.y);
		_background.color = new Color(1f, 1f, 1f, 0f);
		_description.text = description;
		_descriptionRect.localPosition = descriptionPosition;
		_descriptionRect.localScale = Vector3.zero;
		_arrow.rectTransform.localPosition = arrowPosition + new Vector2(0f, 80f);
		_arrow.rectTransform.localEulerAngles = new Vector3(0f, 0f, arrowRotation);
		_arrow.color = new Color(1f, 1f, 1f, 0f);
		StartCoroutine(TweenIn());
	}

	public void Remove()
	{
		StartCoroutine(TweenOut());
	}

	protected virtual IEnumerator TweenIn()
	{
		_background.DOColor(new Color(1f, 1f, 1f, 0.4f), 0.2f);
		yield return _descriptionRect.DOScale(1f, 0.2f).WaitForCompletion();
		_arrow.DOColor(Color.white, 1f);
		yield return _arrow.rectTransform.DOLocalMoveY(_arrow.rectTransform.localPosition.y - 80f, 1f).WaitForCompletion();
		_arrow.rectTransform.DOLocalMoveY(_arrow.rectTransform.localPosition.y + 30f, 1f).SetLoops(-1, LoopType.Yoyo);
	}

	protected virtual IEnumerator TweenOut()
	{
		_arrow.DOKill();
		_arrow.DOColor(new Color(1f, 1f, 1f, 0f), 0.2f);
		_background.DOColor(new Color(1f, 1f, 1f, 0f), 0.2f);
		yield return _descriptionRect.DOScale(0f, 0.2f).WaitForCompletion();
		Object.Destroy(base.gameObject);
	}
}
