using UnityEngine;
using System.Collections;
using Xengine;
using WarpRacers;

public class PlayerStats : MonoBehaviour, Stats {

	public RaceManager rm;

	public GameObject shieldObject;

	public int health = 1;
	public int shield = 0;

	public void Hit(int damage)
	{
		if (shield > 0) {
			shield--;

			if (shield <= 0) {
				DeactivateShield();
			}

			return;
		}

		Die();
	}

	public void Die()
	{
        ObjectPooler.GetPooledObject(Prefabs.EXPLOSION_SMALL, transform.position, Quaternion.identity).SetActive(true);
        RaceManager.EndRace();
        //gameObject.SetActive(false);
    }

	public void ActivateShield(int integrity)
	{
		shield = integrity;
		shieldObject.SetActive(true);
	}

	public void DeactivateShield()
	{
		shield = 0;
		shieldObject.SetActive(false);
	}
}
