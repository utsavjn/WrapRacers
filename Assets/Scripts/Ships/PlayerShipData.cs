using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShipData : MonoBehaviour
{
    public string playerShipName;

    public string playerShipSpeed;

    public string playerShipNickname;

    [HideInInspector]
    public Ship playerShip;

    ShipDatabase database;

    void Awake()
    {
        //GetPlayerData();\

        // need ship data during game
        DontDestroyOnLoad(transform.gameObject);
        database = FindObjectOfType<ShipDatabase>();
    }

    public void ReturnPlayerData()
    {      
        for (int i = 0; i < database.shipDatabase.Count; i++)
        {
            //Temporary.
            if (database.shipDatabase[i].shipID == playerShip.shipID)
            {
                playerShipName = playerShip.shipName;
                playerShipSpeed = playerShip.shipSpeed.ToString();
                playerShipNickname = playerShip.shipNickname;
                break;
            }
        }
    }

}
