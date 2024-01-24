using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JSONSerializable
{
    public static readonly string levelsJSONFileName = "ProgressData";
    public static readonly string saveFilePath = Application.persistentDataPath+"/SaveFile.dat";

    [Serializable]
    public class Levels
    {
        public Level[] levels;

        public Levels(Level[] levels)
        { this.levels = levels; }

        [Serializable]
        public class Level
        {
            public string name;
            public bool isComplete;
            public Level(string name, bool isComplete)
            {
                this.name = name;
                this.isComplete = isComplete;
            }
        }
    }
}
