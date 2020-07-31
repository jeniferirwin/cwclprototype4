using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    // TODO: possibly subscribe to powerups here later

    public float rotateSpeed;

    /// <summary>
    /// Rotate the camera around the playing field based on player input
    /// gathered from the PlayerInput manager.
    /// </summary>
    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, PlayerInput.horizontalInput * rotateSpeed * Time.fixedDeltaTime, 0));
    }
}
