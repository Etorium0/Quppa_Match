using System.Collections.Generic;
using UnityEngine;

public static class APIExtention
{
	public static string CorrectString(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return null;
		}
		return input.Replace("\r", string.Empty);
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = Random.Range(0, num);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
