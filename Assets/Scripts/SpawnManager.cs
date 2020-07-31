using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpawnManager creates and holds a pool of reusable enemy GameObjects.
/// </summary>
public class SpawnManager : MonoSingleton<SpawnManager>
{
    // mobileContainer is an empty GameObject that we parent all of the
    // enemies to.
    public GameObject mobileContainer;
    public GameObject enemyPrefab;
    public int initPoolAmount;
    private List<GameObject> enemyPool = new List<GameObject>();
    private Quaternion startingRotation;

    // TODO: Might use this event for changing how powerups are spawned during runtime
    public delegate void EnemySpawnedEvent();
    public static event EnemySpawnedEvent EnemySpawned;

    /// <summary>
    /// Initialize the enemy pool and set all of them inactive
    /// to start with.
    /// </summary>
    void Start()
    {
        startingRotation = enemyPrefab.transform.rotation;
        IncreaseEnemyPool(initPoolAmount,true);
        Debug.Log("Pool count: " + enemyPool.Count);
        StartCoroutine("TestSpawnPoints");
    }

    IEnumerator TestSpawnPoints()
    {
        do
        {
            SpawnEnemy();
            yield return new WaitForSeconds(3);
        } while (true);
    }
    /// <summary>
    /// Create [amount] new enemies and add them to the pool.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="autoDisable">Whether or not to start the new enemy in disabled mode or not.</param>
    private void IncreaseEnemyPool(int amount, bool autoDisable)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject _newEnemy = Instantiate(enemyPrefab, Vector3.zero, startingRotation, mobileContainer.transform);
            enemyPool.Add(_newEnemy);
            if (autoDisable)
                _newEnemy.SetActive(false);
        }
    }

    private GameObject GetInactiveEnemy()
    {
        enemyPool = Shuffle(enemyPool);
        foreach (GameObject _enemy in enemyPool)
        {
            if (_enemy.activeSelf == false)
            {
                _enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _enemy.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                _enemy.transform.rotation = enemyPrefab.transform.rotation;
                return _enemy;
            }
        }
        // if we didn't find any inactive enemies, we'll return
        // null so that the caller can check it and do something
        // about that.
        return null;
    }

    public void SpawnEnemy()
    {
        Vector3 _spawnPosition = RandomizedSpawnPoint();
        GameObject _inactiveEnemy = GetInactiveEnemy();
        if (_inactiveEnemy)
        {
            Debug.Log("Activating an enemy.");
            _inactiveEnemy.transform.position = _spawnPosition;
            _inactiveEnemy.SetActive(true);
        }
        // TODO: Delete this event if we never end up using it.
        EnemySpawned?.Invoke();
    }

    /// <summary>
    /// Picks a random spot near the middle of the platform that we want
    /// to spawn an enemy at.
    /// 
    /// If it happens to pick a spot that would cause the enemy to telefrag
    /// the player or another enemy, it continues generating new Vector3s until
    /// we have one that is clear of the player.
    /// </summary>
    /// <returns></returns>
    public Vector3 RandomizedSpawnPoint()
    {
        // we don't want to spawn within 2 units of anyone else.
        // if we simply can't find a position after 1000 tries,
        // we'll return Vector3.zero so the caller can decide how to
        // handle it.
        float _minXDistance = 2f;
        float _minZDistance = 2f;
        int maxTries = 1000;


        Vector3 _playerPosition = GameManager.Player.transform.position;
        List<Vector3> _enemyPositions = GetAllEnemyPositions(enemyPool);
        _enemyPositions.Add(_playerPosition);
        do
        {
            // TODO: Make the below not hardcoded.
            // The following lines pick a random spot in the middling area of the
            // platform
            float _xPos = Random.Range(-8f, 8f);
            float _yPos = 0.1f; // this puts our spheres juuuust above the platform
            float _zPos = Random.Range(-5f, 5f);
            bool telefragging = false;

            foreach (Vector3 _pos in _enemyPositions)
            {
                if (Mathf.Abs(_pos.x - _xPos) <= _minXDistance || Mathf.Abs(_pos.z - _zPos) <= _minZDistance)
                {
                    /**
                    Debug.Log(string.Format("{0}x - {1}x = ABS {2}x",_pos.x, _xPos, Mathf.Abs(_pos.x - _xPos)));
                    Debug.Log(string.Format("{0}z - {1}z = ABS {2}z",_pos.z, _zPos, Mathf.Abs(_pos.z - _zPos)));
                    Debug.Log("We are telefragging.");
                    **/
                    telefragging = true;
                    break;
                }
            }

            // if we've made it this far without activating telefragging, that
            // means we have a valid position to spawn in and can return it
            if (!telefragging)
                return new Vector3(_xPos, _yPos, _zPos);

            maxTries--;
        } while (maxTries > 0);
        return Vector3.zero;
    }

    /// <summary>
    /// Get a list of all active enemy positions.
    /// </summary>
    /// <param name="enemyPool"></param>
    /// <returns></returns>
    public List<Vector3> GetAllEnemyPositions(List<GameObject> enemyPool)
    {
        List<Vector3> _enemyPositions = new List<Vector3>();
        List<GameObject> _activeEnemies = GetAllActiveEnemies(enemyPool);
        foreach (GameObject _enemy in _activeEnemies)
        {
            _enemyPositions.Add(_enemy.transform.position);
        }
        return _enemyPositions;
    }

    /// <summary>
    /// Returns a list of all enemies that are currently in an active state.
    /// </summary>
    /// <param name="enemyPool"></param>
    /// <returns></returns>
    public List<GameObject> GetAllActiveEnemies(List<GameObject> enemyPool)
    {
        List<GameObject> _activeEnemies = new List<GameObject>();
        foreach (GameObject _enemy in enemyPool)
        {
            if (_enemy.activeSelf == true)
            {
                _activeEnemies.Add(_enemy);
            }
        }
        Debug.Log("Active enemies: " + _activeEnemies.Count);
        return _activeEnemies;
    }

    /// <summary>
    /// This is used for when we're working with an object pool that has
    /// a variety of randomized objects and is not seeing frequent heavy use.
    /// 
    /// For cases where the object pool has randomized elements (such as
    /// enemies with different stats), working with that pool has the
    /// possibility of just returning the same exact object every time we
    /// request an object from it. For random spawns, we don't want this.
    /// 
    /// Shuffle is used to randomly reorder an object pool so that this doesn't
    /// happen.
    /// 
    /// </summary>
    /// <param name="list">The object pool to reorder.</param>
    /// <returns>Reordered object pool.</returns>
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
