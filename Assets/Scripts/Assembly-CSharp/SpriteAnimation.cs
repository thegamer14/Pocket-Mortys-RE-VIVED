using System.Collections;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
	public enum eLoopType
	{
		None = 0,
		Repeat = 1,
		PingPong = 2
	}

	public bool isPlaying = true;

	public float delay;

	public int fps = 60;

	public eLoopType loopType;

	public Sprite[] sprites;

	private SpriteRenderer _spriteRenderer;

	private float _animationCountdown;

	private int _spriteIndex;

	private int incrementAmount = 1;

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		if (isPlaying && sprites.Length > 0)
		{
			Play();
		}
	}

	private void Update()
	{
		if (isPlaying)
		{
			_animationCountdown -= Time.deltaTime;
			if (_animationCountdown <= 0f)
			{
				SetNextSprite();
				_animationCountdown = (float)fps / 60f;
			}
		}
	}

	public void Play()
	{
		if (!isPlaying)
		{
			_spriteIndex = 0;
			_animationCountdown = (float)fps / 60f;
			SetSprite(_spriteIndex);
			if (delay > 0f)
			{
				StartCoroutine(PlayCo());
			}
			else
			{
				isPlaying = true;
			}
		}
	}

	private IEnumerator PlayCo()
	{
		yield return new WaitForSeconds(delay);
		isPlaying = true;
	}

	public void Stop()
	{
		StopAllCoroutines();
		isPlaying = false;
	}

	public void SetNextSprite()
	{
		switch (loopType)
		{
		case eLoopType.None:
			_spriteIndex++;
			isPlaying = _spriteIndex < sprites.Length;
			break;
		case eLoopType.Repeat:
			_spriteIndex++;
			_spriteIndex %= sprites.Length;
			break;
		case eLoopType.PingPong:
			_spriteIndex += incrementAmount;
			if (_spriteIndex <= 0 || _spriteIndex >= sprites.Length - 1)
			{
				incrementAmount *= -1;
			}
			break;
		}
		if (isPlaying)
		{
			SetSprite(_spriteIndex);
		}
		if (loopType == eLoopType.Repeat && _spriteIndex == 0)
		{
			Stop();
			Play();
		}
	}

	public void SetSprite(int index)
	{
		_spriteIndex = index;
		_spriteRenderer.sprite = sprites[_spriteIndex];
	}
}
