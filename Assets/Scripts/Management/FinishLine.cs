using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<ShipController>())
            return;

        FinishRace();
    }

    void FinishRace()
    {
        //RaceManager._instance.();
    }
}
