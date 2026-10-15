using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMenu : MonoBehaviour
{
	public delegate void ScrollMenuSelectedDelegate(CellBase cell, int index);

	private ScrollRect _scrollRect;

	public Slider slider;

	private CanvasGroup _containerCanvasGroup;

	private float _paddingTop;

	private float _paddingBottom;

	private float _cellGap;

	private List<CellBase> _selectedCells;

	private List<CellBase> _cells;

	private int _selectedCellsMax;

	private bool _allowUnselecting;

	private bool _disableOtherCellsWhenMaxSelected;

	public RectTransform rectTransform { get; private set; }

	public int count
	{
		get
		{
			return _cells.Count;
		}
	}

	public int selectedCount
	{
		get
		{
			return _selectedCells.Count;
		}
	}

	public List<CellBase> selectedCells
	{
		get
		{
			return _selectedCells;
		}
	}

	public bool Interactable
	{
		get
		{
			return _containerCanvasGroup.interactable;
		}
	}

	public event ScrollMenuSelectedDelegate SelectedEvent;

	public event ScrollMenuSelectedDelegate UnselectedEvent;

	private void Awake()
	{
		_cells = new List<CellBase>();
		_selectedCells = new List<CellBase>();
		_selectedCellsMax = 1;
		_allowUnselecting = false;
		_scrollRect = GetComponent<ScrollRect>();
		rectTransform = GetComponent<RectTransform>();
		_containerCanvasGroup = base.transform.FindChild("Container").GetComponent<CanvasGroup>();
	}

	public void SetUp(int selectedCellsMax, bool allowUnselecting, float paddingTop = 0f, float paddingBottom = 0f, float cellGap = 0f, bool disableOtherCellsWhenMaxSelected = false)
	{
		_allowUnselecting = allowUnselecting;
		_paddingTop = paddingTop;
		_paddingBottom = paddingBottom;
		_cellGap = cellGap;
		_disableOtherCellsWhenMaxSelected = disableOtherCellsWhenMaxSelected;
		SetUpSelectMax(selectedCellsMax);
	}

	public void SetUpSelectMax(int selectedCellsMax)
	{
		int num = Mathf.Min(_selectedCellsMax, _selectedCells.Count - 1);
		for (int num2 = num; num2 >= selectedCellsMax; num2--)
		{
			SetCellUnselected(_selectedCells[num]);
		}
		_selectedCellsMax = selectedCellsMax;
	}

	private void OnDestroy()
	{
		for (int i = 0; i < _cells.Count; i++)
		{
			_cells[i].SelectedEvent -= CellSelectedHandler;
		}
	}

	private void Update()
	{
		if (slider != null)
		{
			slider.value = _scrollRect.normalizedPosition.y;
		}
		ActivateOnScreenCells();
	}

	public void ActivateOnScreenCells()
	{
		Canvas canvas = GetComponentInParent<Canvas>();
		Camera camera = ((canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null);
		float num = 0.1f * (float)Screen.height;
		float num2 = (float)Screen.height;
		for (int i = 0; i < _cells.Count; i++)
		{
			Vector3[] array = new Vector3[4];
			_cells[i].rectTransform.GetWorldCorners(array);
			float num3 = float.MaxValue;
			float num4 = float.MinValue;
			for (int j = 0; j < 4; j++)
			{
				float y = RectTransformUtility.WorldToScreenPoint(camera, array[j]).y;
				if (y < num3)
				{
					num3 = y;
				}
				if (y > num4)
				{
					num4 = y;
				}
			}
			if (num4 < num || num3 > num2)
			{
				_cells[i].gameObject.SetActive(false);
			}
			else
			{
				_cells[i].gameObject.SetActive(true);
			}
		}
	}

	public void SetInteractable(bool value)
	{
		_containerCanvasGroup.interactable = value;
	}

	public void AddCell(CellBase cell)
	{
		cell.SelectedEvent += CellSelectedHandler;
		_cells.Add(cell);
		cell.rectTransform.SetParent(_scrollRect.content, false);
		cell.rectTransform.anchoredPosition = GetAnchoredPositionForCell(cell);
		RefreshScrollContainerSize();
	}

	public void RemoveCell(CellBase cell, bool destroy = true)
	{
		cell.SelectedEvent -= CellSelectedHandler;
		_cells.Remove(cell);
		SetCellUnselected(cell);
		RefreshCellPositions();
		RefreshScrollContainerSize();
		if (destroy)
		{
			UnityEngine.Object.Destroy(cell.gameObject);
		}
	}

	public int RemoveAllCellsOfType(Type type, bool destroy = true)
	{
		int num = 0;
		for (int num2 = _cells.Count - 1; num2 >= 0; num2--)
		{
			if (_cells[num2].GetType() == type)
			{
				RemoveCell(_cells[num2], destroy);
				num++;
			}
		}
		return num;
	}

	public int RemoveAllCells(bool destroy = true)
	{
		int num = 0;
		for (int num2 = _cells.Count - 1; num2 >= 0; num2--)
		{
			RemoveCell(_cells[num2], destroy);
			num++;
		}
		return num;
	}

	public void RefreshCellPositions()
	{
		for (int i = 0; i < _cells.Count; i++)
		{
			_cells[i].rectTransform.anchoredPosition = GetAnchoredPositionForCell(_cells[i]);
		}
	}

	private Vector2 GetAnchoredPositionForCell(CellBase cell)
	{
		int num = _cells.IndexOf(cell) - 1;
		RectTransform rectTransform = ((num < 0) ? null : _cells[num].rectTransform);
		float num2 = ((!(rectTransform != null)) ? (0f - _paddingTop) : rectTransform.anchoredPosition.y);
		float num3 = ((!(rectTransform != null)) ? 0f : (rectTransform.sizeDelta.y + _cellGap));
		return new Vector2(0f, num2 - num3);
	}

	public void TrySetCellSelected(CellBase cell)
	{
		if (cell == null || _selectedCellsMax <= 0 || cell.state == eScrollMenuCellState.DISABLED)
		{
			return;
		}
		if (cell.retainSelection)
		{
			bool flag = _selectedCells.Contains(cell);
			int index = (flag ? _selectedCells.IndexOf(cell) : 0);
			if ((_selectedCells.Count >= _selectedCellsMax && !flag) || (flag && _allowUnselecting))
			{
				SetCellUnselected(_selectedCells[index]);
			}
			if (!flag)
			{
				SetCellSelected(cell);
			}
		}
		else if (this.SelectedEvent != null)
		{
			this.SelectedEvent(cell, _cells.IndexOf(cell));
		}
	}

	private void SetCellSelected(CellBase cell)
	{
		if (!_selectedCells.Contains(cell))
		{
			cell.SetSelect(true);
			_selectedCells.Add(cell);
			if (_disableOtherCellsWhenMaxSelected && _selectedCells.Count >= _selectedCellsMax)
			{
				SetUnselectedCellsEnabled(false);
			}
			if (this.SelectedEvent != null)
			{
				this.SelectedEvent(cell, _cells.IndexOf(cell));
			}
		}
	}

	public void SetCellUnselected(CellBase cell)
	{
		if (_selectedCells.Contains(cell))
		{
			cell.SetSelect(false);
			_selectedCells.Remove(cell);
			if (_disableOtherCellsWhenMaxSelected && _selectedCells.Count < _selectedCellsMax)
			{
				SetUnselectedCellsEnabled(true);
			}
			if (this.UnselectedEvent != null)
			{
				this.UnselectedEvent(cell, _cells.IndexOf(cell));
			}
		}
	}

	public void SetAllCellsUnselected()
	{
		while (_selectedCells.Count > 0)
		{
			SetCellUnselected(_selectedCells[0]);
		}
	}

	private void SetUnselectedCellsEnabled(bool enabled)
	{
		foreach (CellBase cell in _cells)
		{
			if (!_selectedCells.Contains(cell))
			{
				cell.SetEnabled(enabled);
			}
		}
	}

	public int GetCellCount()
	{
		return (_cells != null) ? _cells.Count : 0;
	}

	public CellBase GetSelectedCell(int index = 0)
	{
		if (index > -1 && index < _selectedCells.Count)
		{
			return _selectedCells[index];
		}
		return null;
	}

	public int GetSelectedCellIndex(int index = 0)
	{
		if (index > -1 && index < _selectedCells.Count)
		{
			return _cells.IndexOf(_selectedCells[index]);
		}
		return -1;
	}

	public CellBase GetCellAtIndex(int index)
	{
		if (index > -1 && index < _cells.Count)
		{
			return _cells[index];
		}
		return null;
	}

	public CellBase GetLastCell()
	{
		if (_cells.Count == 0)
		{
			return null;
		}
		return _cells[_cells.Count - 1];
	}

	public CellBase GetCellForID(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		for (int i = 0; i < _cells.Count; i++)
		{
			if (id.Equals(_cells[i].id))
			{
				return _cells[i];
			}
		}
		return null;
	}

	public int GetIndexFromCell(CellBase cell)
	{
		return _cells.IndexOf(cell);
	}

	public void AddHeaderCell(string text)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "HeaderCell"));
		HeaderCell component = rectTransform.GetComponent<HeaderCell>();
		component.SetUp(text);
		component.SetUsed(false);
		AddCell(component);
	}

	public List<CellBase> GetCellListCopy()
	{
		return new List<CellBase>(_cells);
	}

	public void SwapCellsIndex(CellBase cellA, CellBase cellB)
	{
		_cells.Swap(_cells.IndexOf(cellA), _cells.IndexOf(cellB));
	}

	public void RefreshScrollContainerSize()
	{
		if (_cells.Count != 0)
		{
			RectTransform rectTransform = _cells[_cells.Count - 1].rectTransform;
			float y = Mathf.Abs(rectTransform.localPosition.y - rectTransform.sizeDelta.y - _paddingBottom);
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, y);
		}
	}

	public void SnapToTop()
	{
		_scrollRect.verticalNormalizedPosition = 1f;
	}

	public void TryCenterAndSelectCellWithID(string id)
	{
		CellBase cellForID = GetCellForID(id);
		if (cellForID != null)
		{
			TrySetCellSelected(cellForID);
			CenterToCell(cellForID);
		}
	}

	public void CenterToCell(CellBase cell)
	{
		_scrollRect.verticalNormalizedPosition = GetCellPositionInScrollRect(cell);
	}

	public void TweenToCell(CellBase cell)
	{
		DOTween.To(() => _scrollRect.verticalNormalizedPosition, delegate(float x)
		{
			_scrollRect.verticalNormalizedPosition = x;
		}, GetCellPositionInScrollRect(cell), 1f);
	}

	public float GetCellPositionInScrollRect(CellBase cell)
	{
		float num = rectTransform.rect.size.y * 0.5f;
		float y = _scrollRect.content.rect.size.y;
		float y2 = _scrollRect.content.InverseTransformPoint(cell.transform.position).y;
		y2 += cell.rectTransform.rect.size.y * 0.25f;
		float num2 = 1f - Mathf.Clamp01(y2 / (0f - (y - num)));
		float num3 = num / y;
		num2 += num2 * num3;
		return Mathf.Clamp01(num2);
	}

	public void CellSelectedHandler(CellBase cell)
	{
		App.sound.PlaySFX("Audio/SFX/CellSelect");
		TrySetCellSelected(cell);
	}
}
