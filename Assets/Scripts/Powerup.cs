using UnityEngine;
using System;

public class Powerup : MonoBehaviour
{
    public GameManager.PowerupTypes powerupType;

    private void Start()
    {
        // give ourselves a random powerup type on startup
        // we start at 1 in Random.Range because 0 is type NONE
        var _powerupTypes = Enum.GetValues(typeof(GameManager.PowerupTypes));
        var _selectedType = _powerupTypes.GetValue(UnityEngine.Random.Range(1,_powerupTypes.Length));
        powerupType = (GameManager.PowerupTypes) _selectedType;
    }
}
