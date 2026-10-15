using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class MPStorageCombineUI : UIBase
{
	public delegate void AbsorbDelegate(MPOpponentMortyData evolveMorty, List<string> absorbMortyNetIds);

	public delegate void UIDelegate();

	public delegate void UIStatsDelegate(MPOpponentMortyData mortyData);

	private const int _kMortyMenuMinimumCellCount = 5;

	private const float _kMortyMenuPaddingTop = 85f;

	private const float _kMortyMenuPaddingBottom = 100f;

	private const float _kMortyMenuCellGap = -4f;

	private MenuDialogue _dialogue;

	private RectTransform _panContainerRectTrans;

	private MenuBase _evolveMenu;

	private ScrollMenu _evolveScrollMenu;

	private EvolveChamber _evolveChamber;

	private MenuBase _absorbMenu;

	private ScrollMenu _absorbScrollMenu;

	private AbsorbChamber _absorbChamber;

	private CanvasGroup _overlayCanvas;

	private MPOpponentMortyData _evolveMorty;

	private List<MPOpponentMortyData> _absorbMortys;

	private string _lastEvolveMortyNetId;

	private bool _inEvolveAnim;

	public event AbsorbDelegate AbsorbEvent;

	public event UIDelegate BackEvent;

	public event UIStatsDelegate StatsEvent;

	protected override void LateAwake()
	{
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_panContainerRectTrans = base.transform.FindChild("PanContainer").GetComponent<RectTransform>();
		_evolveMenu = base.transform.FindChild("MenuBar/EvolveMenu").GetComponent<MenuBase>();
		_evolveScrollMenu = base.transform.FindChild("PanContainer/EvolveScrollMenu").GetComponent<ScrollMenu>();
		_evolveChamber = base.transform.FindChild("PanContainer/EvolveChamber").GetComponent<EvolveChamber>();
		_absorbMenu = base.transform.FindChild("MenuBar/AbsorbMenu").GetComponent<MenuBase>();
		_absorbScrollMenu = base.transform.FindChild("PanContainer/AbsorbScrollMenu").GetComponent<ScrollMenu>();
		_absorbChamber = base.transform.FindChild("PanContainer/AbsorbChamber").GetComponent<AbsorbChamber>();
		_overlayCanvas = base.transform.FindChild("Overlay").GetComponent<CanvasGroup>();
	}

	public void SetUp()
	{
		_evolveScrollMenu.SetUp(1, false, 85f, 100f, -4f);
		_absorbScrollMenu.SetUp(9999, true, 85f, 100f, -4f);
		_absorbMenu.SetPositionOut();
		UpdateMenuForSelectedEvolveMorty();
		int num = CreateEvolveMortyCells();
		_evolveMenu.SetBtnEnabled(0, false);
		_evolveMenu.SetBtnEnabled(2, false);
		_absorbMenu.SetBtnEnabled(2, false);
		if (num > 0)
		{
			_dialogue.SetText(MPTextDefs.MP_STORAGE_COMBINE_CHOOSE_EVOLVE_DIAL);
		}
		else
		{
			_dialogue.SetText(MPTextDefs.MP_STORAGE_COMBINE_NO_MORTIES);
		}
		_overlayCanvas.gameObject.SetActive(false);
	}

	public override void SetLabels()
	{
		_evolveMenu.SetBtnTitle(0, TextDefs.PARTY_BTN_STATS);
		_evolveMenu.SetBtnTitle(2, MPTextDefs.MP_STORAGE_COMBINE_EVOLVE_BTN);
		_absorbMenu.SetBtnTitle(0, MPTextDefs.MP_STORAGE_COMBINE_BACK_BTN);
		_absorbMenu.SetBtnTitle(2, MPTextDefs.MP_STORAGE_COMBINE_ABSORB_BTN);
	}

	protected override void AddEvents()
	{
		_evolveMenu.BtnPressedEvent += EvolveMenuBtnPressedHandler;
		_evolveScrollMenu.SelectedEvent += EvolveScrollCellSelectedHandler;
		_absorbMenu.BtnPressedEvent += AbsorbMenuBtnPressedHandler;
		_absorbScrollMenu.SelectedEvent += AbsorbScrollCellSelectedHandler;
		_absorbScrollMenu.UnselectedEvent += AbsorbScrollCellUnselectedHandler;
	}

	protected override void RemoveEvents()
	{
		_evolveMenu.BtnPressedEvent -= EvolveMenuBtnPressedHandler;
		_evolveScrollMenu.SelectedEvent -= EvolveScrollCellSelectedHandler;
		_absorbMenu.BtnPressedEvent -= AbsorbMenuBtnPressedHandler;
		_absorbScrollMenu.SelectedEvent -= AbsorbScrollCellSelectedHandler;
		_absorbScrollMenu.UnselectedEvent -= AbsorbScrollCellUnselectedHandler;
	}

	private int CreateEvolveMortyCells()
	{
		_evolveScrollMenu.RemoveAllCells();
		List<MPOpponentMortyData> mortys = new List<MPOpponentMortyData>(App.net.player.data.morties);
		mortys.RemoveAll((MPOpponentMortyData data) => MPMortyDefs.EvolutionRequirement(data.id) <= 0);
		mortys.RemoveAll((MPOpponentMortyData data) => mortys.Count((MPOpponentMortyData countData) => countData.id.Equals(data.id)) < 2);
		if (_evolveMorty != null && !mortys.Any((MPOpponentMortyData data) => data.netId.Equals(_evolveMorty.netId)))
		{
			mortys.Add(_evolveMorty);
		}
		mortys = (from e in mortys
			orderby e.level descending
			group e by e.id).OrderByDescending(Enumerable.Count).SelectMany((IGrouping<string, MPOpponentMortyData> e) => e).ToList();
		foreach (MPOpponentMortyData item in mortys)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPMortyCell"));
			MPMortyCell component = rectTransform.GetComponent<MPMortyCell>();
			component.SetUp(item, false, false, true);
			_evolveScrollMenu.AddCell(component);
		}
		while (_evolveScrollMenu.count < 5)
		{
			RectTransform rectTransform2 = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPMortyCell"));
			MPMortyCell component2 = rectTransform2.GetComponent<MPMortyCell>();
			component2.SetUp(null);
			_evolveScrollMenu.AddCell(component2);
		}
		return mortys.Count;
	}

	private void CreateAbsorbMortyCells()
	{
		_absorbScrollMenu.RemoveAllCells();
		List<MPOpponentMortyData> source = App.net.player.data.morties.FindAll((MPOpponentMortyData data) => data.id.Equals(_evolveMorty.id) && data != _evolveMorty);
		source = source.OrderByDescending((MPOpponentMortyData data) => data.level).ToList();
		foreach (MPOpponentMortyData item in source)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPMortyCell"));
			MPMortyCell component = rectTransform.GetComponent<MPMortyCell>();
			component.SetUp(item, false, false, true);
			_absorbScrollMenu.AddCell(component);
		}
		while (_absorbScrollMenu.count < 5)
		{
			RectTransform rectTransform2 = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPMortyCell"));
			MPMortyCell component2 = rectTransform2.GetComponent<MPMortyCell>();
			component2.SetUp(null);
			_absorbScrollMenu.AddCell(component2);
		}
	}

	public void TryAbsorb()
	{
		if (_evolveMorty.evolutionPoints + _absorbMortys.Count < MPMortyDefs.EvolutionRequirement(_evolveMorty.id))
		{
			List<MPOpponentMortyData> first = new List<MPOpponentMortyData>(App.net.player.data.GetAllPartyMortyDatas());
			first = first.Except(_absorbMortys).ToList();
			if (first.Count == 0)
			{
				_dialogue.TweenText(MPTextDefs.MP_STORAGE_COMBINE_ABSORB_ERROR_EMPTY_DECK_DIAL);
				return;
			}
			if (first.FirstOrDefault((MPOpponentMortyData e) => !e.isFainted) == null)
			{
				_dialogue.TweenText(MPTextDefs.MP_STORAGE_COMBINE_ABSORB_ERROR_FAINTED_DECK_DIAL);
				return;
			}
		}
		if (this.AbsorbEvent != null)
		{
			this.AbsorbEvent(_evolveMorty, _absorbMortys.Select((MPOpponentMortyData data) => data.netId).ToList());
		}
	}

	public void DisplayWaitingToAbsorb()
	{
		StartCoroutine(_absorbMenu.TweenOut());
		_dialogue.TweenText(MPTextDefs.MP_STORAGE_COMBINE_WAITING_TO_ABSORB_DIAL);
	}

	public IEnumerator DisplayAbsorbResultGainedPoints(MPOpponentMortyData modifiedMortyData, int evolutionPointsGained, List<string> removedMortyNetIds)
	{
		_inEvolveAnim = true;
		_evolveMorty = modifiedMortyData;
		CreateEvolveMortyCells();
		yield return new WaitForEndOfFrame();
		_evolveScrollMenu.TryCenterAndSelectCellWithID(modifiedMortyData.netId);
		(_evolveScrollMenu.GetSelectedCell() as MPMortyCell).SetEvolutionPoints(modifiedMortyData.evolutionPoints - evolutionPointsGained);
		_dialogue.TweenText("...");
		App.sound.PlaySFX("Audio/SFX/CombineBlend");
		StartCoroutine(_absorbChamber.PlayAbsorbAnimCo());
		yield return _panContainerRectTrans.DOAnchorPos(new Vector2(-724f, _panContainerRectTrans.anchoredPosition.y), 3f).SetEase(Ease.InOutQuad).WaitForCompletion();
		App.sound.PlaySFX("Audio/SFX/CombineSpray");
		yield return StartCoroutine(_evolveChamber.PlaySprayAnimCo());
		yield return _panContainerRectTrans.DOAnchorPos(new Vector2(0f, _panContainerRectTrans.anchoredPosition.y), 1.3f).SetEase(Ease.InOutQuad).WaitForCompletion();
		_absorbChamber.StopAbsorbAnim();
		string text = ((evolutionPointsGained <= 1) ? MPTextDefs.MP_STORAGE_COMBINE_ABSORB_RESULT_POINTS_DIAL : MPTextDefs.MP_STORAGE_COMBINE_ABSORB_RESULT_POINTS_PLUR_DIAL);
		_dialogue.TweenText(string.Format(text, MPMortyDefs.Name(_evolveMorty.id), evolutionPointsGained));
		yield return new WaitForSeconds(1f);
		App.sound.PlaySFX("Audio/SFX/GainXP");
		yield return StartCoroutine((_evolveScrollMenu.GetSelectedCell() as MPMortyCell).TweenToEvolutionPoints(modifiedMortyData.evolutionPoints));
		yield return new WaitForSeconds(1f);
		UpdateMenuForSelectedEvolveMorty();
		StartCoroutine(_evolveMenu.TweenIn());
		_lastEvolveMortyNetId = null;
		_inEvolveAnim = false;
	}

	public IEnumerator DisplayAbsorbResultEvolved(MPOpponentMortyData newMortyData, string preEvoleMortyId, int evolutionPointsGained)
	{
		_inEvolveAnim = true;
		_evolveMorty = newMortyData;
		_dialogue.TweenText("...");
		App.sound.PlaySFX("Audio/SFX/CombineBlend");
		StartCoroutine(_absorbChamber.PlayAbsorbAnimCo());
		yield return _panContainerRectTrans.DOAnchorPos(new Vector2(-724f, _panContainerRectTrans.anchoredPosition.y), 3f).SetEase(Ease.InOutQuad).WaitForCompletion();
		App.sound.PlaySFX("Audio/SFX/CombineSpray");
		yield return StartCoroutine(_evolveChamber.PlaySprayAnimCo());
		yield return _panContainerRectTrans.DOAnchorPos(new Vector2(0f, _panContainerRectTrans.anchoredPosition.y), 1.3f).SetEase(Ease.InOutQuad).WaitForCompletion();
		_absorbChamber.StopAbsorbAnim();
		string text = ((evolutionPointsGained <= 1) ? MPTextDefs.MP_STORAGE_COMBINE_ABSORB_RESULT_POINTS_DIAL : MPTextDefs.MP_STORAGE_COMBINE_ABSORB_RESULT_POINTS_PLUR_DIAL);
		_dialogue.TweenText(string.Format(text, MPMortyDefs.Name(_evolveMorty.id), evolutionPointsGained));
		yield return new WaitForSeconds(1f);
		App.sound.PlaySFX("Audio/SFX/GainXP");
		MPMortyCell oldCell = _evolveScrollMenu.GetSelectedCell() as MPMortyCell;
		yield return StartCoroutine(oldCell.TweenToEvolutionPoints(MPMortyDefs.EvolutionRequirement(oldCell.data.id)));
		yield return new WaitForSeconds(1f);
		CreateEvolveMortyCells();
		_evolveScrollMenu.TryCenterAndSelectCellWithID(_evolveMorty.netId);
		_evolveChamber.SetMorty(_evolveMorty.id);
		_overlayCanvas.gameObject.SetActive(true);
		_overlayCanvas.alpha = 1f;
		yield return new WaitForEndOfFrame();
		App.sound.PlaySFX("Audio/SFX/LevelUP");
		_overlayCanvas.DOFade(0f, 1f).SetEase(Ease.InQuad).OnComplete(delegate
		{
			_overlayCanvas.gameObject.SetActive(false);
		});
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_STORAGE_COMBINE_ABSORB_RESULT_EVOLVED_DIAL, MPMortyDefs.Name(preEvoleMortyId), MPMortyDefs.Name(_evolveMorty.id)), 4f, true));
		UpdateMenuForSelectedEvolveMorty();
		StartCoroutine(_evolveMenu.TweenIn());
		_lastEvolveMortyNetId = null;
		_inEvolveAnim = false;
	}

	public void DisplayAbsorbError()
	{
		_dialogue.TweenText(MPTextDefs.MP_STORAGE_COMBINE_ABSORB_ERROR_DIAL);
		StartCoroutine(_evolveMenu.TweenIn());
	}

	private void SetEvolveMorty(MPOpponentMortyData mortyData)
	{
		_evolveMorty = mortyData;
		StartCoroutine(_evolveChamber.SetMortyCo(mortyData.id));
		UpdateMenuForSelectedEvolveMorty();
	}

	private void UpdateMenuForSelectedEvolveMorty()
	{
		if (_evolveMorty == null)
		{
			_evolveChamber.SetMorty(null);
			_evolveMenu.SetBtnEnabled(0, false);
			_evolveMenu.SetBtnEnabled(2, false);
			_dialogue.TweenText(MPTextDefs.MP_STORAGE_COMBINE_CHOOSE_EVOLVE_DIAL);
			return;
		}
		bool flag = _evolveMorty.evolutionPoints != MPMortyDefs.EvolutionRequirement(_evolveMorty.id) && _evolveScrollMenu.GetCellListCopy().Cast<MPMortyCell>().Count((MPMortyCell cell) => cell.data != null && cell.data.id.Equals(_evolveMorty.id)) > 1;
		_evolveMenu.SetBtnEnabled(0, true);
		_evolveMenu.SetBtnEnabled(2, flag);
		if (flag)
		{
			_dialogue.TweenText(string.Format(MPTextDefs.MP_STORAGE_COMBINE_EVOLVABLE_MORTY_SELECTED_DIAL, MPMortyDefs.Name(_evolveMorty.id)));
		}
		else if (_evolveMorty.evolutionPoints == MPMortyDefs.EvolutionRequirement(_evolveMorty.id))
		{
			_dialogue.TweenText(string.Format(MPTextDefs.MP_STORAGE_COMBINE_FULLY_EVOLVED_MORTY_SELECTED_DIAL, MPMortyDefs.Name(_evolveMorty.id)));
		}
		else
		{
			_dialogue.TweenText(string.Format(MPTextDefs.MP_STORAGE_COMBINE_NON_EVOLVABLE_MORTY_SELECTED_DIAL, MPMortyDefs.Name(_evolveMorty.id)));
		}
	}

	private IEnumerator PanToAbsorb()
	{
		if (_lastEvolveMortyNetId != _evolveMorty.netId)
		{
			CreateAbsorbMortyCells();
		}
		_lastEvolveMortyNetId = _evolveMorty.netId;
		yield return new WaitForEndOfFrame();
		_dialogue.TweenText(string.Empty);
		StartCoroutine(_evolveMenu.TweenOut());
		yield return _panContainerRectTrans.DOAnchorPos(new Vector2(-1408f, _panContainerRectTrans.anchoredPosition.y), 1.3f).SetEase(Ease.InOutQuad).WaitForCompletion();
		StartCoroutine(_absorbMenu.TweenIn());
		_dialogue.TweenText(MPTextDefs.MP_STORAGE_COMBINE_CHOOSE_ABSORB_DIAL);
	}

	private IEnumerator PanToEvolve()
	{
		_dialogue.TweenText(string.Empty);
		StartCoroutine(_absorbMenu.TweenOut());
		yield return _panContainerRectTrans.DOAnchorPos(new Vector2(0f, _panContainerRectTrans.anchoredPosition.y), 1.3f).SetEase(Ease.InOutQuad).WaitForCompletion();
		StartCoroutine(_evolveMenu.TweenIn());
		_dialogue.TweenText(MPTextDefs.MP_STORAGE_COMBINE_CHOOSE_EVOLVE_DIAL);
	}

	public void EvolveScrollCellSelectedHandler(CellBase cell, int index)
	{
		if (!_inEvolveAnim)
		{
			SetEvolveMorty((_evolveScrollMenu.GetSelectedCell() as MPMortyCell).data);
		}
	}

	public void AbsorbScrollCellSelectedHandler(CellBase cell, int index)
	{
		_absorbMortys = (from MPMortyCell mortyCell in _absorbScrollMenu.selectedCells
			select mortyCell.data).ToList();
		_absorbChamber.AddMorty((cell as MPMortyCell).data);
		RefreshAbsorbCellStates();
		_absorbMenu.SetBtnEnabled(2, _absorbMortys.Count > 0);
	}

	public void AbsorbScrollCellUnselectedHandler(CellBase cell, int index)
	{
		_absorbMortys = (from MPMortyCell mortyCell in _absorbScrollMenu.selectedCells
			select mortyCell.data).ToList();
		_absorbChamber.RemoveMorty((cell as MPMortyCell).data);
		RefreshAbsorbCellStates();
		_absorbMenu.SetBtnEnabled(2, _absorbMortys.Count > 0);
	}

	private void RefreshAbsorbCellStates()
	{
		bool flag = _absorbMortys.Count == MPMortyDefs.EvolutionRequirement(_evolveMorty.id) - _evolveMorty.evolutionPoints;
		for (int i = 0; i < _absorbScrollMenu.count; i++)
		{
			CellBase cellAtIndex = _absorbScrollMenu.GetCellAtIndex(i);
			if (cellAtIndex.state != eScrollMenuCellState.SELECTED && cellAtIndex.id != null)
			{
				cellAtIndex.SetEnabled(!flag || _absorbMortys.Contains((cellAtIndex as MPMortyCell).data));
			}
		}
	}

	public void EvolveMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.StatsEvent != null)
			{
				this.StatsEvent(_evolveMorty);
			}
			break;
		case 1:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		case 2:
			StartCoroutine(PanToAbsorb());
			break;
		}
	}

	public void AbsorbMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			StartCoroutine(PanToEvolve());
			break;
		case 1:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		case 2:
			TryAbsorb();
			break;
		}
	}
}
