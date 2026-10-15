using UnityEngine.UI;

public class HeaderCell : CellBase
{
	private Text _titleLabel;

	protected override void Awake()
	{
		base.Awake();
		_titleLabel = base.transform.FindChild("TitleLabel").GetComponent<Text>();
	}

	public override void SetUp(string text)
	{
		base.SetUp(null);
		_titleLabel.text = text;
		SetEnabled(false);
	}
}
