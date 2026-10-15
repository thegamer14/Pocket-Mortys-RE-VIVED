using UnityEngine;
using UnityEngine.UI;

public class MPFriendRatioCellBar : MonoBehaviour
{
	private Text _winText;

	private Text _lossText;

	private void Awake()
	{
		_winText = base.transform.FindChild("Background/Win").GetComponent<Text>();
		_lossText = base.transform.FindChild("Background/Loss").GetComponent<Text>();
	}

	public void SetUp(int win, int loss)
	{
		_winText.text = win.ToString();
		_lossText.text = loss.ToString();
	}
}
