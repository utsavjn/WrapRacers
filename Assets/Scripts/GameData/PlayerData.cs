using System;
using System.Collections.Generic;

namespace GameData
{
	public class PlayerData : GameData<PlayerData>
	{		
		public int playerHealth { get; protected set; }
		public float playerMoveDistance { get; protected set; }
		public int criticalHealth { get; protected set; }
        public int actionPower { get; protected set; }
		public int lifeTime { get; protected set; }                                 
		public int attackPower { get; protected set; }                              
        public int defensePower { get; protected set; }                             
		public int criticalTimes { get; protected set; }
		public int criticalProbability { get; protected set; }
		public int increaseCriticalProbability { get; protected set; }
		public int attackHead { get; protected set; }
		public int attackBody { get; protected set; }
		public int attackLeg { get; protected set; }
		public int skillCondition { get; protected set; }
		public List<int> bloodSplatter { get; protected set; }                     
        public List<int> skillBloodSplatter { get; protected set; }                

        // Add By HKC
        public int minRoomCount { get; protected set; }
		public int maxRoomCount { get; protected set; }
		public int maxBoardWeight { get; protected set; }

		public PlayerData ()
		{			
		}

		static public readonly string fileName = "xml/GameData - Player";
	}
}
