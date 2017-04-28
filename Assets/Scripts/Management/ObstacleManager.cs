using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameData;

public struct ObstacleGenerateInfo
{    
    public float radius;            //generation range
    public float createTime;
    public int rndRange;            //obstacle generation count
    public Vector2 checkPoint;      //obstacle generation point    
}

public struct ObstacleInfo
{
    public OBSTACLETYPE name;           //obstacle type: Asteroid - 1, Mine - 2
    public ASTEROIDTYPE type;           //asteroid type: small, medium, big
    public string strFullPath;          //obstacle prefab full paths
    public List<int> fragmentationIDs;  //when explode the big asteroids, are fragmentations ids.
    public int health;                  //obstacle's health
    public int point;                   //player point(score)
    public int damage;                  //It's obstacle's damage amount inflict to player.
    public string boundary;             //Asteroid's boundary
    public float range;                 //it's explosion range of mine
}

//public struct PowerUpItemsInfo
//{
//    public string name;
//    public string type;
//    public string strFullPath;
//}

public enum ASTEROIDTYPE
{
    None = 0,
    Small = 1,
    Medium = 2,
    Big = 3,
}

public enum OBSTACLETYPE
{
    None = 0,
    Asteroid = 1,
    Mine = 2,
}

public class ObstacleManager : MonoBehaviour {
    
    private Transform player;

    /// obstacle all of infos
    private int obstacleDataLength;
    private List<int> m_obstacleDataIdList = new List<int>();

    ObstacleGenerateInfo obstacleGenerateInfo;
    ObstacleInfo obstacleInfo;

    //PowerUpItemsInfo powerUpInfo;
    private int powerUpDataLength;
    private List<int> m_powerupDataIdList = new List<int>();

    public float radius = 30f;
    public int createObtacles = 10;
    public float createDealyTime = 0f;
    public float playerFwdDis = 100f;
    public LayerMask obstacles;
        
    float deltaTime = 0;   

    void OnEnable()
    {
        RaceManager.OnInitObstacles += Init;        
    }

    void OnDisable()
    {
        RaceManager.OnInitObstacles -= Init;
    }

    /// <summary>
    /// initialize obstacles
    /// </summary>
    private void Init()
    {
        player = GameObject.Find("DefenseShip").transform;
        DestroyObstacles();
        obstacleGenerateInfo.radius = radius;
        obstacleGenerateInfo.createTime = createDealyTime;
        obstacleGenerateInfo.rndRange = createObtacles;
    }

	// Use this for initialization
	void Start () {
        InitObstacleInfoData();
        InitPowerUpsData();
        Init();
    }

    /// <summary>
    /// read "GameData - Obstacles" xml file
    /// </summary>
    void InitObstacleInfoData()
    {        
        obstacleDataLength = ObstacleData.dataMap.Count;
        foreach(var key in ObstacleData.dataMap.Keys)
        {
            m_obstacleDataIdList.Add(key);
        }        
    }

    void InitPowerUpsData()
    {
        powerUpDataLength = PowerUpsData.dataMap.Count;
        foreach (var key in PowerUpsData.dataMap.Keys)
        {
            m_powerupDataIdList.Add(key);
        }
    }
	// Update is called once per frame
	void Update () {
        ////check obstacle creat tiem
        if (!CheckCreateTime())
            return;

        Vector2 fwd = player.up * playerFwdDis + player.position;
        Vector2 checkPoint = new Vector2(fwd.x, fwd.y);
        ////check obstacles are at player forword postion
        if (!CheckObstacleState(checkPoint))
            return;

        CreateObstacle();
    }

    /// <summary>
    /// create obstacles
    /// </summary>
    private void CreateObstacle()
    {
        int count = Random.Range(1, obstacleGenerateInfo.rndRange);        

        for (int i = 0; i < count; i++)
        {
            int rnd = Random.Range(0, 100);
            if (rnd < 3)
            {
                CreatePickUpItems();
            }
            else
            {
                CreateAsteroid();
            }
        }
    }
    
