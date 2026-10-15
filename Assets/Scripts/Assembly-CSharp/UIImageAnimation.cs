using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimation : MonoBehaviour
{
	public enum eLoopType
	{
		None = 0,
		Repeat = 1,
		PingPong = 2
	}

	public bool play = true;

	public int fps = 60;

	public eLoopType loopType;

	public Sprite[] sprites;

	public bool setNativeSize = true;

	private Image _image;

	private float _animationCountdown;

	private int incrementAmount = 1;

	public int spriteIndex { get; private set; }

	private void Awake()
	{
		_image = GetComponent<Image>();
		_animationCountdown = (float)fps / 60f;
		if (play && sprites.Length > 0)
		{
			SetSprite(spriteIndex);
		}
	}

	private void Update()
	{
		if (play)
		{
			_animationCountdown -= Time.deltaTime;
			if (_animationCountdown <= 0f)
			{
				SetNextSprite();
				_animationCountdown = (float)fps / 60f;
			}
		}
	}

	public void SetNextSprite()
	{
		switch (loopType)
		{
		case eLoopType.None:
			spriteIndex++;
			play = spriteIndex < sprites.Length;
			break;
		case eLoopType.Repeat:
			spriteIndex++;
			spriteIndex %= sprites.Length;
			break;
		case eLoopType.PingPong:
			spriteIndex += incrementAmount;
			if (spriteIndex <= 0 || spriteIndex >= sprites.Length - 1)
			{
				incrementAmount *= -1;
			}
			break;
		}
		if (play)
		{
			SetSprite(spriteIndex);
		}
	}

	public void SetSprite(int index)
	{
		spriteIndex = index;
		_image.sprite = sprites[spriteIndex];
		_image.enabled = sprites[spriteIndex] != null;
		if (setNativeSize)
		{
			_image.SetNativeSize();
		}
	}
}
