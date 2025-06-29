using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new();
    [SerializeField] private Pool[] pools;
    [SerializeField] private Transform poolManagerTransform;



    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }



    private void Start()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            CreatePool(pools[i].prefab, pools[i].poolSize);
        }
    }



    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;

        GameObject parentGameObject = new(prefabName + "Anchor"); // 创建父物体，用于挂载子物体

        parentGameObject.transform.SetParent(poolManagerTransform);


        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform);
                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            // 从对象池中获取对象
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObject(position, rotation, objectToReuse, prefab);

            return objectToReuse;
        }
        else
        {
            Debug.Log(prefab + "没有对象池");
            return null;
        }
    }


    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(objectToReuse);

        if (objectToReuse.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }



        return objectToReuse;
    }
    private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;


        //  objectToReuse.GetComponent<Rigidbody2D>().velocity=Vector3.zero;
        objectToReuse.transform.localScale = prefab.transform.localScale;

    }

}
