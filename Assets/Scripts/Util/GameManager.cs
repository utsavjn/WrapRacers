using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager _instance = null;

	public AudioSource m_musicAud;
	public AudioSource m_soundAud;
	void Awake()
	{
		if (_instance == null)
			_instance = this;
		DontDestroyOnLoad (this);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
