using UnityEngine;

public class PlayerInput : MonoSingleton<PlayerInput>
{
    /// <summary>
    /// PlayerInput is a singleton that gathers player input.
    /// Other classes can ask it about what the player is doing.
    /// </summary>

    public static float horizontalInput;
    public static float verticalInput;

    /// <summary>
    /// Collect all information about the state that the player
    /// input is currently in.
    /// </summary>
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");        
    }
}
