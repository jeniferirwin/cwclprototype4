using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    /// <summary>
    /// GameManager is a singleton that holds information about score,
    /// as well as const strings, because I don't like how code looks
    /// when strings are right there in function calls.
    /// </summary>

    public const string TAG_MAINCAMERA = "MainCamera";
    public const string TAG_FOCALPOINT = "FocalPoint";
    public const string TAG_PLAYER     = "Player";
    public const string TAG_ENEMY      = "Enemy";
    public const string TAG_POWERUP    = "Powerup";

    public int Score;
    public static GameObject Player;

    // TODO: Later we will subscribe to enemy despawn events.

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag(TAG_PLAYER);
    }
}
