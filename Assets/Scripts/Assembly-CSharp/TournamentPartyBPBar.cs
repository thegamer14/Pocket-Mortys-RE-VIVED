using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TournamentPartyBPBar : MonoBehaviour
{
	private RectTransform _fillRectTrans;

	private Text _counterText;

	private int _currentAmount;

	private Tween counterTween;

	private void Awake()
	{
		_fillRectTrans = (RectTransform)base.transform.FindChild("Bar/Fill");
		_counterText = base.transform.FindChild("Counter").GetComponent<Text>();
	}

	public void SetUp(int points)
	{
		SetPoints(points, points);
	}

	public void SetFill(int total, int amount)
	{
		_fillRectTrans.localScale = new Vector3(1f, (float)amount / (float)total, 1f);
	}

	public void SetCounter(int total, int amount)
	{
		_counterText.text = string.Format("{0}/{1}", amount, total);
	}

	public void SetPoints(int total, int amount)
	{
		SetFill(total, amount);
		SetCounter(total, amount);
		_currentAmount = amount;
	}

	public void TweenPoints(int total, int amount)
	{
		_fillRectTrans.DOKill();
		if (counterTween != null)
		{
			counterTween.Kill();
			counterTween = null;
		}
		_fillRectTrans.DOScaleY((float)amount / (float)total, 0.4f).SetEase(Ease.Linear);
		counterTween = DOTween.To(() => _currentAmount, delegate(int x)
		{
			_currentAmount = x;
		}, amount, 0.4f);
		counterTween.SetEase(Ease.Linear);
		counterTween.OnUpdate(delegate
		{
			SetCounter(total, _currentAmount);
		});
	}
}
