using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipSelectButton : MonoBehaviour
{
    public int thisShipID;

    public Text shipName;
    public Text shipSpeed;
    public Text shipHealth;
    public Sprite shipIMG;

    ShipDatabase database;
    PlayerShipData playerDat;
    MenusData menuData;

    void Awake()
    {
        menuData = FindObjectOfType<MenusData>();
        database = FindObjectOfType<ShipDatabase>();
    }

    void Start()
    {
        DataToString();
        playerDat = FindObjectOfType<PlayerShipData>();
    }

    void DataToString()
    {
        for (int i = 0; i < database.shipDatabase.Count; i++)
        {
            if (database.shipDatabase[i].shipID == thisShipID)
            {
                shipName.text = database.shipDatabase[i].shipName.ToString();
                shipSpeed.text = "Ship Speed: " + database.shipDatabase[i].shipSpeed.ToString();
                //shipHealth.text = database.shipDatabase[i].shipHealth.ToString();
            }
        }

        
    }

    public void SelectShip()
    {
        for (int i = 0; i < database.shipDatabase.Count; i++)
        {
            if (database.shipDatabase[i].shipID == thisShipID)
            {
                playerDat.playerShip = database.shipDatabase[i];

                playerDat.ReturnPlayerData();

                menuData.PlayerDataToScreen();
                break;
            }
        }
    }
}
