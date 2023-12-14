using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace BasDidon.ObjectPool
{
	[System.Serializable]
	public class PoolItem
	{
		[field: SerializeField] public string PoolId { get; private set; }
		[field: SerializeField] public GameObject Prefab { get; private set; }

		[SerializeField] int poolCount = PoolManager.DEFAULT_POOL_COUNT;
		public int PoolCount => poolCount;
	}

	public class PoolManager : MonoBehaviour
	{
		// Default pool size for each pool item
		public static int DEFAULT_POOL_COUNT = 1;

		// Data Setup
		[SerializeField] private List<PoolItem> poolableObjects = new();

		// Collection checks will throw errors if we try to release an item that is already in the pool.
		[SerializeField] private bool collectionChecks = true;

		// Maximum collection size allocated per pooled object
		[SerializeField] private int maxPoolSize = 10;

		private Dictionary<string, IObjectPool<PooledGameObject>> pool;
		public Dictionary<string, IObjectPool<PooledGameObject>> Pool => pool;

		// Pool is ready to be used
		public bool IsReady { get; private set; }

		public event Action OnCleanup;
		public event Action OnRelease;	// return objects to pool

		public void InstantiatePool()
		{
			if (IsReady)
				return;

			pool = DictionaryPool<string, IObjectPool<PooledGameObject>>.Get();

			// Memory allocation and initialize GameObject
			poolableObjects.ForEach(
				item =>
				{
					// Create stack in dictionary
					CreateObjectsPool(item);
					InitializeObjectsPool(item, item.PoolCount);
				}
			);

			// Disable all pooled objects
			OnRelease?.Invoke();
			IsReady = true;
		}

		public void CleanupPool(bool garbageCollect = true)
		{
			if (!IsReady)
				return;

			// Destroy all game objects and release allocated memory
			OnCleanup?.Invoke();
			DictionaryPool<string, IObjectPool<PooledGameObject>>.Release(pool);

			IsReady = false;
			OnCleanup = null;

			if (garbageCollect)
			{
				GC.Collect();
			}
		}

		public void ReleasePooledObject()
		{
			if (!IsReady)
				return;

			OnRelease?.Invoke();
		}

		private void CreateObjectsPool(PoolItem item)
		{
			if (!item.Prefab)
				throw new Exception("Pool Item prefab is null");

			// Game object doesn't get created at this point, we need to call pool.Get() to do so
			pool.Add(
				item.PoolId,
				new ObjectPool<PooledGameObject>(
					() => CreatePooledItem(item),
					OnGetFromPool,
					OnReleaseToPool,
					OnDestroyPoolObject,
					collectionChecks,
					item.PoolCount,
					maxPoolSize
				)
			);
		}

		private void InitializeObjectsPool(PoolItem item, int count)
		{
			if (!item.Prefab)
				throw new Exception("Pool Item prefab is null");

			var id = item.PoolId;
			if (!pool.TryGetValue(id, out var objectPool))
			{
				CreateObjectsPool(item);
				objectPool = pool[id];
			}

			// Object shouldn't be release at this point else it will just take the same item from the top of the stack
			for (var i = 0; i < count; ++i)
			{
				objectPool.Get();
			}
		}

		private PooledGameObject CreatePooledItem(PoolItem poolItem)
		{
			GameObject go = Instantiate(poolItem.Prefab, Vector3.zero, Quaternion.identity, transform);
			go.name = poolItem.PoolId;

			var pooledGO = go.AddComponent<PooledGameObject>();
			pooledGO.Initialize(this, poolItem.PoolId);
			return pooledGO;
		}

		// PoolObject Actions
		private void OnGetFromPool(PooledGameObject pooledGO)
		{
			pooledGO.gameObject.SetActive(true);
		}

		private void OnReleaseToPool(PooledGameObject releaseGO)
		{
			releaseGO.transform.SetParent(transform);
			releaseGO.gameObject.SetActive(false);
		}

		private void OnDestroyPoolObject(PooledGameObject destroyedGO)
		{
			Destroy(destroyedGO.gameObject);

		}

		// Spawn
		public PooledGameObject Spawn(string id, Transform parent = null)
		{
			if (pool.TryGetValue(id, out var objectPool))
			{
				var pooledGO = objectPool.Get();
				if (parent)
				{
					pooledGO.transform.SetParent(parent);
				}

				return pooledGO;
			}

			return null;
		}
	}
}
