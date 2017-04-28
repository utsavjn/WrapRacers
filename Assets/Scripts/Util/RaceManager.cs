using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Race state
/// </summary>
public enum RACESTAT
{
	Ready = 0,
	Start = 1,
	Finish =2,
	End = 3,
}

public class RaceManager : MonoBehaviour {

    public static RaceManager _instance;

    public static string strCountdown = "Countdown";

    private RACESTAT raceStat = RACESTAT.Ready;

    public Text lapText;

    PlayerShipData shipData;

    public GameObject speedShipPrefab;
    public GameObject defenseShipPrefab;
    public GameObject specialShipPrefab;

//    public GameObject ship;

    //bool firstLap = true;

//    public Transform rightCenter;
//    public Transform leftCenter;

    public delegate void RaceAction();
    public static event RaceAction OnRaceEnd;
    public static event RaceAction OnRaceReady;
    public static event RaceAction OnRaceFinish;
    public static event RaceAction OnRaceStart;

    /// <summary>
    /// OnInitobstacles delegate event , when restart game , all of the obstacels is destroy and initilaize obstacles info.
    /// </summary>
    public delegate void ObstaclesAction();
    public static event ObstaclesAction OnInitObstacles;

    public delegate void PlayersAction();
    public static event PlayersAction OnDeadPlayer;
    public static event PlayersAction OnInitPlayer;
    public static event PlayersAction OnIncPlayerSpeed;

    int laps = 3;
    public int CurrentLap {
        get { return currentLap; }
        set {
            if (value > laps) currentLap = 1;
            else currentLap = value;
        }
    }

    int currentLap = 1;

    float speedIncrement = 5f;

    public PlayerController localPlayer;

    private Dictionary<string, GameObject> shipPrefabs = new Dictionary<string, GameObject>();

    public bool goingBackward;

    public UIController uc;

    bool hitLeftMidway = false;
    bool hitRightMidway = false;

    #region Lap Progress Determination

    private float largestAngleRight = 1;

    private float largestAngleLeft = 1;

    private float currentAngle = 1;

    public float LapProgressRight
    {
        get { return largestAngleRight / CIRCUMFERENCE; }
    }

    public float LapProgressLeft
    {
        get { return largestAngleLeft / CIRCUMFERENCE; }
    }

    public const int CIRCUMFERENCE = 359;

    #endregion

    #region Monobehaviour Functions

    void Awake()
    {
        if(_instance == null)
            _instance = this;        
    }

    void OnEnable()
    {
		OnRaceEnd += OnRaceEndStat;
        OnRaceStart += OnRaceStartStat;
        OnRaceReady += OnRaceReadyStat;
        OnRaceFinish += OnRaceFinishStat;
    }

    void OnDisable()
    {
    }

    #endregion	

    #region Race state
    // ends the race as well as runs any listeners for the race end
    public static void EndRace()
    {
        if (OnRaceEnd != null) OnRaceEnd();
    }

    public static void ResetRace()
    {
        if (OnRaceStart != null) OnRaceStart();
    }

    public static void FinishRace()
    {
        if (OnRaceFinish != null) OnRaceFinish();
    }

    public void ReadyRace()
    {
        if (OnRaceReady != null) OnRaceReady();
    }

    private void OnRaceEndStat()
    {
        raceStat = RACESTAT.End;

        OnInitObstacles();
        OnDeadPlayer();
        uc.ShowGameOver();
        StopGameAudio();
    }

    private void OnRaceReadyStat()
    {
        raceStat = RACESTAT.Ready;
    }

    private void OnRaceStartStat()
    {
        raceStat = RACESTAT.Start;

        uc.HideGameOver();
        uc.ShowCoutDown();
        InitilaizeGame();
        //ResetPlayerOrientation();
        //EnablePlayerControls();
        //InitPlayerInfo();
        InitRaceObstacles();
        PlayGameAudio();
    }

    private void OnRaceFinishStat()
    {
        raceStat = RACESTAT.Finish;
        Debug.Log("raceStat = RACESTAT.Finish;" + raceStat);
    }
    #endregion

    private void InitRaceObstacles()
    {
        OnInitObstacles();
    }

    private void ReadyRacePlayer()
    {
        OnInitPlayer();
    }

    private void StartRacePlayer()
    {

    }

  //  public void ResetPlayerOrientation()
  //  {
		//localPlayer.sc.transform.position = Vector3.up;
  //      localPlayer.sc.transform.rotation = Quaternion.Euler(0, 0, 0);
  //  }

