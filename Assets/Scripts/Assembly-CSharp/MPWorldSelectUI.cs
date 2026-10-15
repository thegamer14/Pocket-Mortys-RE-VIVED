using System;
using System.Collections;
using DG.Tweening;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class MPWorldSelectUI : UIBase
{
	public delegate void WorldSelectedDelegate(string worldId);

	private string _currentWorldId;

	private int _currentWorldLevelReq;

	private int _playerLevel;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private Transform _donutTrans;

	private RawImage _tunnelImage;

	private GameObject _windowObj;

	private UIImageAnimation _deepZoomAnim;

	private Transform _spaceBackgroundTrans;

	private Image _locationImage;

	private GameObject _locationPointerObj;

	private Transform _portalIconTrans;

	private RectTransform _titleBoxTrans;

	private Text _titleText;

	private Image _coloredTitleBoxImage;

	private GameObject _infoBox;

	private Text _infoTitleText;

	private Text _infoText;

	private GameObject _dimensionBox;

	private Text _dimensionTitleText;

	private Text _dimensionText;

	private Image _overlayImage;

	private GameObject _previousButtonObj;

	private GameObject _nextButtonObj;

	private Vector3 _portalRotateBy = new Vector3(0f, 0f, -500f);

	private float _titleBoxOutX = -1015f;

	private float _titleBoxInX;

	private Coroutine _animCoroutine;

	public event WorldSelectedDelegate WorldSelectedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		SetCurrentWorldId("1");
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_donutTrans = base.transform.FindChild("Donut");
		_tunnelImage = base.transform.FindChild("Tunnel").GetComponent<RawImage>();
		_windowObj = base.transform.FindChild("Window").gameObject;
		_deepZoomAnim = _windowObj.transform.FindChild("DeepZoom").GetComponent<UIImageAnimation>();
		_spaceBackgroundTrans = _windowObj.transform.FindChild("SpaceBackgroundMask/SpaceBackground");
		_locationImage = _windowObj.transform.FindChild("SpaceBackgroundMask/LocationImage").GetComponent<Image>();
		_locationPointerObj = _windowObj.transform.FindChild("LocationPointer").gameObject;
		_portalIconTrans = base.transform.FindChild("PortalIcon");
		_titleBoxTrans = (RectTransform)base.transform.FindChild("TitleBox");
		_titleText = _titleBoxTrans.FindChild("Text").GetComponent<Text>();
		_coloredTitleBoxImage = _titleBoxTrans.FindChild("ColoredTitleBox").GetComponent<Image>();
		_infoBox = base.transform.FindChild("InfoBox").gameObject;
		_infoTitleText = _infoBox.transform.FindChild("Title").GetComponent<Text>();
		_infoText = _infoBox.transform.FindChild("Text").GetComponent<Text>();
		_dimensionBox = base.transform.FindChild("DimensionBox").gameObject;
		_dimensionTitleText = _dimensionBox.transform.FindChild("Title").GetComponent<Text>();
		_dimensionText = _dimensionBox.transform.FindChild("Text").GetComponent<Text>();
		_overlayImage = base.transform.FindChild("Overlay").GetComponent<Image>();
		_previousButtonObj = base.transform.FindChild("MenuBar/PageMenu/PreviousBtn").gameObject;
		_nextButtonObj = base.transform.FindChild("MenuBar/PageMenu/NextBtn").gameObject;
		_titleBoxInX = _titleBoxTrans.anchoredPosition.x;
		_deepZoomAnim.gameObject.SetActive(false);
		_overlayImage.gameObject.SetActive(false);
	}

	public void SetUp(string selectedWorldId, int playerLevel)
	{
		SetCurrentWorldId(selectedWorldId);
		_playerLevel = playerLevel;
		RefreshText();
		RefreshImages();
		RefreshButtons();
		_dialogue.SetText(MPTextDefs.MP_WORLD_SELECT_DIAL);
	}

	public override void SetLabels()
	{
		base.SetLabels();
		_infoTitleText.text = MPTextDefs.MP_WORLD_SELECT_RESTRICTIONS;
		_dimensionTitleText.text = MPTextDefs.MP_WORLD_SELECT_DIMENSION;
		_mainMenu.SetBtnTitle(0, MPTextDefs.MP_WORLD_SELECT_TELEPORT_BTN);
	}

	private void SetCurrentWorldId(string worldId)
	{
		_currentWorldId = worldId;
		JSONArray jSONArray = MPWorldDefs.Zones(_currentWorldId);
		if (TestDefs.MP_DISABLE_DIMENSION_LEVEL_LOCKS)
		{
			_currentWorldLevelReq = 0;
			return;
		}
		_currentWorldLevelReq = -1;
		for (int i = 0; i < jSONArray.Count; i++)
		{
			int num = Mathf.Max(1, jSONArray[i]["player"].AsArray[0].AsInt);
			if (_currentWorldLevelReq == -1 || num < _currentWorldLevelReq)
			{
				_currentWorldLevelReq = num;
			}
		}
	}

	public void SetStateJoining()
	{
		_dialogue.TweenText(MPTextDefs.MP_WORLD_SELECT_TELEPORTING);
		_previousButtonObj.SetActive(false);
		_nextButtonObj.SetActive(false);
		StartCoroutine(_mainMenu.TweenOut());
	}

	public void SetStateError(string text)
	{
		_dialogue.TweenText(text);
		RefreshButtons();
		StartCoroutine(_mainMenu.TweenIn());
	}

	public override void Destroy()
	{
		base.Destroy();
		if (_animCoroutine != null)
		{
			StopCoroutine(_animCoroutine);
		}
		DOTween.KillAll();
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void RefreshText()
	{
		_titleText.text = MPTextDefs.GetTitleForWorld(_currentWorldId);
		_dimensionText.text = MPTextDefs.GetDimensionNumberForWorld(_currentWorldId);
		_infoText.text = string.Format(MPTextDefs.MP_WORLD_SELECT_MIN_PLAYER_LEVEL, _currentWorldLevelReq);
		if (_playerLevel >= _currentWorldLevelReq)
		{
			_dialogue.TweenText(string.Format(MPTextDefs.MP_WORLD_SELECT_UNLOCKED_DIAL, _titleText.text));
		}
		else
		{
			_dialogue.TweenText(string.Format(MPTextDefs.MP_WORLD_SELECT_LOCKED_DIAL, _currentWorldLevelReq));
		}
	}

	private void RefreshImages()
	{
		_overlayImage.sprite = Resources.Load<Sprite>(string.Format("Multiplayer/UI/Sprites/MPWorldSelect/{0}/Overlay", _currentWorldId));
		_locationImage.sprite = Resources.Load<Sprite>(string.Format("Multiplayer/UI/Sprites/MPWorldSelect/{0}/WindowView", _currentWorldId));
		_coloredTitleBoxImage.sprite = Resources.Load<Sprite>(string.Format("Multiplayer/UI/Sprites/MPWorldSelect/{0}/TitleBox", _currentWorldId));
	}

	private void RefreshButtons()
	{
		int num = Array.IndexOf(MPWorldDefs.allWorldIDs, _currentWorldId);
		_previousButtonObj.SetActive(num > 0);
		_nextButtonObj.SetActive(num < MPWorldDefs.allWorldIDs.Length - 1);
	}

	private IEnumerator AnimateToWorld(string worldId, bool dirNext)
	{
		if (_animCoroutine != null)
		{
			StopCoroutine(_animCoroutine);
		}
		DOTween.KillAll();
		ActivateInput(false);
		if (_playerLevel < _currentWorldLevelReq)
		{
			StartCoroutine(_mainMenu.TweenOut());
		}
		RefreshText();
		RefreshImages();
		RefreshButtons();
		_windowObj.SetActive(false);
		_deepZoomAnim.gameObject.SetActive(false);
		_spaceBackgroundTrans.gameObject.SetActive(false);
		_locationImage.gameObject.SetActive(false);
		_locationImage.transform.localScale = Vector3.one * 0.2f;
		_spaceBackgroundTrans.transform.localScale = Vector3.one * 0.5f;
		_locationPointerObj.SetActive(false);
		_titleBoxTrans.anchoredPosition = new Vector2(_titleBoxOutX, _titleBoxTrans.anchoredPosition.y);
		_infoBox.SetActive(false);
		_dimensionBox.SetActive(false);
		_overlayImage.color = new Color(1f, 1f, 1f, 0f);
		_overlayImage.gameObject.SetActive(false);
		_coloredTitleBoxImage.color = new Color(1f, 1f, 1f, 0f);
		float toX = ((!dirNext) ? (-0.84f) : 1.16f);
		DOTween.To(delegate(float x)
		{
			_tunnelImage.uvRect = new Rect(x, _tunnelImage.uvRect.y, _tunnelImage.uvRect.width, _tunnelImage.uvRect.height);
		}, 0.16f, toX, 0.5f).SetEase(Ease.InOutQuart);
		yield return _donutTrans.DOScale(0.2f, 0.25f).SetEase(Ease.OutQuart).WaitForCompletion();
		yield return _donutTrans.DOScale(1f, 0.25f).SetEase(Ease.InQuart).WaitForCompletion();
		if (_playerLevel >= _currentWorldLevelReq)
		{
			StartCoroutine(_mainMenu.TweenIn());
		}
		_titleBoxTrans.DOLocalMoveX(_titleBoxInX, 0.2f).SetEase(Ease.OutQuart);
		ActivateInput(true);
		_infoBox.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		_dimensionBox.SetActive(true);
		_windowObj.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		_windowObj.SetActive(false);
		yield return new WaitForSeconds(0.05f);
		_windowObj.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		_windowObj.SetActive(false);
		yield return new WaitForSeconds(0.05f);
		_windowObj.SetActive(true);
		_coloredTitleBoxImage.DOFade(1f, 0.3f);
		_deepZoomAnim.gameObject.SetActive(true);
		_deepZoomAnim.SetSprite(0);
		_deepZoomAnim.play = true;
		while (_deepZoomAnim.spriteIndex != _deepZoomAnim.sprites.Length - 1)
		{
			yield return null;
		}
		_deepZoomAnim.gameObject.SetActive(false);
		_spaceBackgroundTrans.gameObject.SetActive(true);
		_locationImage.gameObject.SetActive(true);
		_spaceBackgroundTrans.DOScale(1f, 0.5f).SetEase(Ease.OutQuint);
		_locationImage.transform.DOScale(1f, 0.75f).SetEase(Ease.OutQuint);
		yield return new WaitForSeconds(0.3f);
		_locationPointerObj.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		_locationPointerObj.SetActive(false);
		yield return new WaitForSeconds(0.05f);
		_locationPointerObj.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		_locationPointerObj.SetActive(false);
		yield return new WaitForSeconds(0.05f);
		_locationPointerObj.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		_overlayImage.gameObject.SetActive(true);
		_overlayImage.DOFade(1f, 0.5f);
		_animCoroutine = null;
	}

	public override void ManualUpdate(float deltaTime)
	{
		base.ManualUpdate(deltaTime);
		if (_portalIconTrans != null)
		{
			_portalIconTrans.Rotate(_portalRotateBy * deltaTime);
		}
	}

	public void NextButtonHandler()
	{
		int num = Array.IndexOf(MPWorldDefs.allWorldIDs, _currentWorldId);
		if (num + 1 < MPWorldDefs.allWorldIDs.Length)
		{
			SetCurrentWorldId(MPWorldDefs.allWorldIDs[num + 1]);
			_animCoroutine = StartCoroutine(AnimateToWorld(_currentWorldId, true));
		}
	}

	public void PreviousButtonHandler()
	{
		int num = Array.IndexOf(MPWorldDefs.allWorldIDs, _currentWorldId);
		if (num > 0)
		{
			SetCurrentWorldId(MPWorldDefs.allWorldIDs[num - 1]);
			_animCoroutine = StartCoroutine(AnimateToWorld(_currentWorldId, false));
		}
	}

	private void MainMenuBtnPressedHandler(int index)
	{
		if (index == 0 && this.WorldSelectedEvent != null)
		{
			this.WorldSelectedEvent(_currentWorldId);
		}
	}
}
