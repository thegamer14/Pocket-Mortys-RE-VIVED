using UnityEngine;
using UnityEngine.UI;

public class MPMortyStatsUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private Image _icon;

	private Text _number;

	private Text _name;

	private Image _gender;

	private MortyElementImage _element;

	private Image _morty;

	private MortyHPBar _hpBar;

	private MortyLevelBar _levelBar;

	private MPMortyStatusBar _statusBar;

	private MortyStatsBox _statsBox;

	private BattleAttackMenu _attackMenu;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private MortyEvolveBar _evolveBar;

	private MPOpponentMortyData _mortyData;

	public Sprite[] genderSprites;

	public event BagUIBtnDelegate ReleaseEvent;

	public event BagUIBtnDelegate AboutEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_icon = base.transform.FindChild("NumberBar/Icon").GetComponent<Image>();
		_number = base.transform.FindChild("NumberBar/Label").GetComponent<Text>();
		_name = base.transform.FindChild("TitleBar/Label").GetComponent<Text>();
		_gender = base.transform.FindChild("NumberBar/Gender").GetComponent<Image>();
		_element = base.transform.FindChild("TitleBar/Element").GetComponent<MortyElementImage>();
		_morty = base.transform.FindChild("Morty").GetComponent<Image>();
		_hpBar = base.transform.FindChild("Box/HPBar").GetComponent<MortyHPBar>();
		_levelBar = base.transform.FindChild("Box/LevelBar").GetComponent<MortyLevelBar>();
		_statusBar = base.transform.FindChild("TitleBar/StatusBar").GetComponent<MPMortyStatusBar>();
		_statsBox = base.transform.FindChild("Box/Stats").GetComponent<MortyStatsBox>();
		_attackMenu = base.transform.FindChild("Box/AttackMenu").GetComponent<BattleAttackMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_evolveBar = base.transform.FindChild("Box/EvolveBar").GetComponent<MortyEvolveBar>();
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(string.Format("UI/MPBattle/{0}", "Timer")));
		gameObject.transform.SetParent(base.transform);
	}

	public void SetUp(MPOpponentMortyData mortyData, bool allowRelease)
	{
		_mortyData = mortyData;
		SetNumberBar();
		SetTitleBar();
		SetMorty();
		SetEvolveBar();
		SetHPBar();
		SetLevelBar();
		SetStatusBar();
		SetStatsBox();
		RefreshAttackMenu();
		_mainMenu.SetPositionIn();
		if (MPMortyDefs.GetGender(_mortyData.id) == eMortyGender.FEMALE)
		{
			_gender.sprite = genderSprites[1];
			_gender.SetNativeSize();
		}
		_mainMenu.SetBtnVisible(0, allowRelease);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_STATS_BTN_RELEASE);
		_mainMenu.SetBtnTitle(1, TextDefs.MORTY_STATS_BTN_ABOUT);
		_dialogue.SetText(TextDefs.MORTY_STATS_DIAL);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public void SetNumberBar()
	{
		_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPMortyDefs.AssetID(_mortyData.id)));
		_icon.SetNativeSize();
		_number.text = MPMortyDefs.GetMortyNumberText(_mortyData.id);
	}

	public void SetTitleBar()
	{
		_name.text = MPMortyDefs.Name(_mortyData.id);
		_element.Set(MPMortyDefs.ElementType(_mortyData.id), true);
	}

	public void SetMorty()
	{
		_morty.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPMortyDefs.AssetID(_mortyData.id), "frontFlipped"));
		if (_morty.sprite == null)
		{
			_morty.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPMortyDefs.AssetID(_mortyData.id), "front"));
		}
		_morty.SetNativeSize();
	}

	public void SetEvolveBar()
	{
		_evolveBar.SetUp(_mortyData.evolutionPoints, MPMortyDefs.EvolutionRequirement(_mortyData.id));
	}

	public void SetHPBar()
	{
		_hpBar.SetHPMax(_mortyData.hpStat, _mortyData.hp);
	}

	public void SetLevelBar()
	{
		_levelBar.SetXPTarget(MPMortyDefs.GetXPTarget(_mortyData.level), MPMortyDefs.GetXPTarget(_mortyData.level + 1), _mortyData.xp);
		_levelBar.SetLevel(_mortyData.level);
	}

	public void SetStatusBar()
	{
		_statusBar.SetUp(_mortyData);
	}

	public void SetStatsBox()
	{
		_statsBox.Set(_mortyData.hpStat, _mortyData.attackStat, _mortyData.defenceStat, _mortyData.speedStat);
	}

	private void RefreshAttackMenu()
	{
		for (int i = 0; i < 4; i++)
		{
			if (_mortyData.attacks.ContainsKey(i))
			{
				_attackMenu.SetBtnEnabled(i, true);
				_attackMenu.SetUpBtn(i, new AttackDisplayInfo(_mortyData.attacks[i].id, _mortyData.attacks[i].PP));
			}
			else
			{
				_attackMenu.SetBtnEnabled(i, false);
			}
		}
	}

	public void TryRelease()
	{
		if (App.net.player.data.IsMortyInParty(_mortyData) && App.net.player.data.GetActiveDeckCount() == 1)
		{
			_dialogue.TweenText(TextDefs.MORTY_STATS_RELEASE_FAIL_LAST_MORTY);
		}
		else if (App.net.player.data.IsMortyInParty(_mortyData) && App.net.player.data.GetActiveDeckAliveCount() <= 1 && !_mortyData.isFainted)
		{
			_dialogue.TweenText(TextDefs.MORTY_STATS_RELEASE_FAIL_LAST_ALIVE_MORTY);
		}
		else if (this.ReleaseEvent != null)
		{
			this.ReleaseEvent();
		}
	}

	public void DisplayReleaseError(eReleaseMortyError error)
	{
		_dialogue.TweenText(MPTextDefs.MP_MORTY_STATS_RELEASE_FAIL_ERROR);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			TryRelease();
			break;
		case 1:
			if (this.AboutEvent != null)
			{
				this.AboutEvent();
			}
			break;
		case 2:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
