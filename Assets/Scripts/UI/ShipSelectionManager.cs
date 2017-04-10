using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipSelectionManager : MonoBehaviour
{
    ShipDatabase database;
    PlayerShipData playerData;

    public int currentShipID;

    public Ship middle, left, right;

    //BOTTOM
    public Image currentImg, currentRImg, currentLImg;

    public Image middleImg, leftImg, RightImg;

    public Text shipText;
    public Text shipDataText;

    public GameObject[] firstRow, secondRow, thirdRow;

    public GameObject buyButton, buyImage, buyImg1, buyTxt, lockedShipImg;

    void Awake()
    {
        database = FindObjectOfType<ShipDatabase>();
        playerData = FindObjectOfType<PlayerShipData>();
    }

    void Start()
    {
        SetLayout();

        print(middle.shipName);

        playerData.playerShip = middle;
        ResetDotData();

        SetDotData(middle);
    }

    void Update()
    {
        middleImg.sprite = middle.shipSprites[0];
        leftImg.sprite = left.shipSprites[0];
        RightImg.sprite = right.shipSprites[0];

        shipText.text = middle.shipName;

        if (middle.shipName == "ShipName_02")
        {
            SetLockButtons();
        }
        else
        {
            ShipAlreadyOwned();
        }

        //shipDataText.text =  "DEBUG INFO: " + " Ship health: " + middle.shipHealth + " Ship armor: " + middle.shipArmor + " Ship speed: " + middle.shipSpeed;
    }

    void SetLockButtons()
    {
        buyButton.SetActive(false);
        buyImage.SetActive(false);
        buyImg1.SetActive(false);
        buyTxt.SetActive(false);
        lockedShipImg.SetActive(false);
    }

    void ShipAlreadyOwned()
    {
        buyButton.SetActive(true);
        buyImage.SetActive(true);
        buyImg1.SetActive(true);
        buyTxt.SetActive(true);
        lockedShipImg.SetActive(true);
    }

    public void SwitchRight()
    {
        ResetDotData();

        Ship s1, s2, s3;

        s1 = left;
        s2 = middle;
        s3 = right;

        middle = s1;
        right = s2;
        left = s3;

        currentImg.sprite = middle.shipSprites[0];
        currentLImg.sprite = middle.shipSprites[1];
        currentRImg.sprite = middle.shipSprites[2];

        playerData.playerShip = middle;
        SetDotData(middle);
    }

    public void SwitchLeft()
    {
        ResetDotData();

        Ship s1, s2, s3;

        s1 = left;
        s2 = middle;
        s3 = right;

        middle = s3;
        right = s1;
        left = s2;

        currentImg.sprite = middle.shipSprites[0];
        currentLImg.sprite = middle.shipSprites[1];
        currentRImg.sprite = middle.shipSprites[2];

        playerData.playerShip = middle;
        SetDotData(middle);
    }

    void SetLayout()
    {
        for (int i = 0; i < database.shipDatabase.Count; i++)
        {
            if (database.shipDatabase[i].shipID == 1)
            {
                left = database.shipDatabase[i];
            }

            if (database.shipDatabase[i].shipID == 2)
            {
                middle = database.shipDatabase[i];
            }

            if (database.shipDatabase[i].shipID == 3)
            {
                right = database.shipDatabase[i];
            }
        }

        currentImg.sprite = middle.shipSprites[0];
        currentLImg.sprite = middle.shipSprites[1];
        currentRImg.sprite = middle.shipSprites[2];
    }

    void SetDotData(Ship shipToCalculate)
    {
        if (shipToCalculate.shipHealth >= 80)
        {
            secondRow[4].SetActive(true);
        }
        if (shipToCalculate.shipHealth >= 60)
        {
            secondRow[3].SetActive(true);
        }
        if (shipToCalculate.shipHealth >= 40)
        {
            secondRow[2].SetActive(true);
        }
        if (shipToCalculate.shipHealth >= 20)
        {
            secondRow[1].SetActive(true);
        }
        if(shipToCalculate.shipHealth > 0)
        {
            secondRow[0].SetActive(true);
        }


        if (shipToCalculate.shipArmor >= 8)
        {
            firstRow[4].SetActive(true);
        }
        if (shipToCalculate.shipArmor >= 6)
        {
            firstRow[3].SetActive(true);
        }
        if (shipToCalculate.shipArmor >= 4)
        {
            firstRow[2].SetActive(true);
        }
        if (shipToCalculate.shipArmor >= 2)
        {
            firstRow[1].SetActive(true);
        }
        if (shipToCalculate.shipArmor > 0)
        {
            firstRow[0].SetActive(true);
        }


        if (shipToCalculate.shipSpeed >= 80)
        {
            thirdRow[4].SetActive(true);
        }
        if (shipToCalculate.shipSpeed >= 60)
        {
            thirdRow[3].SetActive(true);
        }
        if (shipToCalculate.shipSpeed >= 40)
        {
            thirdRow[2].SetActive(true);
        }
        if (shipToCalculate.shipSpeed >= 20)
        {
            thirdRow[1].SetActive(true);
        }
        if (shipToCalculate.shipSpeed > 0)
        {
            thirdRow[0].SetActive(true);
        }

    }

    void ResetDotData()
    {
        for (int i = 0; i < firstRow.Length; i++)
        {
            firstRow[i].SetActive(false);
        }

        for (int i = 0; i < secondRow.Length; i++)
        {
            secondRow[i].SetActive(false);
        }

        for (int i = 0; i < thirdRow.Length; i++)
        {
            thirdRow[i].SetActive(false);
        }
    }
}
