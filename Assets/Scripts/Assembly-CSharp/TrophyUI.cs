using UnityEngine;
using UnityEngine.UI;

public class TrophyUI : MonoBehaviour
{
	public delegate void TrophyUIBtnDelegate(string id);

	private Image _trophyImage;

	private string _id;

	public event TrophyUIBtnDelegate PressedEvent;

	private void Awake()
	{
		_trophyImage = base.transform.FindChild("Image").GetComponent<Image>();
	}

	public void SetUp(string id)
	{
		_id = id;
		_trophyImage.sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/Trophies/{0}", id));
		_trophyImage.SetNativeSize();
	}

	public void PressedHandler()
	{
		if (this.PressedEvent != null)
		{
			this.PressedEvent(_id);
		}
	}
}
