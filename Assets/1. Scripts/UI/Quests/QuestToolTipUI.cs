using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestToolTipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject questTaskPrefab;
        [SerializeField] GameObject questTaskIncompletePrefab;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            ClearOldTasks();
            title.text = quest.GetTitle();
            foreach (string task in quest.GetObjectives())
            {
                Debug.Log(task);
                GameObject prefab = questTaskIncompletePrefab;
                if (status.IsObjectiveComplete(task))
                {
                    prefab = questTaskPrefab;
                }
                GameObject newTask = Instantiate(prefab, objectiveContainer);
                newTask.GetComponentInChildren<TextMeshProUGUI>().text = task;
            }


        }

        
        private void ClearOldTasks()
        {
            QuestsTask[] oldTasks = objectiveContainer.GetComponentsInChildren<QuestsTask>();
            foreach (QuestsTask task in oldTasks)
            {
                Destroy(task.gameObject);
            }
        }
    }
}