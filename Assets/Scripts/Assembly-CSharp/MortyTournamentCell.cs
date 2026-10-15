using UnityEngine.UI;

public class MortyTournamentCell : MortyCell
{
	public delegate void MortyTournamentCellBtnDelegate(MortyData mortyData);

	private Text _costLabel;

	public event MortyTournamentCellBtnDelegate StatsBtnEvent;

	protected override void Awake()
	{
		base.Awake();
		_costLabel = base.transform.FindChild("Cost/Label").GetComponent<Text>();
	}

	public void SetUp(MortyData data, bool isOut, int cost)
	{
		SetUp(data, isOut);
		_costLabel.text = cost.ToString();
		SetUsed(base.id != null);
	}

	public override void Refresh(MortyData newData = null, bool withStatusAnim = false, bool isOut = false)
	{
		base.Refresh(newData, withStatusAnim, isOut);
		if (data != null)
		{
			_costLabel.gameObject.SetActive(true);
		}
		else
		{
			_costLabel.gameObject.SetActive(false);
		}
	}

	protected override void SetElementImage(string type)
	{
		_elementTypeImage.Set(type);
	}

	public void StatsBtnPressed()
	{
		if (this.StatsBtnEvent != null)
		{
			this.StatsBtnEvent(data);
		}
	}
}
