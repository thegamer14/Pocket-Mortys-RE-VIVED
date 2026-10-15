using UnityEngine;
using UnityEngine.UI;

public class MPTutorialPopupUI : UIBase
{
	public delegate void ButtonClickedDelegate();

	private Text _textLabel;

	public event ButtonClickedDelegate CloseButtonClickedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		_textLabel = base.transform.FindChild("Box/Label").GetComponent<Text>();
	}

	public void SetUp(string text)
	{
		_textLabel.text = text;
	}

	public void ButtonClickedHandler(GameObject btn)
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.CloseButtonClickedEvent != null)
		{
			this.CloseButtonClickedEvent();
		}
	}
}
