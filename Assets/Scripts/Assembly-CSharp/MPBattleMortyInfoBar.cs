using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPBattleMortyInfoBar : MonoBehaviour
{
	private const int _kInPositionX = -620;

	private const int _kOutPositionX = -1540;

	private const int _kDownPositionY = 60;

	private const float _kInOutDuration = 0.6f;

	private const float _kUpDownDuration = 0.4f;

	[HideInInspector]
	public int moveDirection;

	private float _startingPositionY;

	private RectTransform _rectTrans;

	private Transform _usernameBar;

	private Text _nameLabel;

	private MortyElementImage _elementTypeImage;

	private MortyHPBar _hpBar;

	private MortyLevelBar _levelBar;

	private MPMortyStatusBar _statusBar;

	private Transform _caughtIcon;

	private Transform _ownedIcon;

	private Transform _partyTrans;

	private Image[] _partyImages;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_usernameBar = base.transform.FindChild("UsernameBar");
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
		_elementTypeImage = base.transform.FindChild("ElementType").GetComponent<MortyElementImage>();
		_hpBar = base.transform.FindChild("HPBar").GetComponent<MortyHPBar>();
		_levelBar = base.transform.FindChild("LevelBar").GetComponent<MortyLevelBar>();
		_statusBar = base.transform.FindChild("StatusBar").GetComponent<MPMortyStatusBar>();
		_caughtIcon = base.transform.FindChild("CaughtIcon");
		_ownedIcon = base.transform.FindChild("OwnedIcon");
		_partyTrans = base.transform.FindChild("PartyBar");
		if (_partyTrans != null)
		{
			_partyImages = _partyTrans.GetComponentsInChildren<Image>();
		}
		_startingPositionY = base.transform.localPosition.y;
		SetUsername(null);
	}

	public void SetAll(List<MPOpponentMortyData> mortyDatas, MPOpponentMortyData currentMortyData)
	{
		SetName(MPMortyDefs.Name(currentMortyData.id));
		_elementTypeImage.Set(MPMortyDefs.ElementType(currentMortyData.id));
		_hpBar.SetHPMax(currentMortyData.hpStat, currentMortyData.hp);
		_levelBar.SetXPTarget(MPMortyDefs.GetXPTarget(currentMortyData.level), MPMortyDefs.GetXPTarget(currentMortyData.level + 1), currentMortyData.xp);
		_levelBar.SetLevel(currentMortyData.level);
		_statusBar.SetUp(currentMortyData);
		SetCaughtIndicator(eCaughtIndicatorType.NONE);
		SetParty(mortyDatas);
	}

	public void SetUsername(string username)
	{
		if (_usernameBar != null)
		{
			if (username != null)
			{
				_usernameBar.gameObject.SetActive(true);
				_usernameBar.FindChild("Label").GetComponent<Text>().text = username;
			}
			else
			{
				_usernameBar.gameObject.SetActive(false);
			}
		}
	}

	public void SetName(string name)
	{
		_nameLabel.text = name;
	}

	public void SetHPMax(float hpMax, int hp = -1)
	{
		_hpBar.SetHPMax(hpMax, hp);
	}

	public void SetHP(float newHP)
	{
		_hpBar.SetHP(newHP);
	}

	public IEnumerator SetHPWithTween(float newHP)
	{
		yield return StartCoroutine(_hpBar.SetHPWithTween(newHP));
	}

	public void ActivateLeveling(bool isActive)
	{
		_levelBar.SetXpHolderActive(isActive);
	}

	public void SetXPTarget(float from, float to, int xp = -1)
	{
		_levelBar.SetXPTarget(from, to, xp);
	}

	public void SetXP(float newXP)
	{
		_levelBar.SetXP(newXP);
	}

	public IEnumerator SetXPWithTween(float newXP)
	{
		yield return StartCoroutine(_levelBar.SetXPWithTween(newXP));
	}

	public void SetLevel(int level)
	{
		_levelBar.SetLevel(level);
	}

	public void SetStatusBar(MPOpponentMortyData mortyData, bool withAnim = false)
	{
		_statusBar.SetUp(mortyData, withAnim);
	}

	public void SetCaughtIndicator(eCaughtIndicatorType indicatorType, bool withPopAnim = false)
	{
		if (_caughtIcon == null || _ownedIcon == null)
		{
			return;
		}
		switch (indicatorType)
		{
		case eCaughtIndicatorType.CAUGHT:
			_caughtIcon.gameObject.SetActive(true);
			_ownedIcon.gameObject.SetActive(false);
			if (withPopAnim)
			{
				_caughtIcon.transform.localScale = Vector3.zero;
				_caughtIcon.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
			}
			break;
		case eCaughtIndicatorType.OWNED:
			_caughtIcon.gameObject.SetActive(false);
			_ownedIcon.gameObject.SetActive(true);
			if (withPopAnim)
			{
				_ownedIcon.transform.localScale = Vector3.zero;
				_ownedIcon.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
			}
			break;
		default:
			_caughtIcon.gameObject.SetActive(false);
			_ownedIcon.gameObject.SetActive(false);
			break;
		}
	}

	public void SetPartyActive(bool value)
	{
		if (!(_partyTrans == null))
		{
			_partyTrans.gameObject.SetActive(value);
		}
	}

	public void SetParty(List<MPOpponentMortyData> mortyDatas)
	{
		if (_partyTrans == null)
		{
			return;
		}
		for (int i = 0; i < _partyImages.Length; i++)
		{
			if (i < mortyDatas.Count)
			{
				if (mortyDatas[i].hp > 0)
				{
					_partyImages[i].sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/PartyStateIcon/{0}", "Alive"));
				}
				else
				{
					_partyImages[i].sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/PartyStateIcon/{0}", "Fainted"));
				}
			}
			else
			{
				_partyImages[i].sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/PartyStateIcon/{0}", "Unavailable"));
			}
		}
	}

	public void SetPositionIn()
	{
		_rectTrans.anchoredPosition = new Vector2(-620 * moveDirection, _rectTrans.anchoredPosition.y);
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector2(-1540 * moveDirection, _rectTrans.anchoredPosition.y);
	}

	public void SetPositionDownY()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY - 60f);
	}

	public void SetPositionToStartingY()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY);
	}

	public void TweenIn()
	{
		StartCoroutine(TweenInCo());
	}

	public IEnumerator TweenInCo()
	{
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(-620 * moveDirection, _rectTrans.anchoredPosition.y), 0.6f);
		yield return tween.WaitForCompletion();
	}

	public void TweenOut()
	{
		StartCoroutine(TweenOutCo());
	}

	public IEnumerator TweenOutCo()
	{
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(-1540 * moveDirection, _rectTrans.anchoredPosition.y), 0.6f);
		yield return tween.WaitForCompletion();
	}

	public void TweenDownY()
	{
		_rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY - 60f), 0.4f);
	}

	public void TweenToStartingY()
	{
		_rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY), 0.4f);
	}
}
