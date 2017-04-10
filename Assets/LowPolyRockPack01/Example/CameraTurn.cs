using UnityEngine;
using System.Collections;

public class CameraTurn : MonoBehaviour {

	public Transform lookAt;

	// Update is called once per frame
	void Update () {

		transform.RotateAround( lookAt.position, Vector3.up, 15*Time.deltaTime);
	}
}
