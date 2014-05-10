using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
	public static class Utility
	{
		public static void ShuffleList<T>(this List<T> li)
		{
			/// Shuffles the list passed in
			int start = 0;

			while (start < li.Count - 1)
			{
				int index = Random.Range(start + 1, li.Count);
				T temp = li[start];
				li[start] = li[index];
				li[index] = temp;
				start++;
			}
		}
	}


}

