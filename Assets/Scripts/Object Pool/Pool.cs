using UnityEngine;
[System.Serializable]
public class Pool 
{
    public PoolObjTypes type;
    public GameObject prefab; 
}

public enum PoolObjTypes
{
    bullets, asteroid
}