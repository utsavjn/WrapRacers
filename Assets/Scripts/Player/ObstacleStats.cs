using UnityEngine;
using System.Collections;
using Xengine;
using WarpRacers;

public class ObstacleStats : MonoBehaviour, Stats {

    ObstacleInfo obstacleInfo;
    PlayerController localPlayer;

    public void InitInfo()
    {
        localPlayer = GameObject.Find("Player").GetComponent<PlayerController>();
        obstacleInfo.asteroidType = ASTEROIDTYPE.Small;
        obstacleInfo.point = 10;
    }

    public void Hit(int damage) 
	{
		Die();
	}

	public void Die() 
	{
        localPlayer.PlayerScore += obstacleInfo.point;
        ObjectPooler.GetPooledObject(Prefabs.EXPLOSION_SMALL, transform.position, Quaternion.identity).SetActive(true);
        Destroy(gameObject);
    }
}
