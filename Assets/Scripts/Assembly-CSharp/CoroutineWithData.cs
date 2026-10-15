using System.Collections;
using UnityEngine;

public class CoroutineWithData
{
	public object result;

	private IEnumerator target;

	public Coroutine coroutine { get; private set; }

	public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
	{
		this.target = target;
		coroutine = owner.StartCoroutine(Run());
	}

	private IEnumerator Run()
	{
		while (target.MoveNext())
		{
			result = target.Current;
			yield return result;
		}
	}
}
