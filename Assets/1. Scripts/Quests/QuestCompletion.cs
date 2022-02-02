using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] Quest quest;
        [SerializeField] string completedOjective;


        QuestList questList;
        
        private void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }


        public void CompleteOjective()
        {
            

            List<string> objectives = quest.GetObjectives();
            foreach (string objective in objectives)
            {
                if (completedOjective == objective && questList.HasQuest(quest))
                {
                    questList.CompleteObjective(quest, completedOjective);
                    
                }
            }
        }
    }
}