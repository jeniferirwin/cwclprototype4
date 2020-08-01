using UnityEngine;
using System;

public class Powerup : MonoBehaviour
{
    public enum PowerupTypes
    {
        NONE,
        STRENGTH,
        MOBILITY,
        NOVA
    }

    public PowerupTypes powerupType;
}
