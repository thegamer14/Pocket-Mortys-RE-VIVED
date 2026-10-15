using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPMortyCell : CellBase
{
	private Text _nameLabel;

	private Image _iconImage;

	protected MortyElementImage _elementTypeImage;

	private MortyHPBar _hpBar;

	private MortyLevelBar _levelBar;

	private MortyEvolveBar _evolveBar;

	private MPMortyStatusBar _statusBar;

	private GameObject _outBar;

	private Text _outBarLabel;

	[HideInInspector]
	public MPOpponentMortyData data;

	public bool isOut;

	public bool showHealthBar;

	public bool showEvolveBar;

	protected override void Awake()
	{
		base.Awake();
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
		_iconImage = base.transform.FindChild("Icon").GetComponent<Image>();
		_elementTypeImage = base.transform.FindChild("ElementType").GetComponent<MortyElementImage>();
		_hpBar = base.transform.FindChild("LowerContainer/HPBar").GetComponent<MortyHPBar>();
		_levelBar = base.transform.FindChild("LowerContainer/LevelBar").GetComponent<MortyLevelBar>();
		_evolveBar = base.transform.FindChild("LowerContainer/EvolveBar").GetComponent<MortyEvolveBar>();
		_statusBar = base.transform.FindChild("StatusBar").GetComponent<MPMortyStatusBar>();
		_outBar = base.transform.FindChild("OutBar").gameObject;
		_outBarLabel = _outBar.transform.FindChild("Label").GetComponent<Text>();
	}

	public void SetUp(MPOpponentMortyData data, bool isOut = false, bool showHealthBar = true, bool showEvolveBar = false)
	{
		this.data = data;
		this.isOut = isOut;
		this.showHealthBar = showHealthBar;
		this.showEvolveBar = showEvolveBar;
		base.SetUp((data == null) ? null : data.netId);
		SetUsed(data != null);
		_outBarLabel.text = TextDefs.PARTY_CELL_OUT_BAR;
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		Refresh(data, isOut);
	}

	public virtual void Refresh(MPOpponentMortyData data, bool isOut, bool withStatusAnim = false)
	{
		this.data = data;
		this.isOut = isOut;
		if (data != null)
		{
			_nameLabel.gameObject.SetActive(true);
			_iconImage.gameObject.SetActive(true);
			_elementTypeImage.SetActive(true);
			_hpBar.gameObject.SetActive(showHealthBar);
			_levelBar.gameObject.SetActive(true);
			_evolveBar.gameObject.SetActive(showEvolveBar);
			_outBar.SetActive(isOut);
			_nameLabel.text = MPMortyDefs.Name(data.id);
			SetElementImage(MPMortyDefs.ElementType(data.id));
			_hpBar.SetHPMax(data.hpStat);
			_hpBar.SetHP(data.hp);
			_evolveBar.SetUp(data.evolutionPoints, MPMortyDefs.EvolutionRequirement(data.id));
			_levelBar.SetLevel(data.level);
			_iconImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPMortyDefs.AssetID(data.id)));
			_iconImage.SetNativeSize();
			_statusBar.SetUp(data, withStatusAnim);
		}
		else
		{
			_nameLabel.gameObject.SetActive(false);
			_iconImage.gameObject.SetActive(false);
			_elementTypeImage.SetActive(false);
			_hpBar.gameObject.SetActive(false);
			_levelBar.gameObject.SetActive(false);
			_evolveBar.gameObject.SetActive(false);
			_statusBar.SetUp(null);
			_outBar.SetActive(false);
		}
	}

	private IEnumerator SetStatusBarPosition()
	{
		yield return new WaitForEndOfFrame();
		_statusBar.transform.localPosition = new Vector3(_nameLabel.rectTransform.localPosition.x + _nameLabel.rectTransform.sizeDelta.x + 30f, _statusBar.transform.localPosition.y);
	}

	public IEnumerator TweenToHP(float value)
	{
		yield return StartCoroutine(_hpBar.SetHPWithTween(value));
	}

	public void SetEvolutionPoints(int value)
	{
		_evolveBar.SetEvolutionPoints(value);
	}

	public IEnumerator TweenToEvolutionPoints(int value)
	{
		yield return StartCoroutine(_evolveBar.SetEvolutionPointsWithTween(value));
	}

	public void PulseLevelBar()
	{
		_levelBar.transform.DOPunchScale(Vector3.one * 0.4f, 0.6f, 0, 0f);
	}

	protected virtual void SetElementImage(string type)
	{
		_elementTypeImage.Set(type, true);
	}
}
