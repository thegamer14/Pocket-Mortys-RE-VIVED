using UnityEngine;
using UnityEngine.UI;

public class BadgeUI : MonoBehaviour
{
	private Image _badgeImage;

	private void Awake()
	{
		_badgeImage = base.transform.FindChild("Badge").GetComponent<Image>();
	}

	public void SetUp(string spriteResource, bool shift = true)
	{
		if (!string.IsNullOrEmpty(spriteResource))
		{
			_badgeImage.sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/Badges/{0}", spriteResource));
			if (shift)
			{
				ShiftBadge();
			}
		}
		else
		{
			_badgeImage.gameObject.SetActive(false);
		}
	}

	private void ShiftBadge()
	{
		_badgeImage.rectTransform.Rotate(0f, 0f, Random.value * 30f - 15f);
		_badgeImage.rectTransform.anchoredPosition = Random.value * new Vector3(4f, 0f, 4f) - new Vector3(8f, 0f, 8f);
	}
}
