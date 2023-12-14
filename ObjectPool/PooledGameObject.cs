using UnityEngine;
using UnityEngine.Pool;

namespace BasDidon.ObjectPool
{
    public class PooledGameObject : MonoBehaviour
    {
        private IObjectPool<PooledGameObject> pool;
        private PoolManager poolManager;

        public string Id { get; private set; }

        public void Initialize(PoolManager _poolManager, string id)
        {
            pool = _poolManager.Pool[id];
            poolManager = _poolManager;
            Id = id;

            poolManager.OnCleanup += OnCleanup;
            poolManager.OnRelease += OnRelease;
        }

        public void OnRelease()
        {
            // Return object to pool
            pool.Release(this);
        }

        private void OnCleanup()
        {
            // Called when pool manager gets cleanup
            OnRelease();
            Destroy(gameObject);
        }

        private void OnDisable()
        {
            // when this object get disable just return it to pool
            OnRelease();
        }

        private void OnDestroy()
        {
            // gameObject gets destroyed unregister from pool
            if (poolManager)
            {
                poolManager.OnCleanup -= OnCleanup;
                poolManager.OnRelease -= OnRelease;
            }
        }
    }
}
