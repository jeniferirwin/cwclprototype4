using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    
    private Rigidbody playerRb;
    private GameObject focalPoint;

    /// <summary>
    /// Gather information that the player object needs to know about.
    /// Rigidbody and FocalPoint.
    /// </summary>

    private void Start()
    {
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
        playerRb.AddForce(focalPoint.transform.forward * moveSpeed * PlayerInput.verticalInput, ForceMode.Acceleration);
    }
}
