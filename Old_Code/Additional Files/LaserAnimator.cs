using UnityEngine;
using System.Collections;

public class LaserAnimator : MonoBehaviour {

	Animator a;

	void Awake()
	{
		a = GetComponent<Animator>();
	}

	public void Disable()
	{
		a.SetBool("fire", false);
		gameObject.SetActive(false);
	}
}
