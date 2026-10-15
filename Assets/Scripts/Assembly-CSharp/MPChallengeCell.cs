using UnityEngine;
using UnityEngine.UI;

public class MPChallengeCell : CellBase
{
	public Sprite completeSpr;

	public Sprite cooldownSpr;

	public MPChallengeData data;

	private Text _title;

	private Text _description;

	private GameObject _rewardObj;

	private Text _rewardTitle;

	private Text _rewardCoins;

	private GameObject _progressObj;

	private Image _progressFill;

	private Text _progressCount;

	private GameObject _completeObj;

	private Text _completeTitle;

	private GameObject _cooldownObj;

	private Text _cooldownTitle;

	protected override void Awake()
	{
		base.Awake();
		_title = base.transform.FindChild("Title").GetComponent<Text>();
		_description = base.transform.FindChild("Description").GetComponent<Text>();
		_rewardObj = base.transform.FindChild("Reward").gameObject;
		_rewardTitle = base.transform.FindChild("Reward/Title").GetComponent<Text>();
		_rewardCoins = base.transform.FindChild("Reward/Coins").GetComponent<Text>();
		_progressObj = base.transform.FindChild("ProgressBar").gameObject;
		_progressFill = base.transform.FindChild("ProgressBar/Fill").GetComponent<Image>();
		_progressCount = base.transform.FindChild("ProgressBar/Count").GetComponent<Text>();
		_completeObj = base.transform.FindChild("CompleteBar").gameObject;
		_completeTitle = base.transform.FindChild("CompleteBar/Title").GetComponent<Text>();
		_cooldownObj = base.transform.FindChild("CooldownBar").gameObject;
		_cooldownTitle = base.transform.FindChild("CooldownBar/Title").GetComponent<Text>();
	}

	public void SetUp(MPChallengeData data)
	{
		this.data = data;
		base.SetUp(data.id);
		RefreshDisplayValues();
	}

	private void RefreshDisplayValues()
	{
		_title.text = LocalizationDefs.GetLocalizedString(eGooglePage.Challenge, base.id, "title");
		_description.text = string.Format(LocalizationDefs.GetLocalizedString(eGooglePage.Challenge, base.id, "description"), data.target);
		_rewardTitle.text = MPTextDefs.MP_CHALLENGE_CELL_REWARD_TITLE;
		_rewardCoins.text = data.reward.ToString();
		_progressFill.fillAmount = 1f - (float)data.count / (float)data.target;
		_progressCount.text = string.Format("{0}/{1}", data.count, data.target);
		_completeTitle.text = MPTextDefs.MP_CHALLENGE_CELL_COMPLETE_TITLE;
		_cooldownTitle.text = string.Format(MPTextDefs.MP_CHALLENGE_CELL_COOLDOWN_TITLE, TextDefs.AbbreviateTimestamp(data.cooldown));
		RefreshSelectedValues();
	}

	private void RefreshSelectedValues()
	{
		if (data.IsCoolingDown())
		{
			_rewardObj.SetActive(false);
			_progressObj.SetActive(false);
			_completeObj.SetActive(false);
			_cooldownObj.SetActive(true);
			if (base.state != eScrollMenuCellState.SELECTED)
			{
				SetBackground(cooldownSpr);
			}
		}
		else if (data.IsClaimable())
		{
			_rewardObj.SetActive(true);
			_progressObj.SetActive(false);
			_cooldownObj.SetActive(false);
			_completeObj.SetActive(true);
			if (base.state != eScrollMenuCellState.SELECTED)
			{
				SetBackground(completeSpr);
			}
		}
		else
		{
			_rewardObj.SetActive(true);
			_cooldownObj.SetActive(false);
			_completeObj.SetActive(false);
			_progressObj.SetActive(true);
			if (base.state != eScrollMenuCellState.SELECTED)
			{
				SetBackground(_defaultSpr);
			}
		}
	}

	public override void SetSelect(bool value)
	{
		base.SetSelect(value);
		RefreshSelectedValues();
	}

	private void Update()
	{
		if (data.cooldown > 0.0)
		{
			data.cooldown -= Time.deltaTime;
			_cooldownTitle.text = string.Format(MPTextDefs.MP_CHALLENGE_CELL_COOLDOWN_TITLE, TextDefs.AbbreviateTimestamp(data.cooldown));
			if (data.cooldown <= 0.0)
			{
				RefreshDisplayValues();
			}
		}
	}
}
