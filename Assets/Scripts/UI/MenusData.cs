using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenusData : MonoBehaviour
{
    ShipDatabase database;
    public Button[] buttonList;

    [Header("Player Ship Data")]
    public PlayerShipData playerData;

    public Text playerShipName, playerShipSpeed, playerShipHealth, playerShipNickname;

    void Awake()
    {
        database = FindObjectOfType<ShipDatabase>();
        playerData = FindObjectOfType<PlayerShipData>();
    }

    public void PlayerDataToScreen()
    {
        playerShipName.text = playerData.playerShip.shipName;
        playerShipSpeed.text = "Ship speed:" + playerData.playerShip.shipSpeed.ToString();
        playerShipHealth.text = "Ship health: " + playerData.playerShip.shipHealth.ToString();
    }

}
