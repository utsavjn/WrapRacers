using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using System;
using WarpRacers;
using Xengine.CollectionExtensions;

public class PowerupController : MonoBehaviour
{
	public delegate void PowerupAction(ShipController player);

	public static Dictionary<string, Effect> powerups = new Dictionary<string, Effect>();

    //RaceManager rm;

	void Start()
	{
		//rm = GetComponent<RaceManager>();

		// Grab them by prefab name, which is the last part of the path
		powerups.Add(Prefabs.POWERUP_BOMB.Split('/').Last(), new Effect(Bomb, null));
		powerups.Add(Prefabs.POWERUP_ENERGY.Split('/').Last(), new Effect(Energy, EnergyEndAction));
		powerups.Add(Prefabs.POWERUP_HEALTH.Split('/').Last(), new Effect(Health, HealthEndAction));
		powerups.Add(Prefabs.POWERUP_MAGNET.Split('/').Last(), new Effect(Magnet, MagnetEndAction));
		powerups.Add(Prefabs.POWERUP_RANDOM.Split('/').Last(), new Effect(Random, RandomEndAction));
		powerups.Add(Prefabs.POWERUP_SHIELD.Split('/').Last(), new Effect(Shield, null));

		print(Prefabs.POWERUP_SHIELD.Split('/').Last());
	}

	#region Powerup Functions

	public static void ApplyPowerup(string name, ShipController player)
	{
        var powerupEffect = powerups.GetValue(name);        
		// add ending later
		powerupEffect.Start(player);
	}

	public void Bomb(ShipController player)
	{
        // Do bomb stuff
        player.GetUpBomb();
    }

	public void Energy(ShipController player)
	{
        // Do bomb stuff
        player.GetUpEnergy();
    }

	public void Health(ShipController player)
	{
        // Do bomb stuff
        player.GetUpHealth();
    }

	public void Magnet(ShipController player)
	{
        // Do bomb stuff
        player.GetUpMagnet();
    }

	public void Random(ShipController player)
	{
        // Do bomb stuff
        player.GetUpRamdom();
    }

	public void Shield(ShipController player)
	{
		// Do bomb stuff
		player.ActivateShield();
    }
	#endregion
	#region Powerup End Actions

	public void BombEndAction(ShipController player)
	{
		// Do bomb stuff
	}

	public void EnergyEndAction(ShipController player)
	{
		// Do bomb stuff
	}

	public void HealthEndAction(ShipController player)
	{
		// Do bomb stuff
	}

	public void MagnetEndAction(ShipController player)
	{
		// Do bomb stuff
	}

	public void RandomEndAction(ShipController player)
	{
		// Do bomb stuff
	}

	public void ShieldEndAction(ShipController player)
	{
		// Do bomb stuff
		player.DeactivateShield();
	}
	#endregion
}

public struct Effect
{
	public Effect(PowerupController.PowerupAction startAction, PowerupController.PowerupAction endAction)
	{
		this.startAction = startAction;
		this.endAction = endAction;
	}

	public void Start(ShipController player)
	{
		if (startAction != null)
		{
			startAction(player);
		}
	}

	public void End(ShipController player)
	{
		if (endAction != null)
		{
			endAction(player);
		}
	}

	public PowerupController.PowerupAction startAction;
	public PowerupController.PowerupAction endAction;
}
