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

	const float LASER_DISTANCE = 27;

	#region Movement Control Attributes

	public float accelerationRate = 2.5f;

	public float decelerationRate = 4f;

    public float baseTopSpeed = 5;
	public float currentSpeed = 0;
	public float topSpeed = 5;

    #endregion
    public void IncrementSpeed(float increment)
    {
        topSpeed += increment;
    }

    void Start()
    {
        shipAnimator = GetComponent<ShipAnimator>();
		shipStats = GetComponent<PlayerStats>();
		rig = GetComponent<Rigidbody2D>();
        Camera.main.GetComponent<CameraController>().Target = transform;
    }

	public void StopFiring()
	{
		isFiring = false;
	}

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
			print("hit " + hit.transform.name);
		}
	}

	public void Move()
    {
		if (thrustActive || Input.GetAxis("Vertical") > 0)
		{
			// accelerate speed by accelerationRate per second
			Accelerate();

			// apply speed as force to rigidbody
			ApplyForce();
		}
		else {
			Decelerate();

			rig.velocity = Vector2.Lerp(rig.velocity, Vector2.zero, decelerationRate * Time.deltaTime);
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

    #region Powerup Interface Functions

    public void ActivateShield()
    {
      shipStats.ActivateShield(1);
    }

    public void DeactivateShield()
    {
      shipStats.DeactivateShield();
    }

    #endregion
}
