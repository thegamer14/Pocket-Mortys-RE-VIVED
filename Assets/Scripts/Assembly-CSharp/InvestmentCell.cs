using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InvestmentCell : CellBase
{
	public delegate void CellStateChangedDelegate(InvestmentCell cell);

	public int investmentIndex;

	private Image _iconImg;

	private GameObject _completeTabObj;

	private Text _completeText;

	private Text _timeText;

	private Text _depositTitleText;

	private Text _depositText;

	private Text _returnTitleText;

	private Text _returnText;

	private Transform _clockTrans;

	private RectTransform _progressArrowTrans;

	private Vector2 _progressArrowStartPos;

	public eInvestmentState investmentState { get; private set; }

	public event CellStateChangedDelegate CellStateChangedEvent;

	protected override void Awake()
	{
		base.Awake();
		_iconImg = base.transform.Find("Icon").GetComponent<Image>();
		_completeTabObj = base.transform.Find("CompleteTab").gameObject;
		_completeText = _completeTabObj.transform.Find("Text").GetComponent<Text>();
		_timeText = base.transform.Find("TimeText").GetComponent<Text>();
		_depositTitleText = base.transform.Find("DepositTitleText").GetComponent<Text>();
		_depositText = base.transform.Find("DepositText").GetComponent<Text>();
		_returnTitleText = base.transform.Find("ReturnTitleText").GetComponent<Text>();
		_returnText = base.transform.Find("ReturnText").GetComponent<Text>();
		_clockTrans = base.transform.Find("Clock");
		_progressArrowTrans = (RectTransform)base.transform.Find("ProgressArrow");
		_progressArrowStartPos = _progressArrowTrans.anchoredPosition;
	}

	public void SetUp(int investmentIndex)
	{
		base.SetUp(investmentIndex.ToString());
		this.investmentIndex = investmentIndex;
		_completeText.text = TextDefs.INVESTMENT_CELL_COMPLETE;
		_depositTitleText.text = TextDefs.INVESTMENT_CELL_DEPOSIT;
		_depositText.text = InvestmentDefs.GetInvestmentDeposit(investmentIndex).ToString();
		_returnTitleText.text = TextDefs.INVESTMENT_CELL_RETURN;
		_returnText.text = InvestmentDefs.GetInvestmentReturn(investmentIndex).ToString();
		_iconImg.sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/Cells/Investment/Icons/investmentIcon{0}", investmentIndex));
		UpdateState();
	}

	private void UpdateState()
	{
		eInvestmentState eInvestmentState2 = InvestmentDefs.GetInvestmentState(investmentIndex);
		if (eInvestmentState2 != investmentState)
		{
			investmentState = eInvestmentState2;
			switch (investmentState)
			{
			case eInvestmentState.IDLE:
				SetAsIdle();
				break;
			case eInvestmentState.PROGRESS:
				SetAsInProgress();
				break;
			case eInvestmentState.RESULT_AVAILABLE:
				SetAsComplete();
				break;
			}
			if (this.CellStateChangedEvent != null)
			{
				this.CellStateChangedEvent(this);
			}
		}
	}

	private void SetAsComplete()
	{
		_completeTabObj.SetActive(true);
		UpdateTimeLeftLabel();
		_clockTrans.localRotation = default(Quaternion);
		_progressArrowTrans.gameObject.SetActive(false);
	}

	private void SetAsInProgress()
	{
		_completeTabObj.SetActive(false);
		UpdateTimeLeftLabel();
		_clockTrans.localRotation = default(Quaternion);
		_progressArrowTrans.gameObject.SetActive(true);
		StartCoroutine(AnimProgressArrowCo());
	}

	private void SetAsIdle()
	{
		_completeTabObj.SetActive(false);
		UpdateTimeLeftLabel();
		_clockTrans.localRotation = default(Quaternion);
		_progressArrowTrans.gameObject.SetActive(false);
	}

	private void Update()
	{
		UpdateState();
		if (investmentState == eInvestmentState.PROGRESS)
		{
			UpdateTimeLeftLabel();
			_clockTrans.Rotate(0f, 0f, -250f * Time.deltaTime);
		}
	}

	private void UpdateTimeLeftLabel()
	{
		string text = null;
		switch (investmentState)
		{
		case eInvestmentState.IDLE:
			text = string.Format(TextDefs.INVESTMENT_CELL_DURATION, TextDefs.AbbreviateTimestamp(InvestmentDefs.GetInvestmentDuration(investmentIndex)));
			break;
		case eInvestmentState.PROGRESS:
			text = string.Format(TextDefs.INVESTMENT_CELL_TIME_LEFT, TextDefs.AbbreviateTimestamp(InvestmentDefs.GetInvestmentTimeLeft(investmentIndex)));
			break;
		case eInvestmentState.RESULT_AVAILABLE:
			text = TextDefs.INVESTMENT_CELL_COMPLETE_MSG;
			break;
		}
		_timeText.text = text;
	}

	private IEnumerator AnimProgressArrowCo()
	{
		_progressArrowTrans.anchoredPosition = _progressArrowStartPos;
		float count = 0f;
		while (_progressArrowTrans.gameObject.activeSelf)
		{
			if (count == 4f)
			{
				count = 0f;
				_progressArrowTrans.anchoredPosition = _progressArrowStartPos;
			}
			else
			{
				count += 1f;
				_progressArrowTrans.anchoredPosition += new Vector2(49.2f, 0f);
			}
			yield return new WaitForSeconds(0.7f);
		}
	}
}
