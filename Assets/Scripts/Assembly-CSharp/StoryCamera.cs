using System.Collections;
using DG.Tweening;
using UnityEngine;

public class StoryCamera : MonoBehaviour
{
	public IEnumerator MoveTo(Vector3 newPosition, float duration)
	{
		Tween moveTween = base.transform.DOMove(newPosition, duration);
		moveTween.SetEase(Ease.Linear);
		yield return moveTween.WaitForCompletion();
	}

	public IEnumerator MoveToX(float x, float duration)
	{
		yield return StartCoroutine(MoveTo(new Vector3(x, base.transform.position.y, base.transform.position.z), duration));
	}

	public IEnumerator MoveToY(float y, float duration)
	{
		yield return StartCoroutine(MoveTo(new Vector3(base.transform.position.x, y, base.transform.position.z), duration));
	}

	public IEnumerator MoveToZ(float z, float duration)
	{
		yield return StartCoroutine(MoveTo(new Vector3(base.transform.position.x, base.transform.position.y, z), duration));
	}
}