    void CreatePickUpItems()
    {
        int idx = Random.Range(0, powerUpDataLength - 1);
        PowerUpsData tmpData = PowerUpsData.dataMap[m_powerupDataIdList[idx]];

        GameObject prefabs = (GameObject)Resources.Load(tmpData.stringfullpath);

        float ang = Random.value * 360;
        float x = obstacleGenerateInfo.checkPoint.x + obstacleGenerateInfo.radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        float y = obstacleGenerateInfo.checkPoint.y + obstacleGenerateInfo.radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        Vector3 pos = new Vector3(x, y, 0);

        GameObject tmpItem = Instantiate(prefabs, pos, Quaternion.identity, transform) as GameObject;
        tmpItem.name = prefabs.name;
    }

    void CreateAsteroid()
    {
        int idx = Random.Range(0, obstacleDataLength - 1);
        InitObstacleInfo(idx);

        GameObject prefabs = (GameObject)Resources.Load(obstacleInfo.strFullPath);

        float ang = Random.value * 360;
        float x = obstacleGenerateInfo.checkPoint.x + obstacleGenerateInfo.radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        float y = obstacleGenerateInfo.checkPoint.y + obstacleGenerateInfo.radius * Mathf.Cos(ang * Mathf.Deg2Rad);

        float rotZ = Random.Range(-180, 180);

        Vector3 pos = new Vector3(x, y, 0);
        Vector3 rot = new Vector3(0, 0, rotZ);
        GameObject asteroid = Instantiate(prefabs, pos, Quaternion.Euler(rot), transform) as GameObject;
        asteroid.GetComponent<ObstacleStats>().InitInfo(obstacleInfo);
    }
    /// <summary>
    /// initialize obstacle info
    /// </summary>
    /// <param name="idx"></param>
    private void InitObstacleInfo(int idx)
    {
        ObstacleData tmp = ObstacleData.dataMap[m_obstacleDataIdList[idx]];
        switch(tmp.name)
        {
            case "Asteroid":
                obstacleInfo.name = OBSTACLETYPE.Asteroid;
                break;
            case "Mine":
                obstacleInfo.name = OBSTACLETYPE.Mine;
                break;
        }

        switch(tmp.type)
        {
            case "small":
                obstacleInfo.type = ASTEROIDTYPE.Small;
                break;
            case "medium":
                obstacleInfo.type = ASTEROIDTYPE.Medium;
                break;
            case "big":
                obstacleInfo.type = ASTEROIDTYPE.Big;
                break;
        }
        obstacleInfo.strFullPath = tmp.stringfullpath;
        obstacleInfo.fragmentationIDs = tmp.fragmentationids;        
        obstacleInfo.health = tmp.health;
        obstacleInfo.point = tmp.point;
        obstacleInfo.damage = tmp.damage;
        obstacleInfo.range = tmp.range;
        obstacleInfo.boundary = tmp.boundary;
    }

    /// <summary>
    /// When player failed or start game, initialize all of obstacles
    /// </summary>
    private void DestroyObstacles()
    {
        Component[] rigidbodys;

        rigidbodys = GetComponentsInChildren(typeof(Rigidbody2D));
        foreach (Rigidbody2D rb in rigidbodys)
        {
            Destroy(rb.gameObject);
        }
    }

    /// <summary>
    /// create obstacles delay time
    /// </summary>
    private bool CheckCreateTime()
    {
        deltaTime += Time.fixedDeltaTime;
        if (deltaTime < obstacleGenerateInfo.createTime)
            return false;

        deltaTime = 0;
        return true;
    }

    /// <summary>
    /// check obstacles at player forward.
    /// </summary>
    private bool CheckObstacleState(Vector2 pos)
    {
        obstacleGenerateInfo.checkPoint = pos;

        Collider2D collider = Physics2D.OverlapCircle(pos, obstacleGenerateInfo.radius, obstacles);

        if (collider) return false;
                
        return true;
    }
}
