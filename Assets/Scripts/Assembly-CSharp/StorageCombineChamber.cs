using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StorageCombineChamber : MonoBehaviour
{
	private const float _kMortyPositionLeftY = 285f;

	private const float _kMortyPositionCenterY = 390f;

	private const float _kMortyPositionRightY = 488f;

	private const float _kMortyPositionOffY = 1000f;

	private Image _morty1;

	private Image _morty2;

	private Image _mortyCombined;

	private Dictionary<MortyCombineCell, Image> _selectedMorties;

	private void Awake()
	{
		_morty1 = base.transform.FindChild("Morty1").GetComponent<Image>();
		_morty2 = base.transform.FindChild("Morty2").GetComponent<Image>();
		_mortyCombined = base.transform.FindChild("MortyCombined").GetComponent<Image>();
		_selectedMorties = new Dictionary<MortyCombineCell, Image>();
		SetMortyPosition(_morty1, 1000f);
		SetMortyPosition(_morty2, 1000f);
	}

	public void SelectMorty(MortyCombineCell cell)
	{
		Image image = (_selectedMorties.ContainsValue(_morty1) ? _morty2 : _morty1);
		SetMorty(image, cell.data.id);
		TweenMortyPosition(image, (_selectedMorties.Count != 0) ? 488f : 390f, 0.4f);
		if (_selectedMorties.Count > 0)
		{
			TweenMortyPosition(_selectedMorties[_selectedMorties.Keys.ToArray()[0]], 285f, 0.4f);
		}
		TweenMortyPosition(_mortyCombined, 1000f, 0.4f);
		_selectedMorties.Add(cell, image);
	}

	public void UnselectMorty(MortyCombineCell cell)
	{
		TweenMortyPosition(_selectedMorties[cell], 1000f, 0.4f);
		_selectedMorties.Remove(cell);
		if (_selectedMorties.Count > 0)
		{
			TweenMortyPosition(_selectedMorties[_selectedMorties.Keys.ToArray()[0]], 390f, 0.4f);
		}
	}

	private void SetMorty(Image image, string id)
	{
		image.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MortyDefs.AssetID(id), "front"));
		image.SetNativeSize();
	}

	private void SetMortyPosition(Image image, float posX)
	{
		image.transform.localPosition = new Vector3(posX, image.transform.localPosition.y);
	}

	private void TweenMortyPosition(Image image, float posX, float duration)
	{
		image.transform.DOLocalMoveX(posX, duration);
	}

	public void SetCombined(MortyData combinedMorty)
	{
		SetMorty(_mortyCombined, combinedMorty.id);
		SetMortyPosition(_mortyCombined, 390f);
	}
}
