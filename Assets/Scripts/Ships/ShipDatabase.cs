using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipDatabase : MonoBehaviour
{
    //This list will include all Ships available in the game
    public List<Ship> shipDatabase = new List<Ship>();

    void Awake()
    {
                                //HEALTH,NAME,      SPEED, NICKNAME,    ID,     SPRITE LOADING                      ARMOR
        shipDatabase.Add(new Ship(40, "ShipName_01", 40, string.Empty, 1, Resources.LoadAll<Sprite>("Ships/Ship_01"), 2));
        shipDatabase.Add(new Ship(70, "ShipName_02", 80, string.Empty, 2, Resources.LoadAll<Sprite>("Ships/Ship_02"), 6));
        shipDatabase.Add(new Ship(40, "ShipName_03", 100, string.Empty, 3, Resources.LoadAll<Sprite>("Ships/Ship_03"), 4));
    }


}
