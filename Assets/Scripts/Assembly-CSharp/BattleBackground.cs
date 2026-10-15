using UnityEngine;
using UnityEngine.UI;

public class BattleBackground : MonoBehaviour
{
	private Image _background;

	private RawImage _overlay;

	private RawImageTileAndScroll _overlayScroll;

	private void Awake()
	{
		_background = base.transform.FindChild("Background").GetComponent<Image>();
		_overlay = base.transform.FindChild("Overlay1").GetComponent<RawImage>();
		_overlayScroll = _overlay.GetComponent<RawImageTileAndScroll>();
	}

	public void SetBackground(string worldID)
	{
		SetBackground(worldID, worldID);
	}

	public void SetBackground(string worldID, string nodeSetID)
	{
		if (worldID.Equals("Tutorial"))
		{
			worldID = nodeSetID;
		}
		Sprite sprite = Resources.Load<Sprite>(string.Format("UI/Battle/BackgroundSprites/{0}/{1}", worldID, "Background"));
		Texture texture = Resources.Load<Texture>(string.Format("UI/Battle/BackgroundSprites/{0}/{1}", nodeSetID, "Overlay"));
		if (sprite == null)
		{
			sprite = Resources.Load<Sprite>(string.Format("UI/Battle/BackgroundSprites/{0}/{1}", "Default", "Background"));
		}
		if (texture == null)
		{
			texture = Resources.Load<Texture>(string.Format("UI/Battle/BackgroundSprites/{0}/{1}", "Default", "Overlay"));
		}
		_background.sprite = sprite;
		_overlay.texture = texture;
	}

	public void SetSpeed(float speed)
	{
		_overlayScroll.scrollSpeedX = speed;
	}
}
