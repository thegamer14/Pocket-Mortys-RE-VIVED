using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : UIBase
{
	public delegate void UIMainMenuBtnDelegate();

	private GameObject FindGO(string path)
	{
		Transform t = FindPath(path);
		return (t == null) ? null : t.gameObject;
	}

	private Text FindLabel(string path)
	{
		Transform t = FindPath(path);
		return (t == null) ? null : t.GetComponent<Text>();
	}

	private RectTransform FindRT(string path)
	{
		Transform t = FindPath(path);
		return (t == null) ? null : t.GetComponent<RectTransform>();
	}

	private Transform FindPath(string path)
	{
		string[] parts = path.Split('/');
		Transform cur = base.transform;
		for (int i = 0; i < parts.Length; i++)
		{
			Transform next = null;
			for (int c = 0; c < cur.childCount; c++)
			{
				if (cur.GetChild(c).name == parts[i])
				{
					next = cur.GetChild(c);
					break;
				}
			}
			if (next == null)
			{
				return null;
			}
			cur = next;
		}
		return cur;
	}

	public UIMainMenuBtnDelegate CampaignEvent;

	public UIMainMenuBtnDelegate MultiplayerEvent;

	public UIMainMenuBtnDelegate SettingsEvent;

	public UIMainMenuBtnDelegate GoogleSignInEvent;

	private RectTransform _earthTrans;

	private RectTransform _moonTrans;

	private GameObject _googleSignInBtnObj;

	private Text _campaignBtnLabel;

	private Text _multiplayerBtnLabel;

	private Text _googleSignInBtnLabel;

	protected override void LateAwake()
	{
		_googleSignInBtnObj = FindGO("GoogleBtn");
		_campaignBtnLabel = FindLabel("CampaignBtn/Label");
		_multiplayerBtnLabel = FindLabel("MultiplayerBtn/Label");
		_googleSignInBtnLabel = FindLabel("GoogleBtn/Image/Label");
		_earthTrans = FindRT("Background/Earth");
		_moonTrans = FindRT("Background/Moon");
	}

	public void SetUp()
	{
		if (_earthTrans != null)
		{
			Tween t = _earthTrans.DOLocalRotate(new Vector3(0f, 0f, 360f), 100f, RotateMode.LocalAxisAdd);
			t.SetEase(Ease.Linear);
			t.SetLoops(-1, LoopType.Restart);
		}
		if (_moonTrans != null)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_moonTrans.DOLocalMove(new Vector3(-845f, 0f, 0f), 25f));
			sequence.AppendInterval(100f);
			sequence.SetLoops(-1, LoopType.Restart);
			sequence.Play();
		}
	}

	public override void SetLabels()
	{
		if (_campaignBtnLabel != null)
		{
			_campaignBtnLabel.text = TextDefs.MAIN_MENU_BTN_CAMPAIGN_TITLE;
		}
		if (_multiplayerBtnLabel != null)
		{
			_multiplayerBtnLabel.text = TextDefs.MAIN_MENU_BTN_MULTIPLAYER_TITLE;
		}
		if (_googleSignInBtnLabel != null)
		{
			_googleSignInBtnLabel.text = TextDefs.MAIN_MENU_BTN_SIGN_IN;
		}
	}

	protected override void AddEvents()
	{
	}

	protected override void RemoveEvents()
	{
	}

	public override void ManualUpdate(float deltaTime)
	{
		if (_googleSignInBtnObj != null)
		{
			_googleSignInBtnObj.SetActive(!App.social.IsAuthenticated());
		}
	}

	public void PlayBtnHandler()
	{
		CampaignBtnHandler();
	}

	public void CampaignBtnHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (CampaignEvent != null)
		{
			CampaignEvent();
		}
	}

	public void MultiplayerBtnHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (MultiplayerEvent != null)
		{
			MultiplayerEvent();
		}
	}

	public void SettingsBtnHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (SettingsEvent != null)
		{
			SettingsEvent();
		}
	}

	public void GoogleSignInBtnHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (GoogleSignInEvent != null)
		{
			GoogleSignInEvent();
		}
	}
}
