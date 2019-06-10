using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool : Singleton<ObjectsPool>
{	
    Dictionary<PoolObjTypes, Queue<GameObject>> poolsDictionary;
    public List<Pool> pools;

    void Start() 
    {
        poolsDictionary = new Dictionary<PoolObjTypes, Queue<GameObject>>();
        for (int i = 0; i < pools.Count; i++)
        {
            poolsDictionary.Add(pools[i].type, new Queue<GameObject>());
        }
    }
    public void AddObjectToPool(PoolObjTypes type, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            GameObject obj = Instantiate(pools[pools.FindIndex(_ => _.type == type)].prefab);
            obj.name = type.ToString();
            obj.SetActive(false);
            poolsDictionary[type].Enqueue(obj);
        }
    }   
	public void ReturnToPool(PoolObjTypes type, GameObject obj)
    {
		obj.SetActive(false);
		poolsDictionary[type].Enqueue(obj);
	}
	public GameObject SpawnObject(PoolObjTypes type, Vector3 position, Quaternion rotation) 
    {
        if(poolsDictionary[type].Count == 0)
        {
            AddObjectToPool(type, 1);
        }
        GameObject newObj = poolsDictionary[type].Dequeue();
        
        newObj.SetActive(true);
        newObj.transform.position = position;
        newObj.transform.rotation = rotation;
        return newObj;
	}
}