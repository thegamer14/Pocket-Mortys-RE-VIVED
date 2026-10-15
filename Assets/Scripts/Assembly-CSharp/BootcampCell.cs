using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BootcampCell : CellBase
{
	public delegate void CellStateChangedDelegate(BootcampCell cell);

	public int bootcampIndex;

	private GameObject _idleTabObj;

	private Text _idleLevelLabel;

	private Text _idleTitleLabel;

	private Text _idleTimeLabel;

	private GameObject _inProgressTabObj;

	private Text _inProgressLevelLabel;

	private Text _inProgressTitleLabel;

	private Text _inProgressTimeLabel;

	private GameObject _resultsTabObj;

	private Text _resultsLevelLabel;

	private Text _resultsTitleLabel;

	public eBootcampState bootcampState { get; private set; }

	public event CellStateChangedDelegate CellStateChangedEvent;

	protected override void Awake()
	{
		base.Awake();
		_idleTabObj = base.transform.Find("IdleTab").gameObject;
		_idleLevelLabel = _idleTabObj.transform.FindChild("LevelBox/LevelLabel").GetComponent<Text>();
		_idleTitleLabel = _idleTabObj.transform.FindChild("TitleLabel").GetComponent<Text>();
		_idleTimeLabel = _idleTabObj.transform.FindChild("TimeBox/TimeLabel").GetComponent<Text>();
		_inProgressTabObj = base.transform.Find("InProgressTab").gameObject;
		_inProgressLevelLabel = _inProgressTabObj.transform.FindChild("LevelBox/LevelLabel").GetComponent<Text>();
		_inProgressTitleLabel = _inProgressTabObj.transform.FindChild("TitleLabel").GetComponent<Text>();
		_inProgressTimeLabel = _inProgressTabObj.transform.FindChild("TimeBox/TimeLabel").GetComponent<Text>();
		_resultsTabObj = base.transform.Find("ResultsTab").gameObject;
		_resultsLevelLabel = _resultsTabObj.transform.FindChild("LevelBox/LevelLabel").GetComponent<Text>();
		_resultsTitleLabel = _resultsTabObj.transform.FindChild("TitleLabel").GetComponent<Text>();
		AddEvents();
	}

	public void SetUp(int bootcampIndex)
	{
		base.SetUp(bootcampIndex.ToString());
		this.bootcampIndex = bootcampIndex;
		PopulateMorties();
		string text = string.Format(TextDefs.MORTY_STORAGE_BOOTCAMP_CELL_LEVEL, BootcampDefs.GetBootcampLevel(bootcampIndex));
		_idleLevelLabel.text = text;
		_idleTitleLabel.text = string.Format(TextDefs.MORTY_STORAGE_BOOTCAMP_CELL_IDLE_TITLE, BootcampDefs.GetXPRewardForBootcamp(bootcampIndex));
		_inProgressLevelLabel.text = text;
		_inProgressTitleLabel.text = TextDefs.MORTY_STORAGE_BOOTCAMP_CELL_IN_PROGRESS_TITLE;
		_resultsLevelLabel.text = text;
		_resultsTitleLabel.text = TextDefs.MORTY_STORAGE_BOOTCAMP_CELL_RESULTS_TITLE;
		UpdateBootcampState();
	}

	private void PopulateMorties()
	{
		List<MortyData> mortiesForBootcamp = BootcampDefs.GetMortiesForBootcamp(bootcampIndex);
		for (int i = 0; i < 5; i++)
		{
			MortyData up = ((i >= mortiesForBootcamp.Count) ? null : mortiesForBootcamp[i]);
			base.transform.FindChild("Mortys/Morty" + i).GetComponent<DeckCellMorty>().SetUp(up);
		}
	}

	private void OnDestroy()
	{
		RemoveEvents();
	}

	private void AddEvents()
	{
	}

	private void RemoveEvents()
	{
	}

	private void UpdateBootcampState()
	{
		eBootcampState eBootcampState2 = BootcampDefs.GetBootcampState(bootcampIndex);
		if (eBootcampState2 != bootcampState)
		{
			bootcampState = eBootcampState2;
			switch (bootcampState)
			{
			case eBootcampState.IDLE:
				SetAsIdle();
				break;
			case eBootcampState.PROGRESS:
				SetAsInProgress();
				break;
			case eBootcampState.RESULT_AVAILABLE:
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
		_idleTabObj.SetActive(false);
		_inProgressTabObj.SetActive(false);
		_resultsTabObj.SetActive(true);
	}

	private void SetAsInProgress()
	{
		_idleTabObj.SetActive(false);
		_inProgressTabObj.SetActive(true);
		_resultsTabObj.SetActive(false);
	}

	private void SetAsIdle()
	{
		_idleTabObj.SetActive(true);
		_inProgressTabObj.SetActive(false);
		_resultsTabObj.SetActive(false);
		UpdateTimeLeftLabel(_idleTimeLabel, BootcampDefs.GetBootcampDuration(bootcampIndex));
	}

	private void Update()
	{
		UpdateBootcampState();
		UpdateTimeLeftLabel(_inProgressTimeLabel, BootcampDefs.GetBootcampTimeLeft(bootcampIndex));
	}

	private void UpdateTimeLeftLabel(Text label, double timeLeft)
	{
		label.text = TextDefs.AbbreviateTimestamp(timeLeft);
	}

	private void BootcampStateChangedHandler(int bootcampIndex, eBootcampState state)
	{
		if (bootcampIndex == this.bootcampIndex)
		{
			UpdateBootcampState();
		}
	}
}
