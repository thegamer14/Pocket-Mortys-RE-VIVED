using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MortyCell : CellBase
{
	private Text _nameLabel;

	private Image _iconImage;

	protected MortyElementImage _elementTypeImage;

	private MortyHPBar _hpBar;

	private MortyLevelBar _levelBar;

	private MortyStatusBar _statusBar;

	private GameObject _outBar;

	private Text _outBarLabel;

	[HideInInspector]
	public MortyData data;

	protected override void Awake()
	{
		base.Awake();
		_nameLabel = base.transform.FindChild("NameLabel").GetComponent<Text>();
		_iconImage = base.transform.FindChild("Icon").GetComponent<Image>();
		_elementTypeImage = base.transform.FindChild("ElementType").GetComponent<MortyElementImage>();
		_hpBar = base.transform.FindChild("HPBar").GetComponent<MortyHPBar>();
		_levelBar = base.transform.FindChild("LevelBar").GetComponent<MortyLevelBar>();
		_statusBar = base.transform.FindChild("StatusBar").GetComponent<MortyStatusBar>();
		_outBar = base.transform.FindChild("OutBar").gameObject;
		_outBarLabel = _outBar.transform.FindChild("Label").GetComponent<Text>();
	}

	public void SetUp(MortyData data, bool isOut = false)
	{
		this.data = data;
		base.SetUp((data == null) ? null : data.id);
		SetUsed(data != null);
		_outBarLabel.text = TextDefs.PARTY_CELL_OUT_BAR;
		_outBar.SetActive(isOut);
	}

	public override void SetUsed(bool value)
	{
		base.SetUsed(value);
		Refresh();
	}

	public virtual void Refresh(MortyData newData = null, bool withStatusAnim = false, bool isOut = false)
	{
		if (newData != null)
		{
			data = newData;
		}
		if (data != null)
		{
			_nameLabel.gameObject.SetActive(true);
			_iconImage.gameObject.SetActive(true);
			_elementTypeImage.SetActive(true);
			_hpBar.gameObject.SetActive(true);
			_levelBar.gameObject.SetActive(true);
			_outBar.SetActive(isOut);
			_nameLabel.text = MortyDefs.Name(data.id);
			SetElementImage(MortyDefs.ElementType(data.id));
			_hpBar.SetHPMax(data.hpStat);
			_hpBar.SetHP(data.hp);
			_levelBar.SetLevel(data.level);
			_iconImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MortyDefs.AssetID(data.id)));
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

	public void PulseLevelBar()
	{
		_levelBar.transform.DOPunchScale(Vector3.one * 0.4f, 0.6f, 0, 0f);
	}

	protected virtual void SetElementImage(string type)
	{
		_elementTypeImage.Set(type, true);
	}
}
