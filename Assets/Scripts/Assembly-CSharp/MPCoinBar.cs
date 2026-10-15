using UnityEngine;
using UnityEngine.UI;

public class MPCoinBar : MonoBehaviour
{
	public delegate void CouponBarDelegate();

	private Text _countTitle;

	private Text _countLabel;

	public RectTransform rectTransform { get; private set; }

	public event CouponBarDelegate GetMoreEvent;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		Transform transform = base.transform.FindChild("Count");
		if (transform != null)
		{
			Transform transform2 = transform.FindChild("Title");
			if (transform2 != null)
			{
				_countTitle = transform2.GetComponent<Text>();
			}
			Transform transform3 = transform.FindChild("Label");
			if (transform3 != null)
			{
				_countLabel = transform3.GetComponent<Text>();
			}
		}
	}

	public void SetUp(int coinCount)
	{
		SetCount(coinCount);
	}

	public void SetLabels()
	{
		if (_countTitle != null)
		{
			_countTitle.text = MPTextDefs.MP_COINS_TITLE;
		}
	}

	public void SetCount(int count)
	{
		if (_countLabel != null)
		{
			_countLabel.text = count.ToString();
		}
	}

	public void GetMorePressed()
	{
		if (this.GetMoreEvent != null)
		{
			this.GetMoreEvent();
		}
	}
}
