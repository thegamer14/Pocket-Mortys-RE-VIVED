using UnityEngine;

public class NodeMoveAnimatorModule : MonoBehaviour
{
	public int fps = 8;

	private Sprite[] _downSprites;

	private Sprite[] _upSprites;

	private Sprite[] _sideSprites;

	private Sprite[] _currentSprites;

	private int _currentSpriteIndex;

	private SpriteRenderer _renderer;

	private bool _isIdle;

	private bool _wasIdle;

	private float _animationCountdown;

	public eDirection direction { get; private set; }

	public void SetUp(string assetID, eDirection startingDirection = eDirection.DOWN)
	{
		_renderer = base.transform.FindChild("Sprite").GetComponent<SpriteRenderer>();
		SetUpSprites(assetID);
		_isIdle = true;
		_currentSpriteIndex = 0;
		SetDirection(startingDirection);
	}

	public void SetUpSprites(string assetID)
	{
		_downSprites = Resources.LoadAll<Sprite>(string.Format("World/NodeAssets/{0}/Down", assetID));
		_upSprites = Resources.LoadAll<Sprite>(string.Format("World/NodeAssets/{0}/Up", assetID));
		_sideSprites = Resources.LoadAll<Sprite>(string.Format("World/NodeAssets/{0}/Side", assetID));
	}

	private void Update()
	{
		if (!_isIdle)
		{
			_animationCountdown -= Time.deltaTime;
			if (_animationCountdown <= 0f)
			{
				SetNextSprite();
				_animationCountdown = (float)fps / 60f;
			}
		}
		else if (!_wasIdle)
		{
			_animationCountdown = 0f;
			SetSprite(0);
		}
		_wasIdle = _isIdle;
	}

	public void SetMoving(bool isMoving)
	{
		_isIdle = !isMoving;
	}

	public void SetDirection(eDirection val)
	{
		direction = val;
		switch (val)
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
		if (_currentSprites != null && _currentSprites.Length != 0)
		{
			_renderer.sprite = _currentSprites[index];
			_currentSpriteIndex = index;
		}
	}

	public void ShowSpriteRenderer(bool isShow)
	{
		_renderer.gameObject.SetActive(isShow);
	}
}
