using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{

    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] List<string> objectives = new List<string>();

        public string GetTitle()
        {
            return this.name;
        }

        public int GetNumberOfObjectives()
        {
            return objectives.Count;
        }

        public List<string> GetObjectives()
        {
            return objectives;
        }

        public bool HasObjective(string objective)
        {
            return objectives.Contains(objective);
        }

        public static Quest GetByName(string questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if(quest.name == questName)
                {
                    return quest;
                }
            }
            return null;
        }
        
        


    }
}