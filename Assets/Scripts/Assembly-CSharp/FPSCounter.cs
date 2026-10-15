using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private GUIStyle style;

	private string displayString;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		timeleft = updateInterval;
		style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		displayString = string.Empty;
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
	}

	private void OnGUI()
	{
		int width = Screen.width;
		int height = Screen.height;
		Rect position = new Rect(0f, 0f, width, height * 2 / 100);
		if ((double)timeleft <= 0.0)
		{
			float num = accum / (float)frames;
			displayString = string.Format("{0:F2} FPS", num);
			style.fontSize = height * 5 / 100;
			if (num < 30f)
			{
				style.normal.textColor = Color.yellow;
			}
			else if (num < 10f)
			{
				style.normal.textColor = Color.red;
			}
			else
			{
				style.normal.textColor = Color.green;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
		GUI.Label(position, displayString, style);
	}
}
