using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Xengine.CollectionExtensions;
using System;
using System.Linq;

public class ShipCollisionInteraction : MonoBehaviour {

	delegate void CollisionAction(Transform collidedObject);

	Dictionary<string, CollisionAction> tagCollisionActions = new Dictionary<string, CollisionAction>();

	// Use this for initialization
	void Start () 
	{
		tagCollisionActions.Add("obstacle", HitObstacle);
		tagCollisionActions.Add("PowerUp", GrabPowerUp);
	}

    void OnCollisionEnter2D (Collision2D other) 
	{
		var func = tagCollisionActions.GetValue(other.transform.tag);

		if (func != null)
		{
			func(other.transform);
		}
    }

	void OnTriggerEnter2D(Collider2D trigger)
	{
		var func = tagCollisionActions.GetValue(trigger.transform.tag);

		if (func != null)
		{
			func(trigger.transform);
		}
	}

    /// <summary>
    /// when conflect obstacle, ...
    /// </summary>
    /// <param name="collidedObject"></param>
	void HitObstacle(Transform collidedObject)
	{
		collidedObject.transform.GetComponent<Stats>().Die();
		//GetComponent<Stats>().Hit(1);
	}
	/// <summary>
	/// Grabs the power up.
	/// </summary>
	void GrabPowerUp(Transform collidedObject)
	{
		string powerupName = collidedObject.name.Split(' ').First();

		print(powerupName);

		PowerupController.ApplyPowerup(powerupName, GetComponent<ShipController>());

        Destroy(collidedObject.gameObject);
	}
}
