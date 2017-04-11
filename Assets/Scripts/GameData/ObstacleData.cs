using System;
using System.Collections.Generic;

namespace GameData
{
    public class ObstacleData : GameData<ObstacleData>
    {
        public string name { get; protected set; }
        public string type { get; protected set; }
        public string stringfullpath { get; protected set; }
        public List<int> fragmentationids { get; protected set; }
        public int health { get; protected set; }
        public int point { get; protected set; }
        public int damage { get; protected set; }
        public string boundary { get; protected set; }
        public float range { get; protected set; }

        public ObstacleData()
        {

        }

        static public readonly string fileName = "xml/GameData - Obstacles";
    }

    

}
