using System;
using System.Collections.Generic;

namespace GameData
{
	public class SoundData : GameData<SoundData>
	{
		public string name { get; protected set; }
		public string stringfullpath { get; protected set; }
		public float volume { get; protected set; }

		public SoundData()
		{

		}

		static public readonly string fileName = "xml/GameData - Sounds";
	}
}
