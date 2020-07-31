using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody enemyRb;

    public delegate void EnemyDespawnedEvent();
    public static event EnemyDespawnedEvent EnemyDespawned;

    /// <summary>
    /// Need to know the rigidbody so we can apply forces.
    /// </summary>
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Get a normalized Vector3 of the direction to the player,
    /// accelerate towards them.
    /// </summary>
    void FixedUpdate()
    {
        Vector3 lookDirection = (GameManager.Player.transform.position - transform.position).normalized;
        enemyRb.AddForce(lookDirection * moveSpeed, ForceMode.Acceleration);

        if (transform.position.y < -25)
        {
            enemyRb.velocity = Vector3.zero;
            enemyRb.angularVelocity = Vector3.zero;
            transform.rotation = SpawnManager.Instance.enemyPrefab.transform.rotation;
            gameObject.SetActive(false);
            EnemyDespawned?.Invoke();
        }
    }
}
