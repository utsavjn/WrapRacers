using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ship
{
    public float shipHealth;
    public string shipName;
    public float shipSpeed;
    public string shipNickname;
    public Sprite[] shipSprites;

    public float shipArmor;

    public int shipID;

    public Ship(float newShipHealth, string newShipName, float newShipSpeed, string newShipNickname, int newShipID, Sprite[] newShipSprites, float newShipArmor)
    {
        shipHealth = newShipHealth;
        shipName = newShipName;
        shipSpeed = newShipSpeed;
        shipID = newShipID;
        shipSprites = newShipSprites;
        shipArmor = newShipArmor;

        shipNickname = newShipNickname;
    }

    public Ship()
    {

    }

}
