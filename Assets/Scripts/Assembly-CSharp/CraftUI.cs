using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CraftUI : UIBase
{
	public delegate void CraftUIBtnDelegate();

	public delegate void CraftUIItemDelegate(int index);

	public delegate void CraftUIResultDelegate(string id);

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private RectTransform _table;

	private CraftSlot[] _slots;

	private CraftSlotResult _slotResult;

	private IEnumerator _refreshResultsCo;

	private string _recipeID;

	private AudioSource _previewSFX;

	public event CraftUIBtnDelegate TweetEvent;

	public event CraftUIBtnDelegate RecipesEvent;

	public event CraftUIBtnDelegate BackEvent;

	public event CraftUIItemDelegate AddEvent;

	public event CraftUIItemDelegate RemoveEvent;

	public event CraftUIResultDelegate CraftEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_table = (RectTransform)base.transform.FindChild("Table");
		_slots = new CraftSlot[3];
		_slots[0] = _table.FindChild("Slot1").GetComponent<CraftSlot>();
		_slots[1] = _table.FindChild("Slot2").GetComponent<CraftSlot>();
		_slots[2] = _table.FindChild("Slot3").GetComponent<CraftSlot>();
		_slotResult = _table.FindChild("SlotResult").GetComponent<CraftSlotResult>();
	}

	public void SetUp()
	{
		for (int i = 0; i < _slots.Length; i++)
		{
			_slots[i].SetUp();
		}
		_slotResult.SetUp();
		_mainMenu.SetBtnEnabled(0, false);
		_mainMenu.SetBtnEnabled(1, false);
		_dialogue.SetText(TextDefs.CRAFT_DIAL_NO_ITEMS);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.CRAFT_BTN_CRAFT);
		_mainMenu.SetBtnTitle(1, TextDefs.CRAFT_BTN_TWEET);
		_mainMenu.SetBtnTitle(2, TextDefs.CRAFT_BTN_RECIPES);
		_slots[0].SetLabels();
		_slots[1].SetLabels();
		_slots[2].SetLabels();
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		for (int i = 0; i < _slots.Length; i++)
		{
			_slots[i].AddEvent += SlotAddHandler;
			_slots[i].RemoveEvent += SlotRemoveHandler;
		}
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		for (int i = 0; i < _slots.Length; i++)
		{
			_slots[i].AddEvent -= SlotAddHandler;
			_slots[i].RemoveEvent -= SlotRemoveHandler;
		}
	}

	public override void Destroy()
	{
		if (_previewSFX != null)
		{
			App.sound.StopSFX(_previewSFX, 0.3f);
		}
		base.Destroy();
	}

	public void TryCraft()
	{
		if (!string.IsNullOrEmpty(_recipeID))
		{
			if (_refreshResultsCo != null)
			{
				StopCoroutine(_refreshResultsCo);
				_refreshResultsCo = null;
			}
			StartCoroutine(_mainMenu.TweenOut());
			StartCoroutine(Craft());
		}
	}

	public IEnumerator Craft()
	{
		_dialogue.TweenText(TextDefs.CRAFT_DIAL_CRAFTING);
		ActivateInput(false);
		StartCoroutine(_mainMenu.TweenOut());
		bool waitForAdds = false;
		for (int i = 0; i < _slots.Length; i++)
		{
			waitForAdds |= _slots[i].TweenAddOut();
		}
		if (waitForAdds)
		{
			yield return new WaitForSeconds(0.5f);
		}
		for (int j = 0; j < _slots.Length; j++)
		{
			if (_slots[j].itemId != null)
			{
				App.sound.PlaySFX("Audio/SFX/CraftUseItem", 1f, false, 1f, 0f, 0.2f);
				yield return StartCoroutine(_slots[j].TweenSlotUsed());
			}
		}
		yield return StartCoroutine(_slotResult.TweenInFloor());
		App.sound.StopSFX(_previewSFX, 0.3f);
		yield return StartCoroutine(_slotResult.TweenOutResult());
		StartCoroutine(_slotResult.TweenInGlow());
		App.sound.PlaySFX("Audio/SFX/CraftItem");
		yield return _table.transform.DOLocalMoveY(-140f, 4f).WaitForCompletion();
		if (this.CraftEvent != null)
		{
			this.CraftEvent(_recipeID);
		}
	}

	public void SetSlotFilled(int index, string id)
	{
		_slots[index].SetSlotFilled(id, ItemDefs.AssetID(id));
	}

	public void SetSlotEmpty(int index)
	{
		_slots[index].SetSlotEmpty();
	}

	public IEnumerator TweenSlotFilled(int index, string id)
	{
		ActivateInput(false);
		yield return StartCoroutine(_slots[index].TweenSlotFilled(id, ItemDefs.AssetID(id)));
		ActivateInput(true);
	}

	public IEnumerator TweenSlotEmpty(int index)
	{
		ActivateInput(false);
		yield return StartCoroutine(_slots[index].TweenSlotEmpty());
		ActivateInput(true);
	}

	public IEnumerator RefreshResultsSlot(string id, int slotsFilled, float imageDelay)
	{
		_recipeID = id;
		bool validID = !string.IsNullOrEmpty(_recipeID);
		bool validBagSpace = validID && ItemDefs.ItemsIndividualCount(SharedStatInfo.craftInfo.items, RecipeDefs.ResultID(id)) < 10;
		_mainMenu.SetBtnEnabled(0, validID && validBagSpace);
		_mainMenu.SetBtnEnabled(1, validID);
		_refreshResultsCo = RefreshResultsDialogue(id, slotsFilled, validBagSpace);
		StartCoroutine(_refreshResultsCo);
		yield return new WaitForSeconds(imageDelay);
		if (_slotResult.isFilled)
		{
			App.sound.StopSFX(_previewSFX, 0.3f);
			yield return StartCoroutine(_slotResult.TweenOutResult());
		}
		if (!string.IsNullOrEmpty(_recipeID))
		{
			_previewSFX = App.sound.PlaySFX("Audio/SFX/CraftPreview", 1f, true, 1f, 0.2f);
			yield return StartCoroutine(_slotResult.TweenInResult(ItemDefs.AssetID(RecipeDefs.ResultID(_recipeID))));
		}
	}

	private IEnumerator RefreshResultsDialogue(string id, int slotsFilled, bool validBagSpace)
	{
		if (!string.IsNullOrEmpty(id))
		{
			if (validBagSpace)
			{
				if (!PlayerDefs.HasRecipedBeenLearned(id))
				{
					App.sound.PlaySFX("Audio/SFX/PickupItem");
					yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.CRAFT_DIAL_NEW_RECIPE, 2f));
				}
				_dialogue.TweenText(string.Format(TextDefs.CRAFT_DIAL_CRAFT, ItemDefs.Name(RecipeDefs.ResultID(id))));
			}
			else
			{
				_dialogue.TweenText(string.Format(TextDefs.ITEM_OBTAIN_NO_SPACE, ItemDefs.Name(RecipeDefs.ResultID(id))));
			}
		}
		else if (slotsFilled >= 2)
		{
			_dialogue.TweenText(TextDefs.CRAFT_DIAL_NO_MATCH);
		}
		else if (slotsFilled >= 1)
		{
			_dialogue.TweenText(TextDefs.CRAFT_DIAL_ONE_ITEM);
		}
		else
		{
			_dialogue.TweenText(TextDefs.CRAFT_DIAL_NO_ITEMS);
		}
	}

	public void TweenToCache()
	{
		if (_previewSFX != null)
		{
			App.sound.StopSFX(_previewSFX, 0.3f);
		}
		StartCoroutine(_mainMenu.TweenOut());
	}

	public void TweenFromCache()
	{
		if (_slotResult.isFilled)
		{
			_previewSFX = App.sound.PlaySFX("Audio/SFX/CraftPreview", 1f, true, 1f, 0.2f);
		}
		StartCoroutine(_mainMenu.TweenIn());
	}

	public void SlotAddHandler(CraftSlot slot)
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.AddEvent != null)
		{
			this.AddEvent(Array.IndexOf(_slots, slot));
		}
	}

	public void SlotRemoveHandler(CraftSlot slot)
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.RemoveEvent != null)
		{
			this.RemoveEvent(Array.IndexOf(_slots, slot));
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			TryCraft();
			break;
		case 1:
			if (this.TweetEvent != null)
			{
				this.TweetEvent();
			}
			break;
		case 2:
			if (this.RecipesEvent != null)
			{
				this.RecipesEvent();
			}
			break;
		case 3:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
