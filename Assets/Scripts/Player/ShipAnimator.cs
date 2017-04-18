using UnityEngine;
using Xengine.UnityExtensions;
using System.Collections;
using Xengine;

[RequireComponent(typeof(Animator))]
public class ShipAnimator : MonoBehaviour {

	// Mecanim Animation parameter names
    const string TILT_LEFT_PARAM_NAME = "tiltleft";
    const string TILT_RIGHT_PARAM_NAME = "tiltright";
    const string TILT_PARAM_NAME = "tilt";
	const string FIRE_PARAM_NAME = "fire";
    const string DEFAULT_PARAM_NAME = "default";

	const string LASER_PREFAB_PATH = "Prefabs/Projectile/Laser";

	const float laserOffsetY = 0;

	Animator animator;
	public Transform[] weapons;
	GameObject shield;

	void Start()
	{
		animator = GetComponent<Animator>();
	}

	private void SpawnLaser(Vector3 position, Quaternion rotation, Transform parent)
	{
		GameObject laser = ObjectPooler.GetPooledObject(LASER_PREFAB_PATH, position, rotation, parent);
		Animator a = laser.GetComponent<Animator>();
		laser.SetActive(true);

		a.Animate(FIRE_PARAM_NAME);
	}

	public void Fire()
	{
		foreach (Transform t in weapons)
		{
			SpawnLaser(new Vector3(t.position.x,
			                       t.position.y + laserOffsetY,
			                       t.position.z), 
			           Quaternion.Euler(t.rotation.eulerAngles + new Vector3(0, 0, 90)),
			          t);
		}
	}

    public void AnimateTilt(float tilt)
    {
        animator.SetFloat(TILT_PARAM_NAME, tilt);
    }

	public void TiltRight()
	{
		animator.Animate(TILT_RIGHT_PARAM_NAME);
	}

	public void TiltLeft()
	{
		animator.Animate(TILT_LEFT_PARAM_NAME);
	}

	public void SetShieldState(bool state)
	{
		shield.SetActive(state);	
	}

}
