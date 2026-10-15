using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MortyHPBar : MonoBehaviour
{
	private const float _kHPTweenSpeed = 10f;

	private Text _hpTitle;

	private Text _hpLabel;

	private Image _hpFill;

	private float _hpMax;

	private float _hp;

	private void Awake()
	{
		_hpTitle = base.transform.FindChild("Title").GetComponent<Text>();
		_hpLabel = base.transform.FindChild("Label").GetComponent<Text>();
		_hpFill = base.transform.FindChild("Fill").GetComponent<Image>();
	}

	private void Start()
	{
		_hpTitle.text = TextDefs.MORTY_STAT_HP;
	}

	public void SetHPMax(float hpMax, int hp = -1)
	{
		_hpMax = hpMax;
		if (hp > -1)
		{
			SetHP(hp);
		}
	}

	public void SetHP(float newHP)
	{
		_hp = newHP;
		_hpLabel.text = string.Format("{0}/{1}", Mathf.CeilToInt(_hp), _hpMax);
		SetHPBar();
	}

	private void SetHPBar()
	{
		_hpFill.transform.localScale = new Vector3(_hp / _hpMax, 1f, 1f);
		if (_hpFill.transform.localScale.x <= 0.25f)
		{
			_hpFill.color = Color.red;
		}
		else if (_hpFill.transform.localScale.x <= 0.5f)
		{
			_hpFill.color = Color.yellow;
		}
		else
		{
			_hpFill.color = Color.green;
		}
	}

	public IEnumerator SetHPWithTween(float newHP)
	{
		if (newHP != _hp)
		{
			float scaleX = newHP / _hpMax;
			Tween textTween = DOTween.To(duration: Mathf.Abs(scaleX - _hpFill.transform.localScale.x), getter: () => _hp, setter: delegate(float x)
			{
				_hp = x;
			}, endValue: newHP);
			textTween.OnUpdate(delegate
			{
				SetHP(_hp);
			});
			yield return textTween.WaitForCompletion();
		}
	}
}
