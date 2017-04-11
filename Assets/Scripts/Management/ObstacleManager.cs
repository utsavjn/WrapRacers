using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameData;

struct ObstacleInfo
{
    public string[] strObstaclesFullPathList;
    public float radius;            //generation range
    public float createTime;
    public int rndRange;            //obstacle generation count
    public Vector2 checkPoint;      //obstacle generation point
    public int point;               //player point(score)
    public OBSTACLETYPE obstacleType;
    public ASTEROIDTYPE asteroidType;
}

public enum ASTEROIDTYPE
{
    None = 0,
    Small = 1,
    Normal = 2,
    Big = 3,
}

public enum OBSTACLETYPE
{
    None = 0,
    Asteroid = 1,
    Mine = 2,
}
public class ObstacleManager : MonoBehaviour {
    
    public string str_obstaclePath = "Prefabs/Obstacles/Asteroid1";
    
    private Transform player;

    /// <summary>
    /// obstacle all of infos
    /// </summary>
    public ObstacleData m_obstacleData;

    private int obstacleDataLength;
    private List<int> m_obstacleDataIdList = new List<int>();

    ObstacleInfo obstacleInfo;
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
        obstacleInfo.radius = radius;
        obstacleInfo.createTime = createDealyTime;
        obstacleInfo.rndRange = createObtacles;
    }

	// Use this for initialization
	void Start () {
        InitObstacleInfoData();
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
            Debug.Log("int dataLength = ObstacleData.dataMap.Count;" + key);
            m_obstacleDataIdList.Add(key);
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
        int count = Random.Range(1, obstacleInfo.rndRange);
        int idx = Random.Range(0, obstacleDataLength - 1);

        m_obstacleData = ObstacleData.dataMap[m_obstacleDataIdList[idx]];

        for (int i = 0; i < count; i++)
        {                  
            
            GameObject prefabs = (GameObject)Resources.Load(m_obstacleData.stringfullpath);

            float ang = Random.value * 360;
            float x = obstacleInfo.checkPoint.x + obstacleInfo.radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            float y = obstacleInfo.checkPoint.y + obstacleInfo.radius * Mathf.Cos(ang * Mathf.Deg2Rad);

            float rotZ = Random.Range(-180, 180);

            Vector3 pos = new Vector3(x, y, 0);
            Vector3 rot = new Vector3(0, 0, rotZ);
            GameObject asteroid = Instantiate(prefabs, pos, Quaternion.Euler(rot), transform) as GameObject;
            asteroid.GetComponent<ObstacleStats>().InitInfo();
        }
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
        if (deltaTime < obstacleInfo.createTime)
            return false;

        deltaTime = 0;
        return true;
    }

    /// <summary>
    /// check obstacles at player forward.
    /// </summary>
    private bool CheckObstacleState(Vector2 pos)
    {
        obstacleInfo.checkPoint = pos;

        Collider2D collider = Physics2D.OverlapCircle(pos, obstacleInfo.radius, obstacles);

        if (collider) return false;
                
        return true;
    }
}
