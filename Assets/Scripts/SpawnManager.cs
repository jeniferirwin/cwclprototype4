using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpawnManager creates and holds pools of reusable enemy and powerup
/// GameObjects.
/// </summary>
public class SpawnManager : MonoSingleton<SpawnManager>
{
    // mobileContainer is an empty GameObject that we parent all of the
    // enemies to.
    //
    // powerupContainer is an empty GameObject that we parent all of the
    // powerups to.
    public GameObject mobileContainer;
    public GameObject powerupContainer;
    public GameObject[] enemyPrefabs;
    public GameObject[] powerupPrefabs;
    public int initEnemyPoolAmount;
    public int initPowerupPoolAmount;
    private List<GameObject> enemyPool = new List<GameObject>();
    private List<GameObject> powerupPool = new List<GameObject>();
    private Quaternion startingEnemyRotation;
    private Quaternion startingPowerupRotation;

    // TODO: Might use this event for changing how powerups are spawned during runtime
    public delegate void EnemySpawnedEvent();
    public static event EnemySpawnedEvent EnemySpawned;
    
    public delegate void PowerupSpawnedEvent();
    public static event PowerupSpawnedEvent PowerupSpawned;

    /// <summary>
    /// Initialize the pools and set all of the resulting objects inactive
    /// to start with.
    /// </summary>
    void Start()
    {
        IncreasePool(GameManager.TAG_ENEMY,initEnemyPoolAmount,true);
        IncreasePool(GameManager.TAG_POWERUP,initPowerupPoolAmount,true);
        StartCoroutine("TestPowerups");
    }

    private IEnumerator TestPowerups()
    {
        yield return new WaitForSeconds(2);
        
    }

    /// <summary>
    /// Create <amount> new entities associated with tag <tag> and add
    /// them to the appropriate pool.
    /// </summary>
    /// <param name="tag">The tag of the object type for the pool.</param>
    /// <param name="amount">The number of new objects to generate for the pool.</param>
    /// <param name="Disable">Whether to auto-disable the object upon creation.</param>
    private void IncreasePool(string tag, int amount, bool autoDisable)
    {
        // this is kind of hacky, but it's the best way I can think of
        // to accomplish this right now.
        //
        // In order to help IncreasePool know what prefabs, containers and
        // objects to use, we derive these values from tag.

        GameObject[] _entityPrefabs = PrefabsFromTag(tag);
        GameObject _entityContainer = ContainerFromTag(tag);
        List<GameObject> _entityPool = PoolFromTag(tag);

        for (int i = 0; i < amount; i++)
        {
            GameObject _selectedPrefab = _entityPrefabs[Random.Range(0,_entityPrefabs.Length)];
            Quaternion _startingRotation = _selectedPrefab.transform.rotation;
            GameObject _newEntity = Instantiate(_selectedPrefab, Vector3.zero, _startingRotation, _entityContainer.transform);
            _entityPool.Add(_newEntity);
            if (autoDisable)
                _newEntity.SetActive(false);
        }
    }

    //TODO: Can we make the following three functions somehow compressed into one while retaining clarity?

    private List<GameObject> PoolFromTag(string tag)
    {
        switch (tag)
        {
            case GameManager.TAG_ENEMY:
                return enemyPool;
            case GameManager.TAG_POWERUP:
                return powerupPool;
            default:
                Debug.LogError("Can't find a prefab list matching tag: " + tag);
                break;
        }
        return null;
    }

    private GameObject[] PrefabsFromTag(string tag)
    {
        switch (tag)
        {
            case GameManager.TAG_ENEMY:
                return enemyPrefabs;
            case GameManager.TAG_POWERUP:
                return powerupPrefabs;
            default:
                Debug.LogError("Can't find a prefab list matching tag: " + tag);
                break;
        }
        return null;
    }

    private GameObject ContainerFromTag(string tag)
    {
        switch (tag)
        {
            case GameManager.TAG_ENEMY:
                return mobileContainer;
            case GameManager.TAG_POWERUP:
                return powerupContainer;
            default:
                Debug.LogError("Can't find a prefab list matching tag: " + tag);
                break;
        }
        return null;
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

                // TODO: The [0] here is a dirty hack. Need to think of a better way.

                _enemy.transform.rotation = enemyPrefabs[0].transform.rotation;
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
            float _tryXPos = Random.Range(-8f, 8f);
            float _tryYPos = 15f;
            float _tryZPos = Random.Range(-5f, 5f);

            // but what if we're spawning inside another entity? we don't want
            // that, so let's set up to pick another vector if so
            bool telefragging = false;

            foreach (Vector3 _existingEnemyPos in _enemyPositions)
            {
                // diffX and diffZ are the difference between an existing enemy
                // position and our potential spawn position. if either of these
                // are less than the minimum distances declared up there before
                // our do loop, then we're telefragging and will generate
                // another vector.

                float _diffX = Mathf.Abs(_existingEnemyPos.x - _tryXPos);
                float _diffZ = Mathf.Abs(_existingEnemyPos.z - _tryZPos);

                if (_diffX <= _minXDistance || _diffZ <= _minZDistance)
                {
                    telefragging = true;
                    break;
                }
            }

            // if we've made it this far without activating telefragging, that
            // means we have a valid position to spawn in and can return it
            if (!telefragging)
                return new Vector3(_tryXPos, _tryYPos, _tryZPos);

            maxTries--;
        } while (maxTries > 0);

        // if we've tried 1000 times to keep from telefragging and it's
        // not working, we'll return Vector3.zero. the calling function
        // should preferably be aware that this needs to be handled in
        // some other way
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
