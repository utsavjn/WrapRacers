using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Copyright (C) Bilal Itani - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bilal Itani <bilalitani1@gmail.com>, March 2016
 */
namespace Xengine
{
	public class ObjectPooler : MonoBehaviour
	{
		private static int defaultPoolSize = 20;

		public static bool willGrow = true;

		public static Dictionary<string, List<GameObject>> objectPools = new Dictionary<string, List<GameObject>>();

		public static GameObject GetPooledObject(string prefabPath)
		{
			if (!objectPools.ContainsKey(prefabPath))
			{
				CreateObjectPool(prefabPath, defaultPoolSize);
				return GetPooledObject(prefabPath);
			}

			var pool = objectPools[prefabPath];

			// Pick the next inactive object.
			for (int i = 0; i < pool.Count; i++)
			{
				if (!pool[i].activeInHierarchy)
				{
					return pool[i];
				}
			}

			// If all active and allowed to expand, expand pool and use new object.
			if (willGrow)
			{
				GameObject prefab = Resources.Load<GameObject>(prefabPath);

				GameObject shotInstance = Instantiate(prefab) as GameObject;
				pool.Add(shotInstance);
				return shotInstance;
			}

			// If willgrow is false, and we request more than the pool offers
			return null;
		}

		public static GameObject GetPooledObject(string prefabPath, int poolSize)
		{
			if (!objectPools.ContainsKey(prefabPath))
			{
				CreateObjectPool(prefabPath, poolSize);
				return GetPooledObject(prefabPath);
			}

			var pool = objectPools[prefabPath];

			// Pick the next inactive object.
			for (int i = 0; i < pool.Count; i++)
			{
				if (pool[i] && !pool[i].activeInHierarchy)
				{
					return pool[i];
				}
			}

			// If all active and allowed to expand, expand pool and use new object.
			if (willGrow)
			{
				GameObject prefab = Resources.Load<GameObject>(prefabPath);

				GameObject shotInstance = Instantiate(prefab) as GameObject;
				pool.Add(shotInstance);
				return shotInstance;
			}

			// If willgrow is false, and we request more than the pool offers
			return null;
		}

		public static GameObject GetPooledObject(string prefabPath, Vector3 position, Quaternion rotation)
		{
			GameObject go = GetPooledObject(prefabPath);

			Transform t = go.transform;
			t.position = position;
			t.rotation = rotation;

			return go;
		}

		public static GameObject GetPooledObject(string prefabPath, Vector3 position, Quaternion rotation, Transform parent)
		{
			GameObject go = GetPooledObject(prefabPath, position, rotation);

			go.transform.SetParent(parent);

			return go;
		}

		//3 positions to give parent
		// give parent to default and creating an object pool, or if it adds a new instance, set its parent
		public static GameObject GetPooledObject(string prefabPath, Transform parent)
		{
			if (!objectPools.ContainsKey(prefabPath))
			{
				CreateObjectPool(prefabPath, defaultPoolSize);
				return GetPooledObject(prefabPath);
			}

			var pool = objectPools[prefabPath];

			// Pick the next inactive object.
			for (int i = 0; i < pool.Count; i++)
			{
				if (!pool[i].activeInHierarchy)
				{
					pool[i].transform.SetParent(parent);
					return pool[i];
				}
			}

			// If all active and allowed to expand, expand pool and use new object.
			if (willGrow)
			{
				GameObject prefab = Resources.Load<GameObject>(prefabPath);

				GameObject instance = Instantiate(prefab) as GameObject;
				instance.transform.SetParent(parent);
				pool.Add(instance);
				return instance;
			}

			// If willgrow is false, and we request more than the pool offers
			return null;
		}


		public static GameObject GetPooledObject(string prefabPath, int poolSize, Transform parent)
		{
			if (!objectPools.ContainsKey(prefabPath))
			{
				CreateObjectPool(prefabPath, poolSize, parent);
				return GetPooledObject(prefabPath);
			}

			var pool = objectPools[prefabPath];

			// Pick the next inactive object.
			for (int i = 0; i < pool.Count; i++)
			{
				if (pool[i] && !pool[i].activeInHierarchy)
				{
					pool[i].transform.SetParent(parent);
					return pool[i];
				}
			}

			// If all active and allowed to expand, expand pool and use new object.
			if (willGrow)
			{
				GameObject prefab = Resources.Load<GameObject>(prefabPath);

				GameObject instance = Instantiate(prefab) as GameObject;
				instance.transform.SetParent(parent);
				pool.Add(instance);
				return instance;
			}

			// If willgrow is false, and we request more than the pool offers
			return null;
		}

		// no parent
		public static List<GameObject> CreateObjectPool(string prefabPath, int count)
		{
			if (count <= 0) count = 1;

			GameObject prefab = Resources.Load<GameObject>(prefabPath);
			List<GameObject> objects = new List<GameObject>();

			for (int i = 0; i < count; i++)
			{
				GameObject instance = Instantiate<GameObject>(prefab);

				objects.Add(instance);

				//instance.hideFlags = HideFlags.HideInHierarchy;

				instance.SetActive(false);
			}

			objectPools.Add(prefabPath, objects);

			return objects;
		}

		// Create an object pool and set each object in the pool to be a child of a given parent
		public static List<GameObject> CreateObjectPool(string prefabPath, int count, Transform parent)
		{
			if (count <= 0) count = 1;

			GameObject prefab = Resources.Load<GameObject>(prefabPath);
			List<GameObject> objects = new List<GameObject>();

			for (int i = 0; i < count; i++)
			{
				GameObject instance = Instantiate<GameObject>(prefab);

				objects.Add(instance);

				//instance.hideFlags = HideFlags.HideInHierarchy;

				instance.transform.SetParent(parent);

				instance.SetActive(false);
			}

			objectPools.Add(prefabPath, objects);

			return objects;
		}

	}
}