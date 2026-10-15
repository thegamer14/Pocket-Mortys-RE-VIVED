using UnityEngine;

public class MPUserNodeLevel : MonoBehaviour
{
	private TextMesh _titleLabel;

	private TextMesh _valueLabel;

	private void Awake()
	{
		_titleLabel = base.transform.FindChild("Title").GetComponent<TextMesh>();
		_valueLabel = base.transform.FindChild("Value").GetComponent<TextMesh>();
	}

	private void Start()
	{
		_titleLabel.text = TextDefs.MORTY_LEVEL_ABBREV;
	}

	public void SetLevel(int level)
	{
		_valueLabel.text = level.ToString();
	}
}
