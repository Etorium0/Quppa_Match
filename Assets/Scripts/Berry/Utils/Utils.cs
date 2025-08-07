using System.Collections.Generic;
using UnityEngine;

namespace Berry.Utils
{
	public static class Utils
	{
		public static T GetRandom<T>(this ICollection<T> collection)
		{
			if (collection == null)
			{
				return default(T);
			}
			int num = Random.Range(0, collection.Count);
			foreach (T item in collection)
			{
				if (num == 0)
				{
					return item;
				}
				num--;
			}
			return default(T);
		}
	}
}
