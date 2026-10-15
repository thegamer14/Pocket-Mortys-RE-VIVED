using UnityEngine;
using UnityEngine.UI;

public class CouponBar : MonoBehaviour
{
	public delegate void CouponBarDelegate();

	private Text _countTitle;

	private Text _countLabel;

	private Text _getMoreBtnTitle;

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
		Transform transform4 = base.transform.FindChild("GetMoreBtn");
		if (transform4 != null)
		{
			Transform transform5 = transform4.FindChild("Label");
			if (transform5 != null)
			{
				_getMoreBtnTitle = transform5.GetComponent<Text>();
			}
		}
	}

	public void SetUp(int count)
	{
		SetCount(count);
	}

	public void SetLabels()
	{
		if (_countTitle != null)
		{
			_countTitle.text = TextDefs.COUPONS_TITLE;
		}
		if (_getMoreBtnTitle != null)
		{
			_getMoreBtnTitle.text = TextDefs.COUPONS_BTN_GET_MORE;
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
