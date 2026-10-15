using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MortydexEntryUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private const int _kMortyInPosX = 500;

	private const int _kMortyOutPosX = 900;

	private const float _kMortyInOutDuration = 0.5f;

	private Image _morty;

	private Image _icon;

	private Text _number;

	private Text _name;

	private Image _gender;

	private MortyElementImage _element;

	private Text _heightTitle;

	private Text _height;

	private Text _weightTitle;

	private Text _weight;

	private Text _characteristicTitle;

	private Text _characteristic;

	private Text _descriptionTitle;

	private Text _description;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	public Sprite[] genderSprites;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_morty = base.transform.FindChild("Morty").GetComponent<Image>();
		_icon = base.transform.FindChild("TopBar/Icon").GetComponent<Image>();
		_number = base.transform.FindChild("TopBar/Number").GetComponent<Text>();
		_name = base.transform.FindChild("TopBar/Name").GetComponent<Text>();
		_gender = base.transform.FindChild("TopBar/Gender").GetComponent<Image>();
		_element = base.transform.FindChild("TopBar/Element").GetComponent<MortyElementImage>();
		_heightTitle = base.transform.FindChild("TopBar/BodyInfo/HeightTitle").GetComponent<Text>();
		_height = base.transform.FindChild("TopBar/BodyInfo/HeightBackground/Label").GetComponent<Text>();
		_weightTitle = base.transform.FindChild("TopBar/BodyInfo/WeightTitle").GetComponent<Text>();
		_weight = base.transform.FindChild("TopBar/BodyInfo/WeightBackground/Label").GetComponent<Text>();
		_characteristicTitle = base.transform.FindChild("BottomBar/Characteristics/Title").GetComponent<Text>();
		_characteristic = base.transform.FindChild("BottomBar/Characteristics/Box/Label").GetComponent<Text>();
		_descriptionTitle = base.transform.FindChild("BottomBar/Description/Title").GetComponent<Text>();
		_description = base.transform.FindChild("BottomBar/Description/Label").GetComponent<Text>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_morty.rectTransform.localPosition = new Vector2(900f, _morty.rectTransform.localPosition.y);
	}

	public void SetUp(string id)
	{
		_mainMenu.SetPositionIn();
		_morty.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MortyDefs.AssetID(id), "front"));
		_morty.SetNativeSize();
		_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MortyDefs.AssetID(id)));
		_icon.SetNativeSize();
		_element.Set(MortyDefs.ElementType(id));
		SetLabels(id);
		if (id == "MortyMorticia")
		{
			_gender.sprite = genderSprites[1];
			_gender.SetNativeSize();
		}
		_morty.rectTransform.DOLocalMoveX(500f, 0.5f);
	}

	private void SetLabels(string id)
	{
		_number.text = MortyDefs.GetMortyNumberText(id);
		_name.text = MortyDefs.Name(id);
		_heightTitle.text = TextDefs.MORTYDEX_ENTRY_HEIGHT_TITLE;
		_height.text = MortyDefs.Height(id);
		_weightTitle.text = TextDefs.MORTYDEX_ENTRY_WEIGHT_TITLE;
		_weight.text = MortyDefs.Weight(id);
		_characteristicTitle.text = TextDefs.MORTYDEX_ENTRY_CHARACTERISTIC_TITLE;
		_characteristic.text = MortyDefs.Characteristic(id);
		_descriptionTitle.text = TextDefs.MORTYDEX_ENTRY_DESCRIPTION_TITLE;
		_description.text = MortyDefs.Description(id);
		_dialogue.SetText(string.Format(TextDefs.MORTYDEX_ENTRY_DIAL_SET_UP, MortyDefs.Name(id)));
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		if (index == 0)
		{
			_morty.rectTransform.DOLocalMoveX(900f, 0.5f);
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
		}
	}
}
