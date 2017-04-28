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
				DoAffectObstacal ();
                if(CheckLevel() == "Level 1")
                {
                    //decrease player speed
                    localPlayer.sc.DecrementSpeed(20);
                }
                else if (CheckLevel() == "Level 2")
                {
                    //decrease player speed
                    localPlayer.sc.DecrementSpeed(20);
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

	void DoAffectObstacal()
	{
		Rigidbody2D rb = GetComponent<Rigidbody2D> ();
		Vector2 own = new Vector2 (this.transform.position.x, this.transform.position.y);
		Vector2 player = new Vector2 (localPlayer.sc.transform.position.x, localPlayer.sc.transform.position.y);

		Vector2 dis = own - player;
		rb.AddForce (dis.normalized * localPlayer.sc.currentSpeed, ForceMode2D.Impulse);
	}

    void DestroyObstacle()
    {
		string strPath;
		switch (obstacleInfo.type) {
		case ASTEROIDTYPE.Small:
			strPath = Prefabs.EXPLOSION_SMALL;
			break;
		case ASTEROIDTYPE.Medium:
			strPath = Prefabs.EXPLOSION_MEDIUM;
			break;
		case ASTEROIDTYPE.Big:
			strPath = Prefabs.EXPLOSION_BIG;
			break;
		default:
			strPath = Prefabs.EXPLOSION_SMALL;
			break;
		}
        //explosion effect
		ObjectPooler.GetPooledObject(strPath, transform.position, Quaternion.identity).SetActive(true);
        Destroy(gameObject);
    }

    string CheckLevel()
    {
        string level = "Level 1";
        return level;
    }
}
