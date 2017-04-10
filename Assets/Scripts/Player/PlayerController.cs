using UnityEngine;
using System.Collections;

public struct PlayerInfo
{
    public PLAYERTYPE playerType;    
    public int score;
    public SHIELDSTATE shieldState;
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
	float distanceTraveled = 0;

    /// <summary>
    /// Player Info
    /// </summary>
    PlayerInfo playerInfo;

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
    /// Initialze ship parameters(current speed and acceleration)
    /// </summary>
    void InitShipParam()
    {
        accelerationOverride = 0;
        sc.currentSpeed = 0;
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

    public int PlayerScore
    {
        set { playerInfo.score = value; Debug.Log("Player Score" + playerInfo.score); }
        get { return playerInfo.score; }
    }
}


public struct PlayerShip
{
    public Ship ship;
    public GameObject go;
}
