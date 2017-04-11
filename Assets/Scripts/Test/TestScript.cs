using UnityEngine;
using System.Collections;
using GameData;

public class TestScript : MonoBehaviour {

    public PlayerData m_PlayerData;

    // Use this for initialization
    void Start () {
        InitPlayerInfo();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void InitPlayerInfo()
    {
        m_PlayerData = PlayerData.dataMap[1];

        Debug.Log((m_PlayerData.criticalHealth * m_PlayerData.playerHealth) / 100);
        Debug.Log(m_PlayerData.actionPower);
        Debug.Log(m_PlayerData.lifeTime);
        Debug.Log(m_PlayerData.criticalTimes);
        
    }
}
