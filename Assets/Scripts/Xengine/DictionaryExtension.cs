using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xengine
{
	namespace CollectionExtensions
	{
		public static class DictionaryExtension
		{
			public static void StorePair<K, V>(this IDictionary<K, V> dictionary, K key, V value, DictionaryOverwrite overwriteType = DictionaryOverwrite.Leave)
			{
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, value);
				}
				else {
					switch (overwriteType)
					{
						case DictionaryOverwrite.Replace:
							dictionary[key] = value;
							break;
						default:
							break;
					}
				}
			}

			public static V GetValue<K, V>(this IDictionary<K, V> dictionary, K key)
			{
				if (!dictionary.ContainsKey(key))
				{
					return default(V);
				}

				return dictionary[key];
			}
		}
	}
}

public enum DictionaryOverwrite { Replace, Leave }