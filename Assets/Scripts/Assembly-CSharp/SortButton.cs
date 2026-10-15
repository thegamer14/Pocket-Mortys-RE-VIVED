using UnityEngine;
using UnityEngine.UI;

public class SortButton : MonoBehaviour
{
	private Text _text;

	private GameObject _arrow;

	private void Awake()
	{
		_arrow = base.transform.FindChild("Arrow").gameObject;
		_text = base.transform.FindChild("Text").GetComponent<Text>();
	}

	public void SetText(string val)
	{
		_text.text = val;
	}

	public void SetArrowState(eSortButtonState val)
	{
		switch (val)
		{
		case eSortButtonState.NONE:
			_arrow.SetActive(false);
			break;
		case eSortButtonState.DESCENDING:
			_arrow.SetActive(true);
			_arrow.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case eSortButtonState.ASCENDING:
			_arrow.SetActive(true);
			_arrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
			break;
		}
	}
}
