using System.Collections;
using UnityEngine;

public class Flicker : MonoBehaviour
{
	public MonoBehaviour componentToFlick;

	public float onMinTime = 0.8f;

	public float onMaxTime = 0.5f;

	public float offMaxTime = 0.08f;

	public float offMinTime = 0.08f;

	private void Start()
	{
		StartCoroutine("FlickerCo");
	}

	private void OnDestroy()
	{
		StopCoroutine("FlickerCo");
	}

	private IEnumerator FlickerCo()
	{
		while (base.enabled)
		{
			componentToFlick.enabled = !componentToFlick.enabled;
			if (componentToFlick.enabled)
			{
				yield return new WaitForSeconds(Random.Range(onMinTime, onMaxTime));
			}
			else
			{
				yield return new WaitForSeconds(Random.Range(offMaxTime, offMinTime));
			}
		}
	}
}
