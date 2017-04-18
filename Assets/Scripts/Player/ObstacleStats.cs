using UnityEngine;
using System.Collections;
using Xengine;
using WarpRacers;

public class ObstacleStats : MonoBehaviour, Stats {

    ObstacleInfo obstacleInfo;
    PlayerController localPlayer;
    
    
    public void InitInfo(ObstacleInfo tmpInfo)
    {
        localPlayer = FindObjectOfType<PlayerController>();        
        obstacleInfo = tmpInfo;
    }

    /// <summary>
    /// destroy object, if hit ship gun.
    /// </summary>
    /// <param name="damage"></param>
    public void Hit(int damage) 
	{
        localPlayer.AddPlayerScore(obstacleInfo.point);
        DestroyObstacle();
	}

    public bool ConflectObstacle()
    {

        return true;
    }

    /// <summary>
    /// conflect ship
    /// </summary>
	public void Die() 
	{        
        DoConflectEffect();
    }
    
    /// <summary>
    /// After conflect player ship
    /// </summary>
    void DoConflectEffect()
    {
        switch (obstacleInfo.type)
        {
            case ASTEROIDTYPE.Small:
                if(CheckLevel() == "Level 1")
                {
                    //decrease player speed
                    localPlayer.sc.DecrementSpeed(20);
                    //rigidbody - true
                    GetComponent<Rigidbody2D>().isKinematic = true;
                }
                else if (CheckLevel() == "Level 2")
                {
                    //decrease player speed
                    localPlayer.sc.DecrementSpeed(20);
                    //rigidbody - true
                    GetComponent<Rigidbody2D>().isKinematic = true;
                }
                break;
            case ASTEROIDTYPE.Medium:

                break;
            case ASTEROIDTYPE.Big:
                localPlayer.sc.GetComponent<Stats>().Die();     //die player
                DestroyObstacle();                              //destroy obstacle
                break;
        }
    }

    void DestroyObstacle()
    {
        //explosion effect
        ObjectPooler.GetPooledObject(Prefabs.EXPLOSION_SMALL, transform.position, Quaternion.identity).SetActive(true);
        Destroy(gameObject);
    }

    string CheckLevel()
    {
        string level = "Level 1";
        return level;
    }
}
