using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] Quest[] tempQuests;
        [SerializeField] QuestItemUI questPrefab;
        QuestList questList;

        private void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.onQuestUpdated += UpdateUI;

            UpdateUI();
        }

        private void ClearOldQuests()
        {
            QuestItemUI[] childern = GetComponentsInChildren<QuestItemUI>();
            foreach (QuestItemUI child in childern)
            {
                Destroy(child.gameObject);
            }
        }

        private void UpdateUI()
        {
            ClearOldQuests();
            foreach (QuestStatus status in questList.GetStatuses())
            {
                QuestItemUI newQuest = Instantiate(questPrefab, this.transform);
                newQuest.Setup(status);
            }

        }
    }
}