using UnityEngine;
using UnityEngine.UI;

public class MPRecipeCell : CellBase
{
	private Image _icon;

	private Text _name;

	private Text _description;

	private GameObject _slot1;

	private Image _slot1Icon;

	private GameObject _slot2;

	private Image _slot2Icon;

	private GameObject _slot3;

	private Image _slot3Icon;

	private Text _countBarText;

	private int _recipeNumber;

	protected override void Awake()
	{
		base.Awake();
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_name = base.transform.FindChild("Name").GetComponent<Text>();
		_description = base.transform.FindChild("Description").GetComponent<Text>();
		_slot1 = base.transform.FindChild("Slot1").gameObject;
		_slot1Icon = base.transform.FindChild("Slot1/Icon").GetComponent<Image>();
		_slot2 = base.transform.FindChild("Slot2").gameObject;
		_slot2Icon = base.transform.FindChild("Slot2/Icon").GetComponent<Image>();
		_slot3 = base.transform.FindChild("Slot3").gameObject;
		_slot3Icon = base.transform.FindChild("Slot3/Icon").GetComponent<Image>();
		_countBarText = base.transform.FindChild("CountBar/Label").GetComponent<Text>();
	}

	public void SetUp(string id, int number, int craftableQuantity, bool hasLearned)
	{
		base.SetUp(id);
		_recipeNumber = number;
		if (_recipeNumber > 9)
		{
			_name.rectTransform.localPosition += new Vector3(12f, 0f);
		}
		if (craftableQuantity == 0)
		{
			_countBarText.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			_countBarText.text = craftableQuantity.ToString();
		}
		if (hasLearned)
		{
			SetUnlocked();
		}
		else
		{
			SetLocked();
		}
	}

	private void SetLocked()
	{
		_name.text = string.Format("#{0}", _recipeNumber);
		_description.text = TextDefs.CRAFT_RECIPE_NAME_LOCKED;
		if (string.IsNullOrEmpty(MPRecipeDefs.Slot1(base.id)))
		{
			_slot1.gameObject.SetActive(false);
		}
		if (string.IsNullOrEmpty(MPRecipeDefs.Slot2(base.id)))
		{
			_slot2.gameObject.SetActive(false);
		}
		if (string.IsNullOrEmpty(MPRecipeDefs.Slot3(base.id)))
		{
			_slot3.gameObject.SetActive(false);
		}
		_countBarText.transform.parent.gameObject.SetActive(false);
	}

	private void SetUnlocked()
	{
		string text = MPRecipeDefs.ResultID(base.id);
		_name.text = string.Format("#{0}: {1}", _recipeNumber, MPItemDefs.Name(text));
		_description.text = MPItemDefs.Description(text);
		_icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPItemDefs.AssetID(text)));
		_icon.SetNativeSize();
		if (!string.IsNullOrEmpty(MPRecipeDefs.Slot1(base.id)))
		{
			_slot1Icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPItemDefs.AssetID(MPRecipeDefs.Slot1(base.id))));
			_slot1Icon.SetNativeSize();
		}
		else
		{
			_slot1.gameObject.SetActive(false);
		}
		if (!string.IsNullOrEmpty(MPRecipeDefs.Slot2(base.id)))
		{
			_slot2Icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPItemDefs.AssetID(MPRecipeDefs.Slot2(base.id))));
			_slot2Icon.SetNativeSize();
		}
		else
		{
			_slot2.gameObject.SetActive(false);
		}
		if (!string.IsNullOrEmpty(MPRecipeDefs.Slot3(base.id)))
		{
			_slot3Icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPItemDefs.AssetID(MPRecipeDefs.Slot3(base.id))));
			_slot3Icon.SetNativeSize();
		}
		else
		{
			_slot3.gameObject.SetActive(false);
		}
	}
}
