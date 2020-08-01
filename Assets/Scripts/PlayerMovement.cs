using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    
    private Rigidbody playerRb;
    private GameObject focalPoint;

    public Powerup currentPowerup;

    public delegate void CollectedPowerupEvent();
    public static event CollectedPowerupEvent CollectedPowerup;
    
    /// <summary>
    /// Gather information that the player object needs to know about.
    /// Rigidbody and FocalPoint.
    /// </summary>

    private void Start()
    {
        currentPowerup = null;
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.FindGameObjectWithTag(GameManager.TAG_FOCALPOINT);
    }

    /// <summary>
    /// Ask the PlayerInput manager about player input and move accordingly.
    /// We are only moving forward because our turning depends upon the camera
    /// rotation, found in RotateCamera.cs.
    /// </summary>
    
    private void FixedUpdate()
    {
        if (currentPowerup.powerupType == Powerup.PowerupTypes.MOBILITY)
        {
            // move at 150% speed if we're affected by mobility powerup
            playerRb.AddForce(focalPoint.transform.forward * moveSpeed * PlayerInput.verticalInput * 1.5f, ForceMode.Acceleration);
        }
        else
        {
            playerRb.AddForce(focalPoint.transform.forward * moveSpeed * PlayerInput.verticalInput, ForceMode.Acceleration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameManager.TAG_POWERUP))
        {
            // learn what the powerup type of the powerup is, then apply it to ourselves
            // and raise the event that indicates we received it - then deactivate the
            // powerup object
            CollectedPowerup?.Invoke();
            other.gameObject.SetActive(false);
        }
    }
}
