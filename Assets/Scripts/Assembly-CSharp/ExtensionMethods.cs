using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public static class ExtensionMethods
{
	private static System.Random __rnd;

	private static System.Random _rnd
	{
		get
		{
			if (__rnd == null)
			{
				__rnd = new System.Random();
			}
			return __rnd;
		}
	}

	public static string SerializeObjectToXmlString<T>(this T toSerialize)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
		using (StringWriter stringWriter = new StringWriter())
		{
			xmlSerializer.Serialize(stringWriter, toSerialize);
			return stringWriter.ToString();
		}
	}

	public static float DegAngleTo360(this float angle)
	{
		if (angle > 360f || angle < 360f)
		{
			angle %= 360f;
		}
		if (angle < 0f)
		{
			angle = 360f + angle;
		}
		return angle;
	}

	public static string IntToLetters(int value)
	{
		string text = string.Empty;
		while (value >= 0)
		{
			text = (char)(65 + value % 26) + text;
			value /= 26;
			value--;
		}
		return text;
	}

	public static string Ordinal(int number)
	{
		string empty = string.Empty;
		int num = number % 10;
		int num2 = (int)Math.Floor((decimal)number / 10m) % 10;
		if (num2 == 1)
		{
			empty = "th";
		}
		else
		{
			switch (num)
			{
			case 1:
				empty = "st";
				break;
			case 2:
				empty = "nd";
				break;
			case 3:
				empty = "rd";
				break;
			default:
				empty = "th";
				break;
			}
		}
		return string.Format("{0}{1}", number, empty);
	}

	public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 vector = point - pivot;
		vector = Quaternion.Euler(angles) * vector;
		point = vector + pivot;
		return point;
	}

	public static float DistanceOnLine(this Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		return Vector3.Magnitude(point.PositionOnLine(lineStart, lineEnd) - point);
	}

	public static Vector3 PositionOnLine(this Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 rhs = point - lineStart;
		Vector3 vector = lineEnd - lineStart;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector;
		if (magnitude > 1E-06f)
		{
			vector2 /= magnitude;
		}
		float num = Mathf.Clamp(Vector3.Dot(vector2, rhs), 0f, magnitude);
		return lineStart + vector2 * num;
	}

	public static T[] GetComponentsInImmediateChildren<T>(this Transform transform) where T : Component
	{
		T[] componentsInChildren = transform.GetComponentsInChildren<T>();
		List<T> list = new List<T>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].transform.parent == transform)
			{
				list.Add(componentsInChildren[i]);
			}
		}
		return list.ToArray();
	}

	public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
	{
		T value = list[indexA];
		list[indexA] = list[indexB];
		list[indexB] = value;
		return list;
	}

	public static IList<T> Shuffle<T>(this IList<T> arr, System.Random rndInstance = null)
	{
		if (rndInstance == null)
		{
			rndInstance = _rnd;
		}
		int num = arr.Count;
		while (num > 1)
		{
			num--;
			int index = rndInstance.Next(num + 1);
			T value = arr[index];
			arr[index] = arr[num];
			arr[num] = value;
		}
		return arr;
	}

	public static void Populate<T>(this IList<T> arr, T value)
	{
		for (int i = 0; i < arr.Count; i++)
		{
			arr[i] = value;
		}
	}

	public static void PopulateRandom(this IList<int> arr, int min, int max)
	{
		System.Random random = new System.Random();
		for (int i = 0; i < arr.Count; i++)
		{
			arr[i] = random.Next(min, max);
		}
	}

	public static T PickRandom<T>(this IList<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[_rnd.Next(list.Count)];
	}

	public static T PickRandom<T>(this IList<T> list, T excluding)
	{
		List<T> list2 = new List<T>(list);
		list2.Remove(excluding);
		return list2.PickRandom();
	}

	public static string ToStringElements<T>(this IList<T> list)
	{
		if (list == null)
		{
			return null;
		}
		string text = "[";
		for (int i = 0; i < list.Count; i++)
		{
			text += list[i].ToString();
			if (i < list.Count - 1)
			{
				text += ", ";
			}
		}
		return text + "]";
	}

	public static T[] SubArray<T>(this T[] data, int index, int length)
	{
		T[] array = new T[length];
		Array.Copy(data, index, array, 0, length);
		return array;
	}

	public static byte[] ObjectToByteArray(object obj)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using (MemoryStream memoryStream = new MemoryStream())
		{
			binaryFormatter.Serialize(memoryStream, obj);
			return memoryStream.ToArray();
		}
	}

	public static int AddNext<T>(this SortedList<int, T> sortedList, T item)
	{
		int num = 0;
		int count = sortedList.Count;
		int num2 = 0;
		while (count != 0)
		{
			int num3 = sortedList.Keys[num2++];
			if (num != num3)
			{
				break;
			}
			num = num3 + 1;
			if (count == 1 || num2 == count || num != sortedList.Keys[num2])
			{
				break;
			}
		}
		sortedList.Add(num, item);
		return num;
	}

	public static void LimitSize<T>(this Queue<T> queue, int size)
	{
		while (queue.Count > size)
		{
			queue.Dequeue();
		}
	}

	public static T ToEnum<T>(this string strEnumValue, T defaultValue)
	{
		if (!Enum.IsDefined(typeof(T), strEnumValue))
		{
			return defaultValue;
		}
		return (T)Enum.Parse(typeof(T), strEnumValue);
	}

	public static IEnumerator PlayAndWait(this Animator animator, string animationName, int layer = 0, float normalizedTime = float.NegativeInfinity)
	{
		animator.Play(animationName, layer, normalizedTime);
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
	}

	public static string ToBitString(this BitArray bits)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < bits.Count; i++)
		{
			char value = ((!bits[i]) ? '0' : '1');
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}
}
