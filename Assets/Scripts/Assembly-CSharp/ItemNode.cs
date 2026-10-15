using DG.Tweening;
using UnityEngine;

public class ItemNode : AdvancedNode
{
	private SpriteRenderer _spriteRenderer;

	private SpriteRenderer _shadow;

	private void Awake()
	{
		_spriteRenderer = base.transform.FindChild("Sprite").GetComponent<SpriteRenderer>();
		_shadow = base.transform.FindChild("Shadow").GetComponent<SpriteRenderer>();
		_sfxInteract = "Audio/SFX/PickupItem";
		_sfxInteractVolume = 1f;
		StartTween();
	}

	private void StartTween()
	{
		_spriteRenderer.transform.DOLocalMoveY(-0.4f, 0.6f).SetRelative().SetEase(Ease.InQuad)
			.SetLoops(-1, LoopType.Yoyo);
		_spriteRenderer.transform.DOScaleY(0.95f, 0.6f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
		_shadow.transform.DOScale(1.3f, 0.6f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
	}

	protected override void RefreshAssetID()
	{
		if (!string.IsNullOrEmpty(data.id))
		{
			_assetID = ItemDefs.AssetID(data.id);
		}
		if (!string.IsNullOrEmpty(_assetID))
		{
			_spriteRenderer.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", _assetID));
		}
	}
}
