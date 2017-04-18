using UnityEngine;
using System.Collections.Generic;

namespace GameData
{
    public class PowerUpsData : GameData<PowerUpsData>
    {
        public string name { get; protected set; }
        public string type { get; protected set; }
        public string stringfullpath { get; protected set; }

        public PowerUpsData()
        {

        }

        static public readonly string fileName = "xml/GameData - Items";
    }
}