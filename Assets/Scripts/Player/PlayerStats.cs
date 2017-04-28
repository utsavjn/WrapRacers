using UnityEngine;
using System.Collections;
using Xengine;
using WarpRacers;

public class PlayerStats : MonoBehaviour, Stats {

	private static string strShieldObj = "Shield";

	public int health = 1;
	public int shield = 0;

	private GameObject shieldObject;

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	void Initialize()
	{
		shieldObject = this.transform.FindChild (strShieldObj).gameObject;
		shieldObject.GetComponent<SpriteRenderer> ().enabled = false;
	}

	void Start()
	{
		Initialize ();
	}

	public void Hit(int damage)
	{
		if (shield > 0) {
			shield--;

			if (shield <= 0) {
				DeactivateShield();
			}

			return;
		}

		//Die();
	}

	public void Die()
	{        
        ObjectPooler.GetPooledObject(Prefabs.EXPLOSION_SMALL, transform.position, Quaternion.identity).SetActive(true);
        RaceManager._instance.EndRace();
    }

	public void ActivateShield(int integrity)
	{
		shield = integrity;
		shieldObject.GetComponent<SpriteRenderer> ().enabled = true;
	}

	public void DeactivateShield()
	{
		shield = 0;
		shieldObject.GetComponent<SpriteRenderer> ().enabled = false;
	}
}