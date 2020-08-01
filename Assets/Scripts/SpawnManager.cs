using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpawnManager creates and holds pools of reusable enemy and powerup
/// GameObjects.
/// </summary>
public class SpawnManager : MonoSingleton<SpawnManager>
{
    public GameObject enemyContainer;
    public GameObject powerupContainer;

    public float spawnRadius;
    public float minEntityDistance;

    private RandomizedObjectPool enemyPool;
    private RandomizedObjectPool powerupPool;

    private void Start()
    {
        enemyPool = enemyContainer.GetComponent<RandomizedObjectPool>();
        powerupPool = powerupContainer.GetComponent<RandomizedObjectPool>();
    }

    private void SpawnRandomObject(RandomizedObjectPool pool)
    {
        Vector3 _spawnPosition = GetValidSpawnPosition();
    }

    /// <summary>
    /// Gets a list of all active entities, including the player, then 
    /// tries up to 1000 times to get a valid spawn position that is
    /// not too close to an active entity. Most of the time the loop
    /// won't need to run very many times to find a valid position,
    /// but if somehow it can't, it returns Vector3.zero.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetValidSpawnPosition()
    {
        List<GameObject> _activeEntities = new List<GameObject>();

        _activeEntities.AddRange(enemyPool.GetAllActiveObjects());
        _activeEntities.AddRange(powerupPool.GetAllActiveObjects());
        _activeEntities.Add(GameManager.Player);

        Vector3 _spawnPosition = Vector3.zero;
        bool telefragging = false;
        int maxTries = 1000;

        do
        {
            float _xTryPos = Random.Range(-spawnRadius, spawnRadius);
            float _zTryPos = Random.Range(-spawnRadius, spawnRadius);

            foreach (GameObject _enemy in _activeEntities)
            {
                float _xDiff = Mathf.Abs(_enemy.transform.position.x - _xTryPos);
                float _zDiff = Mathf.Abs(_enemy.transform.position.z - _zTryPos);
                if (_xDiff <= minEntityDistance || _zDiff <= minEntityDistance)
                {
                    telefragging = true;
                    break;
                }
                else
                {
                    _spawnPosition = new Vector3(_xTryPos, 0.1f, _zTryPos);
                }
            }
            
            if (!telefragging)
                return _spawnPosition;

            maxTries--;
        } while (maxTries > 0);
        return _spawnPosition;
    }
}
