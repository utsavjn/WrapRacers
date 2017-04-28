#define ANDROID_TEST
using UnityEngine;
using System.Collections;
using GameData;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ShipAnimator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(ShipCollisionInteraction))]

public class ShipController : MonoBehaviour {

  	public const float tiltThreshold = .2f;
  	public const float rotationPerTick = 100;

	Rigidbody2D rig;

  	public bool thrustActive = false;

	bool isFiring = false;

	public int weaponDamage = 1;

  	ShipAnimator shipAnimator;

	PlayerStats shipStats;

    PlayerController localPlayer;

	const float LASER_DISTANCE = 100;

	#region Movement Control Attributes

	public float accelerationRate = 2.5f;

	public float decelerationRate = 4f;

    public float baseTopSpeed = 5;
	public float currentSpeed = 0;
	public float topSpeed = 5;

    private float disTraveled;
    private float thresholdDis = 5f;

	private Vector3 firstPoint = new Vector3 (387, 0, 0);
	private Vector3 secondPoint = new Vector3 (-379, 0, 0);
	private const float minRadius = 227f;
	private const float maxRadius = 539f;

    #endregion
    public void IncrementSpeed(float increment)
    {
        if (baseTopSpeed >= LevelData.dataMap[4001].shipbasespeed)
            return;

        baseTopSpeed += increment;
        topSpeed += increment;
    }

    public void DecrementSpeed(float decrement)
    {
        if(baseTopSpeed - topSpeed < 20)
            topSpeed -= decrement;
    }

    public void MomentDecSpeed(float decrement)
    {
        if (baseTopSpeed - topSpeed < 20)
        {
            topSpeed -= decrement;
            currentSpeed -= decrement;
        }
    }

    /// <summary>
    /// initalize a ship
    /// </summary>
    void Initialize()
    {

    }
    void Start()
    {
        shipAnimator = GetComponent<ShipAnimator>();
		shipStats = GetComponent<PlayerStats>();
		rig = GetComponent<Rigidbody2D>();
        Camera.main.GetComponent<CameraController>().Target = transform;
        localPlayer = FindObjectOfType<PlayerController>();
    }

	public void StopFiring()
	{
		isFiring = false;
	}

    /// <summary>
    /// ship shoot obstacle
    /// </summary>
	public void Fire()
	{
		//if (isFiring) return;

		isFiring = true;
		shipAnimator.Fire();

		RaycastHit2D hit;

		if (hit = Physics2D.Raycast(transform.position, transform.up, LASER_DISTANCE,
		                            LayerMaskShifter.IntToLayerMask(LayerMasks.obstacles)))
		{
			hit.transform.GetComponent<Stats>().Hit(1);
		}
	}

	public void Move()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        if (thrustActive)
#else
        if (thrustActive || Input.GetAxis("Vertical") > 0)
#endif
        {
            // accelerate speed by accelerationRate per second
            Accelerate();
		}
		else
        {
			Decelerate();
		}

		ApplyForce ();
        DisTraveledScore();
		GoOutSide ();
	}
    /// <summary>
    /// distance traveled Score
    /// </summary>
    private void DisTraveledScore()
    {
        disTraveled += currentSpeed * Time.fixedDeltaTime;
        
        if (disTraveled > thresholdDis)
        {
            disTraveled = 0;
			localPlayer.AddPlayerScore(1);
        }
    }

    public void ApplyForce()
	{
        if (rig == null)
            rig = GetComponent<Rigidbody2D>();
		rig.velocity = transform.up * currentSpeed;
	}

	void Accelerate()
	{
		currentSpeed = Mathf.Lerp(currentSpeed, topSpeed, accelerationRate * Time.deltaTime);
	}

	void Decelerate()
	{
		currentSpeed = Mathf.Lerp(currentSpeed, 0, decelerationRate * Time.deltaTime);
	}

	void GoOutSide()
	{
		float dis = Vector3.Distance (transform.position, firstPoint);

		if (dis > minRadius && dis < maxRadius)
			return;

		dis = Vector3.Distance (transform.position, secondPoint);

		if (dis > minRadius && dis < maxRadius)
			return;

		RaceManager._instance.EndRace();
	}

    public void ToggleThrust()
    {
        thrustActive = !thrustActive;
    }

    public void RotateByTilt(float tiltValue)
    {
        // make target rotation? no.
        // add on rotation
        if (tiltValue > -tiltThreshold && tiltValue < tiltThreshold) return;

        var newRotation = transform.rotation.eulerAngles;
        newRotation.z += -tiltValue * Time.deltaTime * rotationPerTick;

        transform.eulerAngles = newRotation;

        //HandleTilt(Input.acceleration.x);
    }

    private void HandleTilt(float tiltValue)
    {
        AnimateTilt(tiltValue);
    }

    private void AnimateTilt(float tiltValue)
    {
        shipAnimator.AnimateTilt(tiltValue);
    }

    void ShowPlayerItem(string name)
    {
        string strfullpath = "Prefabs/PowerUpsItem/" + name;
        GameObject obj = Resources.Load(strfullpath) as GameObject;
        GameObject items = (GameObject)Instantiate(obj, transform);
        items.transform.localPosition = new Vector3(20 + localPlayer.playerInfo.itemCount * 10, -1, 0);
        items.transform.localRotation = Quaternion.identity;
        localPlayer.playerInfo.itemCount++;
    }

#region Powerup Interface Functions

    public void ActivateShield()
    {
        shipStats.ActivateShield(1);
        GetUpShield();
    }
    public void DeactivateShield()
    {
      shipStats.DeactivateShield();
    }
    public void GetUpBomb()
    {
        localPlayer.SetPlayerBomb();
        ShowPlayerItem("Bomb");
    }
    public void GetUpHealth()
    {
        localPlayer.SetPlayerHealth(10);
        ShowPlayerItem("Magnet");
    }
    public void GetUpMagnet()
    {
        localPlayer.SetPlayerBooster(true);
        ShowPlayerItem("Magnet");
    }
    public void GetUpRamdom()
    {
        int rand = Random.Range(0, 4);
        switch(rand)
        {
            case 0:
                GetUpHealth();
                break;
            case 1:
                GetUpMagnet();
                break;
            case 2:
                GetUpBomb();
                break;
            case 3:
                GetUpEnergy();
                break;
        }
    }
    public void GetUpEnergy()
    {
        localPlayer.SetPlayerEnergy(10);
        ShowPlayerItem("Magnet");
    }
    public void GetUpShield()
    {
        ShowPlayerItem("Shield");
    }
#endregion
}
