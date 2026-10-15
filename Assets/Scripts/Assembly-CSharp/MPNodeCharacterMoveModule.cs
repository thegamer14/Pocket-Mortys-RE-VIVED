using UnityEngine;

public class MPNodeCharacterMoveModule : MPNodeMoveModule
{
	public const float kDefaultZDepth = 0.25f;

	public const float kUserZDepth = 0.248f;

	public const float kUserMortyZDepth = 0.249f;

	public const float kPlayerZDepth = 0.246f;

	public const float kPlayerMortyZDepth = 0.247f;

	private const int _kFps = 8;

	private Sprite[] _downSprites;

	private Sprite[] _upSprites;

	private Sprite[] _sideSprites;

	private Sprite[] _currentSprites;

	private int _currentSpriteIndex;

	private SpriteRenderer _renderer;

	private SpriteRenderer _shadow;

	private float _animationCountdown;

	private bool _isPlaying;

	private bool _wasPlaying;

	public void SetUp(Coord newCoord, eDirection direction, string assetId)
	{
		_renderer = base.transform.FindChild("Sprite").GetComponent<SpriteRenderer>();
		_shadow = base.transform.FindChild("Shadow").GetComponent<SpriteRenderer>();
		LoadSprites(assetId);
		_currentSpriteIndex = 0;
		SetUp(newCoord, direction);
	}

	public override void SetDirection(eDirection newDirection)
	{
		base.direction = newDirection;
		switch (base.direction)
		{
		case eDirection.DOWN:
			_currentSprites = _downSprites;
			_renderer.transform.localScale = new Vector3(1f, _renderer.transform.localScale.y, _renderer.transform.localScale.z);
			break;
		case eDirection.UP:
			_currentSprites = _upSprites;
			_renderer.transform.localScale = new Vector3(1f, _renderer.transform.localScale.y, _renderer.transform.localScale.z);
			break;
		case eDirection.RIGHT:
			_currentSprites = _sideSprites;
			_renderer.transform.localScale = new Vector3(-1f, _renderer.transform.localScale.y, _renderer.transform.localScale.z);
			break;
		case eDirection.LEFT:
			_currentSprites = _sideSprites;
			_renderer.transform.localScale = new Vector3(1f, _renderer.transform.localScale.y, _renderer.transform.localScale.z);
			break;
		}
		SetSprite(_currentSpriteIndex);
	}

	public void SetZDepth(float amount)
	{
		_renderer.transform.localPosition = new Vector3(_renderer.transform.localPosition.x, _renderer.transform.localPosition.y, amount);
	}

	public override void UpdateMovement()
	{
		base.UpdateMovement();
		_wasPlaying = _isPlaying;
		_isPlaying = base.isMoving || base.wasMoving;
		if (_isPlaying)
		{
			_animationCountdown -= Time.deltaTime;
			if (_animationCountdown <= 0f)
			{
				SetNextSprite();
				_animationCountdown = 2f / 15f;
			}
		}
		else if (_wasPlaying)
		{
			_animationCountdown = 0f;
			SetSprite(0);
		}
	}

	public void LoadSprites(string assetID)
	{
		_downSprites = Resources.LoadAll<Sprite>(string.Format("World/NodeAssets/{0}/Down", assetID));
		_upSprites = Resources.LoadAll<Sprite>(string.Format("World/NodeAssets/{0}/Up", assetID));
		_sideSprites = Resources.LoadAll<Sprite>(string.Format("World/NodeAssets/{0}/Side", assetID));
		if (_currentSprites != null && base.direction != eDirection.NONE)
		{
			SetDirection(base.direction);
		}
	}

	private void SetNextSprite()
	{
		if (_currentSprites != null && _currentSprites.Length != 0)
		{
			_currentSpriteIndex = (_currentSpriteIndex + 1) % _currentSprites.Length;
			_renderer.sprite = _currentSprites[_currentSpriteIndex];
		}
	}

	private void SetSprite(int index)
	{
		if (_currentSprites != null && _currentSpriteIndex < _currentSprites.Length)
		{
			_renderer.sprite = _currentSprites[index];
			_currentSpriteIndex = index;
		}
		else
		{
			_renderer.sprite = null;
			_currentSpriteIndex = 0;
		}
	}

	public void ShowSprite(bool isShow)
	{
		_renderer.gameObject.SetActive(isShow);
		if (_shadow != null)
		{
			_shadow.gameObject.SetActive(isShow);
		}
	}
}
