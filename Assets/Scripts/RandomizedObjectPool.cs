using UnityEngine;
using System.Collections.Generic;

public class RandomizedObjectPool : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    public int initObjectAmount;
    private List<GameObject> objectPool = new List<GameObject>();

    private void Start()
    {
        AddToPool(initObjectAmount,false);
    }

    /// <summary>
    /// Generates <amount> new objects from objectPrefabs to add to
    /// objectPool.
    /// </summary>
    /// <param name="amount">The number of new objects to add.</param>
    /// <param name="spawnActive">Whether or not the object should be set active right away.</param>
    public void AddToPool(int amount, bool spawnActive)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject _randomPrefab = objectPrefabs[Random.Range(0,objectPrefabs.Length)];
            Quaternion _startingRotation = _randomPrefab.transform.rotation;
            GameObject _newObject = Instantiate(_randomPrefab, Vector3.zero, _startingRotation, gameObject.transform);
            objectPool.Add(_newObject);
            if (!spawnActive)
                _newObject.SetActive(false);
        }
    }

    /// <summary>
    /// Gets all currently active objects in our pool.
    /// I'm sure there's a better way to do this with LINQ,
    /// but I'm not quite that advanced yet.
    /// </summary>
    /// <returns>List of currently active objects in the pool.</returns>
    public List<GameObject> GetAllActiveObjects()
    {
        List<GameObject> _activeObjects = new List<GameObject>();
        foreach (GameObject _obj in objectPool)
        {
            if (_obj.activeSelf)
            {
                _activeObjects.Add(_obj);
            }
        }
        return _activeObjects;
    } 

    /// <summary>
    /// Gets all currently inactive objects in our pool.
    /// I'm sure there's a better way to do this with LINQ,
    /// but I'm not quite that advanced yet.
    /// </summary>
    /// <returns>List of currently inactive objects in the pool.</returns>
    public List<GameObject> GetAllInactiveObjects()
    {
        List<GameObject> _inactiveObjects = new List<GameObject>();
        foreach (GameObject _obj in objectPool)
        {
            if (_obj.activeSelf == false)
            {
                _inactiveObjects.Add(_obj);
            }
        }
        return _inactiveObjects;
    } 

    /// <summary>
    /// Find a random inactive object.
    /// </summary>
    /// <returns>A random inactive object in the pool.</returns>
    public GameObject GetRandomInactiveObject()
    {
        objectPool = Shuffle(objectPool);
        foreach (GameObject _obj in objectPool)
        {
            if (_obj.activeSelf == false)
            {
                return _obj;
            }
        }
        // if we've made it here, we just have to
        // add a new object to the pool and return that
        AddToPool(1,false);
        return objectPool[objectPool.Count];
    } 

    /// <summary>
    /// Return a randomized version of a GameObject list.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private List<GameObject> Shuffle(List<GameObject> list)
    {
        List<GameObject> _newList = new List<GameObject>();
        while (list.Count > 0)
        {
            var _newRandom = Random.Range(0, list.Count);
            _newList.Add(list[_newRandom]);
            list.RemoveAt(_newRandom);
        }
        return _newList;
    }
}
