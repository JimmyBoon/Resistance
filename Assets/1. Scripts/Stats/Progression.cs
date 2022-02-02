
using System;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {

        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharaterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharaterClass charaterClass, int level)
        {

            BuildLookUp();

            float[] levels =  lookupTable[charaterClass][stat]; 

            if (levels.Length < level) 
            {
                return 0;
            }
            
            return levels[level - 1]; // would be without levels check: return lookupTable[charaterClass][stat][level] 
        }

        private void BuildLookUp()
        {
            if(lookupTable != null)
            { return; }

            lookupTable = new Dictionary<CharaterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();
                
                foreach (ProgressionStat progressionStat in progressionClass.stat)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }


                lookupTable[progressionClass.charaterClassName] = statLookupTable;

            }


        }

        public int GetLevels(Stat stat, CharaterClass charaterClass)
        {
            BuildLookUp();

            float[] levels = lookupTable[charaterClass][stat];
            return levels.Length;

        }

        [Serializable]
        class ProgressionCharacterClass
        {
            public CharaterClass charaterClassName;
            public ProgressionStat[] stat;
        }

        [Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }




    }
}