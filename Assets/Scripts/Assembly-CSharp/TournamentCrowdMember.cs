using DG.Tweening;
using UnityEngine;

public class TournamentCrowdMember : MonoBehaviour
{
	private Sequence _sequenceTween;

	private void Awake()
	{
	}

	private void Start()
	{
		_sequenceTween = DOTween.Sequence();
		_sequenceTween.AppendInterval(Random.value);
		_sequenceTween.Append(base.transform.DOPunchPosition(Vector3.up * 0.3f, 0.4f, 0, 0f).SetEase(Ease.InOutCirc));
		_sequenceTween.AppendInterval(Random.value);
		_sequenceTween.SetLoops(-1);
		_sequenceTween.Play();
	}

	private void OnDestroy()
	{
		_sequenceTween.Kill();
		_sequenceTween = null;
	}

	private void Update()
	{
	}
}
