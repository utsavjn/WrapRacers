using System;
using System.Collections.Generic;

namespace GameData
{
    public class LevelData : GameData<LevelData>
    {
        public string name { get; protected set; }
        public float shipbasespeed { get; protected set; }
        public float shiptopspeed { get; protected set; }

        public LevelData()
        {

        }

        static public readonly string fileName = "xml/GameData - Level";
    }



}
