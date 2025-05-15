using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : SingletonBase<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public ObjectType type;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<GameObject, Queue<GameObject>> prefabPools = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, List<GameObject>> activeObjects = new Dictionary<GameObject, List<GameObject>>();

    private new void Awake()
    {
        base.Awake();

        foreach (var pool in pools)
        {
            InitializePool(pool.prefab, pool.size);
        }

        EventManager.Instance.OnObjectReturnToPool += ReturnToPool;
    }

    public void InitializePool(GameObject prefab, int size)
    {
        if (!prefabPools.ContainsKey(prefab))
        {
            prefabPools[prefab] = new Queue<GameObject>();
            activeObjects[prefab] = new List<GameObject>();
        }

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            prefabPools[prefab].Enqueue(obj);
        }
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position)
    {
        if (!prefabPools.ContainsKey(prefab))
        {
            Debug.LogWarning("No pool for this prefab. Initializing...");
            InitializePool(prefab, 1);
        }

        if (prefabPools[prefab].Count == 0)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            prefabPools[prefab].Enqueue(obj);
        }

        GameObject pooledObj = prefabPools[prefab].Dequeue();
        pooledObj.SetActive(true);
        pooledObj.transform.position = position;

        activeObjects[prefab].Add(pooledObj);
        return pooledObj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);

        foreach (var kvp in activeObjects)
        {
            if (kvp.Value.Contains(obj))
            {
                kvp.Value.Remove(obj);
                prefabPools[kvp.Key].Enqueue(obj);
                return;
            }
        }

        Debug.LogWarning("Returned object not tracked in active list.");
    }

    public void ResetAll()
    {
        foreach (var kvp in activeObjects)
        {
            List<GameObject> activeList = kvp.Value;
            GameObject prefabKey = kvp.Key;

            for (int i = activeList.Count - 1; i >= 0; i--)
            {
                GameObject obj = activeList[i];
                obj.SetActive(false);
                prefabPools[prefabKey].Enqueue(obj);
            }

            activeList.Clear();
        }
    }
}



//using System.Collections.Generic;
//using UnityEngine;

//public class ObjectPooler : SingletonBase<ObjectPooler>
//{
//    [System.Serializable]
//    public class Pool
//    {
//        public ObjectType type;
//        public GameObject prefab;
//        public int size;
//    }

//    public List<Pool> pools;
//    private Dictionary<GameObject, Queue<GameObject>> prefabPools = new Dictionary<GameObject, Queue<GameObject>>();

//    private new void Awake()
//    {
//        base.Awake();

//        foreach (var pool in pools)
//        {
//            InitializePool(pool.prefab, pool.size);
//        }

//        EventManager.Instance.OnObjectReturnToPool += ReturnToPool;
//    }

//    public void InitializePool(GameObject prefab, int size)
//    {
//        if (!prefabPools.ContainsKey(prefab))
//        {
//            prefabPools[prefab] = new Queue<GameObject>();
//        }

//        for (int i = 0; i < size; i++)
//        {
//            GameObject obj = Instantiate(prefab);
//            obj.SetActive(false);
//            prefabPools[prefab].Enqueue(obj);
//        }
//    }

//    public GameObject GetFromPool(GameObject prefab, Vector3 position)
//    {
//        if (!prefabPools.ContainsKey(prefab))
//        {
//            Debug.LogWarning("No pool for this prefab. Initializing...");
//            InitializePool(prefab, 1); // or your default size
//        }

//        if (prefabPools[prefab].Count == 0)
//        {
//            GameObject obj = Instantiate(prefab);
//            obj.SetActive(false);
//            prefabPools[prefab].Enqueue(obj);
//        }

//        GameObject pooledObj = prefabPools[prefab].Dequeue();
//        pooledObj.SetActive(true);
//        pooledObj.transform.position = position;
//        return pooledObj;
//    }

//    public void ReturnToPool(GameObject obj)
//    {
//        obj.SetActive(false);

//        if (!prefabPools.ContainsKey(obj))
//        {
//            prefabPools[obj] = new Queue<GameObject>();
//        }

//        prefabPools[obj].Enqueue(obj);
//    }
//}


public enum ObjectType
{
    Coin,
    PowerUp,
    Obstacle
}