using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameData;

public struct PlayerInfo
{
    public PLAYERTYPE playerType;
    public SHIELDSTATE shieldState;
    public int score;
    public bool booster;
    public float topSpeed;
    public float baseTopSpeed;
    public float curSpeed;
    public int health;
    public int itemCount;
}

public enum PLAYERTYPE
{
    None = 0,
    FastLaserSingle = 1,
    MediumTwinShortLaser = 2,
    SlowTripleLazer = 3,
    MediumTwinLongLazer  =4,
}

public enum SHIELDSTATE
{
    Off = 0,
    On = 1,
}

public enum PLAYERSTATE
{
	None = 0,
	Alive = 1,
	Die = 2,
}

public class PlayerController : MonoBehaviour {

	PlayerShip ship = new PlayerShip();
	public ShipController sc;

	public bool controlsEnabled = true;

	public int points = 0;

    /// <summary>
    /// Player Info
    /// </summary>
    public PlayerInfo playerInfo;
    private List<int> m_levelDataIdList = new List<int>();

    public Text str_playerScore;
    public Text str_playerBooster;

    void OnEnable()
    {
        RaceManager.OnInitPlayer += InitShipParam;
        //RaceManager.OnRaceStart += InitShipParam;
        RaceManager.OnRaceEnd += InitShipParam;
    }

    void OnDisable()
    {
        //RaceManager.OnRaceStart -= InitShipParam;
		//RaceManager.OnRaceEnd -= InitShipParam;
    }

	bool ShouldOverride
	{
		get { return accelerationOverride != 0; }
	}

	public float accelerationOverride = 0;

    private void LoadPlayerData(int id)
    {
        playerInfo.baseTopSpeed = LevelData.dataMap[id].shipbasespeed;
        playerInfo.topSpeed = LevelData.dataMap[id].shiptopspeed;

        Debug.Log("playerInfo.baseTopSpeed " + playerInfo.baseTopSpeed);
        Debug.Log("playerInfo.topSpeed " + playerInfo.topSpeed);
    }

    void InitShipByLevel(int idx)
    {
        foreach (var key in LevelData.dataMap.Keys)
        {
            m_levelDataIdList.Add(key);
        }

        for(int i = 0; i < m_levelDataIdList.Count; i++)
        {
            string str = "level_" + idx.ToString();
            if (str.Equals(LevelData.dataMap[m_levelDataIdList[i]].name))
            {
                LoadPlayerData(LevelData.dataMap[m_levelDataIdList[i]].id);
            }
        }
    }

    void Initialize()
    {
        Debug.Log("InitShipByLevel(RaceManager._instance.CurrentLap);");
        InitShipByLevel(RaceManager._instance.CurrentLap);
    }

    void Awake()
    {
        if (!sc) sc = ship.go.GetComponent<ShipController>();        
    }

    private void Start()
    {
        Initialize();
    }
    /// <summary>
    /// Initialze player(ship) move parameters(current speed and acceleration)
    /// </summary>
    void InitShipParam()
    {        
        accelerationOverride = 0;
        sc.currentSpeed = 0;
    }

    public void InitPlayerItem()
    {
        GameObject[] objList = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject obj in objList)
        {
            Destroy(obj);
        }

        DeactivateShield();
    }

    void Update()
    {
  		if (controlsEnabled) 
			ControlShip();
	}

    #region Ship Powerup Interface Functions
    public void ActivateShield() {
      sc.ActivateShield();
    }
    public void DeactivateShield() {
      sc.DeactivateShield();
    }
    #endregion

    public void IncrementSpeed(float increment)
    {
        sc.IncrementSpeed(increment);
    }

    private void ControlShip()
    {
        // temporary
        if (Input.GetKeyDown(KeyCode.Space))
        {
			//sc.ToggleThrust();
			sc.Fire();
        }

		sc.Move();

		// Override phone acceleration with keyboard before we grab it for the rotation
		if (Input.GetAxis("Horizontal") != 0)
		{
			OverrideAcceleration(Input.GetAxis("Horizontal"));
		}

		sc.RotateByTilt(GetAcceleration());
    }

	float GetAcceleration()
	{
		if (!ShouldOverride)
		{
			return Input.acceleration.x;
		}
		else {
            return accelerationOverride;
		}
	}

	public void OverrideAcceleration(float acceleration)
	{
		accelerationOverride = acceleration;
	}

	public void Fire()
	{
		sc.Fire();
	}

	public void ToggleThrust()
	{
		sc.thrustActive = !sc.thrustActive;
	}
    /// <summary>
    /// a partial Player Score
    /// </summary>
    public int PlayerScore
    {
        set
        {
            playerInfo.score = value;
            ShowPlayerScore();
        }
        get { return playerInfo.score; }
    }

    public void AddPlayerScore(int score)
    {
        if (playerInfo.booster)
            score = (int)(score * 1.2f);
        PlayerScore += score;
    }

    void ShowPlayerScore()
    {
        str_playerScore.text = PlayerScore.ToString();
    }

    void InitPlayerScore()
    {
        playerInfo.score = 0;
        ShowPlayerScore();
    }

    public void SetPlayerBooster(bool bFlag)
    {
        playerInfo.booster = bFlag;
        str_playerBooster.text = "1.2" + "x";
        Debug.Log("playerInfo.booster = bFlag=====" + playerInfo.booster);
    }

    public void SetPlayerHealth(int delta)
    {
        playerInfo.health += delta;
        Debug.Log("playerInfo.health += delta=====" + playerInfo.health);
    }

    public void SetPlayerEnergy(float delta)
    {
        playerInfo.topSpeed += delta;
        Debug.Log("playerInfo.topSpeed += delta=====" + playerInfo.topSpeed);
    }

    public void SetPlayerBomb()
    {

    }
}


public struct PlayerShip
{
    public Ship ship;
    public GameObject go;
}
