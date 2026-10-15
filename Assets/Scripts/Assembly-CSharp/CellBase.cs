using UnityEngine;
using UnityEngine.UI;

public class CellBase : MonoBehaviour
{
	public delegate void CellBaseDelegate(CellBase cell);

	protected Sprite _defaultSpr;

	public Sprite unusedSpr;

	public Sprite disabledSpr;

	public Sprite selectedSpr;

	private Button _btn;

	protected Image _backgroundImage;

	public eScrollMenuCellState state { get; private set; }

	public RectTransform rectTransform { get; private set; }

	public string id { get; protected set; }

	public bool retainSelection { get; protected set; }

	public event CellBaseDelegate SelectedEvent;

	protected virtual void Awake()
	{
		rectTransform = (RectTransform)base.transform;
		_btn = GetComponent<Button>();
		_backgroundImage = base.transform.FindChild("Background").GetComponent<Image>();
		_defaultSpr = _backgroundImage.sprite;
		retainSelection = true;
	}

	public virtual void SetUp(string id)
	{
		this.id = id;
	}

	public virtual void SetUsed(bool value)
	{
		state = ((!value) ? eScrollMenuCellState.UNUSED : eScrollMenuCellState.UNSELECTED);
		SetBackground((!value) ? unusedSpr : _defaultSpr);
		_btn.interactable = value;
	}

	public virtual void SetEnabled(bool value)
	{
		state = ((!value) ? eScrollMenuCellState.DISABLED : eScrollMenuCellState.UNSELECTED);
		SetBackground((!value) ? disabledSpr : _defaultSpr);
	}

	public virtual void SetSelect(bool value)
	{
		if (string.IsNullOrEmpty(id))
		{
			SetUsed(false);
			return;
		}
		state = ((!value) ? eScrollMenuCellState.UNSELECTED : eScrollMenuCellState.SELECTED);
		SetBackground((!value) ? _defaultSpr : selectedSpr);
	}

	protected void SetBackground(Sprite spr)
	{
		_backgroundImage.sprite = spr;
	}

	public void SelectedHandler()
	{
		if (this.SelectedEvent != null)
		{
			this.SelectedEvent(this);
		}
	}
}
