using UnityEngine;
using UnityEngine.UI;

public class MPFriendCellLevelBar : MonoBehaviour
{
	private Text _title;

	private Text _label;

	private void Awake()
	{
		_title = base.transform.FindChild("Title").GetComponent<Text>();
		_label = base.transform.FindChild("Label").GetComponent<Text>();
	}

	public void SetUp(int level)
	{
		_title.text = TextDefs.MORTY_LEVEL_ABBREV;
		_label.text = level.ToString();
	}
}
