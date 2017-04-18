#define ANDROID_TEST
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ShipAnimator))]
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

	const float LASER_DISTANCE = 27;

	#region Movement Control Attributes

	public float accelerationRate = 2.5f;

	public float decelerationRate = 4f;

    public float baseTopSpeed = 5;
	public float currentSpeed = 0;
	public float topSpeed = 5;

    private float disTraveled;
    private float thresholdDis = 50f;

    #endregion
    public void IncrementSpeed(float increment)
    {
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
#if ANDROID_TEST
        if (thrustActive)
#else
        if (thrustActive || Input.GetAxis("Vertical") > 0)
#endif
        {
            // accelerate speed by accelerationRate per second
            Accelerate();

			// apply speed as force to rigidbody
			ApplyForce();
		}
		else
        {
			Decelerate();

			rig.velocity = Vector2.Lerp(rig.velocity, Vector2.zero, decelerationRate * Time.deltaTime);
		}

        DisTraveledScore();
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
            localPlayer.AddPlayerScore(10);
        }
    }

    void ApplyForce()
	{
		//rig.AddForce(transform.up.normalized * currentSpeed, ForceMode2D.Force);
		rig.velocity = Vector2.Lerp(rig.velocity, transform.up * topSpeed, accelerationRate * Time.deltaTime);
	}

	void Accelerate()
	{
		currentSpeed = Mathf.Lerp(currentSpeed, topSpeed, accelerationRate * Time.deltaTime);
	}

	void Decelerate()
	{
		currentSpeed = Mathf.Lerp(currentSpeed, 0, decelerationRate * Time.deltaTime);
	}

	void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.tag == "map")
        {
			RaceManager.EndRace();
        }
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

        HandleTilt(Input.acceleration.x);
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
        //localPlayer.playerInfo.itemCount
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
    }
    public void GetUpShield()
    {
        ShowPlayerItem("Shield");
    }
#endregion
}
