using UnityEngine.UI;

public class ItemDescriptionCell : ItemCell
{
	private Text _descriptionLabel;

	protected override void Awake()
	{
		base.Awake();
		_descriptionLabel = base.transform.FindChild("DescriptionLabel").GetComponent<Text>();
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		if (value)
		{
			_descriptionLabel.text = _description;
		}
		else
		{
			_descriptionLabel.gameObject.SetActive(false);
		}
	}
}
