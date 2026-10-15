using UnityEngine;
using UnityEngine.UI;

public class MortyStatsUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private Image _icon;

	private Text _number;

	private Text _name;

	private Image _gender;

	private MortyElementImage _element;

	private Image _morty;

	private Text _dimensionTitle;

	private Text _dimension;

	private MortyHPBar _hpBar;

	private MortyLevelBar _levelBar;

	private MortyStatusBar _statusBar;

	private MortyStatsBox _statsBox;

	private BattleAttackMenu _attackMenu;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private MortyData _mortyData;

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
		_dimensionTitle = base.transform.FindChild("Box/DimensionBar/Title").GetComponent<Text>();
		_dimension = base.transform.FindChild("Box/DimensionBar/Box/Label").GetComponent<Text>();
		_hpBar = base.transform.FindChild("Box/HPBar").GetComponent<MortyHPBar>();
		_levelBar = base.transform.FindChild("Box/LevelBar").GetComponent<MortyLevelBar>();
		_statusBar = base.transform.FindChild("TitleBar/StatusBar").GetComponent<MortyStatusBar>();
		_statsBox = base.transform.FindChild("Box/Stats").GetComponent<MortyStatsBox>();
		_attackMenu = base.transform.FindChild("Box/AttackMenu").GetComponent<BattleAttackMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp(MortyData mortyData, bool isBattle)
	{
		_mortyData = mortyData;
		SetNumberBar();
		SetTitleBar();
		SetMorty();
		SetDimensionBar();
		SetHPBar();
		SetLevelBar();
		SetStatusBar();
		SetStatsBox();
		RefreshAttackMenu();
		_mainMenu.SetPositionIn();
		if (isBattle)
		{
			_mainMenu.SetBtnEnabled(0, false);
		}
		if (mortyData.id == "MortyMorticia")
		{
			_gender.sprite = genderSprites[1];
			_gender.SetNativeSize();
		}
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_STATS_BTN_RELEASE);
		_mainMenu.SetBtnTitle(1, TextDefs.MORTY_STATS_BTN_ABOUT);
		_dialogue.SetText(TextDefs.MORTY_STATS_DIAL);
		if (SharedStatInfo.mortyStatsInfo.hideReleaseBtn)
		{
			_mainMenu.SetBtnVisible(0, false);
			_mainMenu.SetBtnPosition(1, new Vector3(-128f, 0f, 0f), true);
			_mainMenu.SetBtnPosition(2, new Vector3(-128f, 0f, 0f), true);
		}
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
		_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MortyDefs.AssetID(_mortyData.id)));
		_icon.SetNativeSize();
		_number.text = MortyDefs.GetMortyNumberText(_mortyData.id);
	}

	public void SetTitleBar()
	{
		_name.text = MortyDefs.Name(_mortyData.id);
		_element.Set(MortyDefs.ElementType(_mortyData.id), true);
	}

	public void SetMorty()
	{
		_morty.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MortyDefs.AssetID(_mortyData.id), "frontFlipped"));
		if (_morty.sprite == null)
		{
			_morty.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MortyDefs.AssetID(_mortyData.id), "front"));
		}
		_morty.SetNativeSize();
	}

	public void SetDimensionBar()
	{
		_dimensionTitle.text = TextDefs.MORTY_STATS_DIMENSION_TITLE;
		_dimension.text = string.Format(TextDefs.MORTY_STATS_DIMENSION, _mortyData.dimensionCaught);
	}

	public void SetHPBar()
	{
		_hpBar.SetHPMax(_mortyData.hpStat, _mortyData.hp);
	}

	public void SetLevelBar()
	{
		_levelBar.SetXPTarget(MortyDefs.GetXPTarget(_mortyData.level), MortyDefs.GetXPTarget(_mortyData.level + 1), _mortyData.xp);
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
			if (i < _mortyData.attackIDs.Count && _mortyData.attackIDs[i] != null)
			{
				_attackMenu.SetBtnEnabled(i, true);
				string id = _mortyData.attackIDs[i];
				AttackData data = AttackDefs.GetData(id);
				_attackMenu.SetUpBtn(i, new AttackDisplayInfo(data, _mortyData.attackPPs[i]));
			}
			else
			{
				_attackMenu.SetBtnEnabled(i, false);
			}
		}
	}

	public void ReleaseFailed(string message)
	{
		_dialogue.TweenText(message);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.ReleaseEvent != null)
			{
				this.ReleaseEvent();
			}
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
