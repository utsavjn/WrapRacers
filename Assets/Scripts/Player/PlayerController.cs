using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
public class PlayerController : MonoBehaviour {

	PlayerShip ship = new PlayerShip();
	public ShipController sc;

	public bool controlsEnabled = true;

	public int points = 0;

    /// <summary>
    /// Player Info
    /// </summary>
    public PlayerInfo playerInfo;

    public Text str_playerScore;
    public Text str_playerBooster;

    void OnEnable()
    {
        RaceManager.OnRaceStart += InitShipParam;
    }

    void OnDisable()
    {
        RaceManager.OnRaceStart -= InitShipParam;
    }

	bool ShouldOverride
	{
		get { return accelerationOverride != 0; }
	}

	public float accelerationOverride = 0;

    void Awake()
    {
        if (!sc) sc = ship.go.GetComponent<ShipController>();
    }

    /// <summary>
    /// Initialze player(ship) move parameters(current speed and acceleration)
    /// </summary>
    void InitShipParam()
    {
        accelerationOverride = 0;
        sc.currentSpeed = 0;
    }

    public void InitPlayerInfo()
    {
        GameObject[] objList = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject obj in objList)
        {
            Destroy(obj);
        }
    }

    void Update()
    {
  		if (controlsEnabled) ControlShip();       


  		// if (sc.thrustActive)
  		// {
      //
  		// }
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
