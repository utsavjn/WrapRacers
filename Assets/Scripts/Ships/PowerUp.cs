using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter2D (Collider2D power) {
        if(power.transform.tag == "PowerUp")
            power.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