  //  public void InitPlayerInfo()
  //  {
  //      localPlayer.InitPlayerItem();
  //  }    
    	
  //  void DisablePlayerControls()
  //  {
  //      localPlayer.controlsEnabled = false;
  //  }

  //  void EnablePlayerControls()
  //  {
  //      localPlayer.controlsEnabled = true;
  //  }

    public void PlayGameAudio()
    {
        SoundManager.PlayMusic(3001, SoundManager.PlayMusicMode.Repeat);
    }

    public void StopGameAudio()
	{
        SoundManager.StopBackgroundMusic();
        //play the other backsound
        //SoundManager.PlayMusic(3001, SoundManager.PlayMusicMode.Repeat);
    } 

    //   public int ShipToPathOriginAngle(Transform ship, Vector2 origin)
    //{
    //	return AngleFromOrigin(ship.position, origin);
    //}

    //public void SetPlayerShip(Ship ship)
    //   {
    //       this.ship = Instantiate(shipPrefabs[ship.shipName]);
    //   }

    //int AngleFromOrigin(Vector2 position, Vector2 origin)
    //{
    //	float width = position.x - origin.x;
    //	float height = origin.y - position.y;

    //	return (int)(Mathf.Rad2Deg * Mathf.Atan2(height, width) + 180);
    //}

    //bool OnRightSide(Vector2 position)
    //{
    //	return position.x >= 0;
    //}

    // Use this for initialization
    void Start ()
    {
        shipPrefabs.Add("ShipName_01", speedShipPrefab);
        shipPrefabs.Add("ShipName_02", defenseShipPrefab);
        shipPrefabs.Add("ShipName_03", specialShipPrefab);

		InitilaizeGame ();
	}

	void InitilaizeGame()
	{
        localPlayer = GameObject.Find(Common.strPlayer).GetComponent<PlayerController>();
        uc = GetComponent<UIController>();
        ReadyRace();
    }

	void StartGame()
	{		
		//ResetPlayerOrientation();
		//EnablePlayerControls();
		//InitPlayerInfo();
		InitRaceObstacles();
		PlayGameAudio();
	}

	// less than 180
	// largest angle > 180
	// don't update?	

//	void UpdatePlayerAngleToTrack(ref float largestAngle)
//	{
//       
//
//		// midway check
//		if (!hitRightMidway)
//		{
//			if (currentAngle <= 190 && currentAngle >= 170)
//			{
//				hitRightMidway = true;
//			}
//		}
//		else {
//			if (!hitLeftMidway)
//			{
//				if (currentAngle >= 350 && currentAngle <= 360 ||
//				    currentAngle >= 0 && currentAngle <= 10)
//				{
//					hitLeftMidway = true;
//				}
//			}
//
//			// hit left and right, time to win
//			if (hitLeftMidway)
//			{
//
//				print(hitLeftMidway && hitRightMidway);
//
//				//if (CurrentLap == laps)
//				//{
//				//	WinRace();
//				//}
//				//else {
//                    CycleLap();
//                //}               
//
//				hitRightMidway = false;
//				hitLeftMidway = false;
//			}
//		}
//
//		if (currentAngle > largestAngle)
//		{
//			largestAngle = currentAngle;
//		}
//
//		// switch sides of circle when hitting halfway through
//		if (currentAngle > CIRCUMFERENCE / 2)
//		{
//			largestAngle = currentAngle;
//		}
//	}

    private void CycleLap()
    {
        CurrentLap++;
        if (CurrentLap < laps)
            localPlayer.SendMessage("IncrementSpeed", speedIncrement);

        lapText.text = "LAP " + CurrentLap + " / " + laps;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            CycleLap();
        }

//        currentAngle = ShipToPathOriginAngle(ship.transform, rightCenter.position);
//
//		if (OnRightSide(ship.transform.position))
//		{
//			UpdatePlayerAngleToTrack(ref largestAngleRight);
//		}
//		else
//		{
//			UpdatePlayerAngleToTrack(ref largestAngleLeft);
//		}

		//print(currentLap);
    }

//	bool OnSameSideOfCircle(float angle1, float angle2) 
//	{
//		bool under180 = (angle1 >= 0 && angle1 < 180) && (angle2 >= 0 && angle2 < 180);
//
//		bool over180 = (angle1 >= 180 && angle1 <= 360) && (angle2 >= 180 && angle2 <= 360);
//
//		return under180 || over180;
//	}
}

public struct RaceData
{
    public float points;
}
