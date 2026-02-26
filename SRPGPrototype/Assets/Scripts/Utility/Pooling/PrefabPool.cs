using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool<T> : IPool<T> where T : MonoBehaviour
{
    private readonly GameObject prefab;
    private readonly Queue<T> pool;
    private readonly Transform container;

    public PrefabPool(GameObject prefab, Transform container, int capacity)
    {
        this.prefab = prefab;
        this.container = container;
        pool = new Queue<T>(capacity);
    }

    public T Get()
    {
        if(pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        return GameObject.Instantiate(prefab, container).GetComponent<T>();
    }

    public void Release(T obj)
    {
        pool.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
}
