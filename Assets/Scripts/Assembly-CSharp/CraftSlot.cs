using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour
{
	public delegate void CraftSlotBtnDelegate(CraftSlot slot);

	private Image _floor;

	private Image _beam;

	private Button _addBtn;

	private CanvasGroup _addBtnGroup;

	private Text _addBtnLabel;

	private Button _itemBtn;

	private RectTransform _itemBtnRect;

	public string itemId { get; private set; }

	public event CraftSlotBtnDelegate AddEvent;

	public event CraftSlotBtnDelegate RemoveEvent;

	private void Awake()
	{
		_floor = base.transform.FindChild("Floor").GetComponent<Image>();
		_beam = base.transform.FindChild("Beam").GetComponent<Image>();
		_addBtn = base.transform.FindChild("AddBtn").GetComponent<Button>();
		_addBtnGroup = _addBtn.GetComponent<CanvasGroup>();
		_addBtnLabel = base.transform.FindChild("AddBtn/Image/Label").GetComponent<Text>();
		_itemBtnRect = (RectTransform)base.transform.FindChild("ItemBtn");
		_itemBtn = _itemBtnRect.GetComponent<Button>();
	}

	public void SetUp()
	{
		SetSlotEmpty();
		TweenAddIdle();
	}

	public void SetLabels()
	{
		_addBtnLabel.text = TextDefs.CRAFT_BTN_ADD;
	}

	public void SetSlotEmpty()
	{
		_floor.gameObject.SetActive(false);
		_beam.gameObject.SetActive(false);
		ShowAddButton(true);
		ShowItemButton(false);
		itemId = null;
	}

	public void SetSlotFilled(string id, string assetId)
	{
		_floor.gameObject.SetActive(true);
		_beam.gameObject.SetActive(false);
		ShowAddButton(false);
		ShowItemButton(true);
		_itemBtn.image.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", assetId, "front"));
		_itemBtn.image.SetNativeSize();
		itemId = id;
	}

	private void ShowAddButton(bool show)
	{
		_addBtn.gameObject.SetActive(show);
	}

	public bool TweenAddOut()
	{
		if (string.IsNullOrEmpty(itemId))
		{
			DOTween.To(() => _addBtnGroup.alpha, delegate(float x)
			{
				_addBtnGroup.alpha = x;
			}, 0f, 0.5f);
			return true;
		}
		return false;
	}

	private void TweenAddIdle()
	{
		_addBtn.transform.DOKill();
		_addBtn.transform.DOLocalMoveY(30f, 2f).SetRelative().SetLoops(-1, LoopType.Yoyo);
	}

	private void ShowItemButton(bool show)
	{
		_itemBtn.gameObject.SetActive(show);
	}

	private void TweenItemIdle(bool tween)
	{
		KillItemTween();
		if (tween)
		{
			_itemBtnRect.DOLocalMoveY(6f, 2.2f).SetRelative().SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine);
			_itemBtnRect.DORotate(new Vector3(0f, 0f, 4f), 3f).SetRelative().SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine);
		}
	}

	public IEnumerator TweenSlotFilled(string id, string assetID)
	{
		SetSlotFilled(id, assetID);
		_itemBtnRect.anchoredPosition = new Vector2(0f, -80f);
		_itemBtnRect.eulerAngles = new Vector3(0f, 0f, -2f);
		_itemBtn.image.color = new Color(1f, 1f, 1f, 0f);
		_floor.color = new Color(1f, 1f, 1f, 0f);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, _itemBtn.image.DOColor(Color.white, 0.5f));
		sequence.Insert(0f, _itemBtnRect.DOAnchorPos(new Vector2(0f, -142f), 0.5f));
		sequence.Insert(0f, _floor.DOColor(Color.white, 0.5f));
		yield return sequence.WaitForCompletion();
		TweenItemIdle(true);
	}

	public IEnumerator TweenSlotEmpty()
	{
		TweenItemIdle(false);
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, _itemBtn.image.DOColor(new Color(1f, 1f, 1f, 0f), 0.4f));
		sequence.Insert(0f, _itemBtnRect.DOAnchorPos(new Vector2(0f, -80f), 0.4f));
		sequence.Insert(0f, _floor.DOColor(new Color(1f, 1f, 1f, 0f), 0.4f));
		yield return sequence.WaitForCompletion();
		SetSlotEmpty();
		_addBtnGroup.alpha = 0f;
		yield return DOTween.To(() => _addBtnGroup.alpha, delegate(float x)
		{
			_addBtnGroup.alpha = x;
		}, 1f, 0.15f).WaitForCompletion();
	}

	public IEnumerator TweenSlotUsed()
	{
		if (!string.IsNullOrEmpty(itemId))
		{
			_beam.gameObject.SetActive(true);
			_beam.rectTransform.localScale = Vector3.zero;
			yield return _beam.rectTransform.DOScaleY(1f, 0.5f).WaitForCompletion();
			yield return _beam.rectTransform.DOScaleX(1f, 0.3f).WaitForCompletion();
			_itemBtn.gameObject.SetActive(false);
			_beam.DOColor(new Color(1f, 1f, 1f, 0f), 0.8f);
			_floor.DOColor(new Color(1f, 1f, 1f, 0f), 0.8f);
		}
	}

	private void KillItemTween()
	{
		_itemBtnRect.DOKill();
		_itemBtn.image.DOKill();
	}

	public void AddBtnPressedHandler()
	{
		if (this.AddEvent != null)
		{
			this.AddEvent(this);
		}
	}

	public void ItemBtnPressedHandler()
	{
		if (this.RemoveEvent != null)
		{
			this.RemoveEvent(this);
		}
	}
}
