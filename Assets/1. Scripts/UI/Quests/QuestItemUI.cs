using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;
using TMPro;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI progress;
        QuestStatus status;

        public void Setup(QuestStatus status)
        {
            title.text = status.GetQuest().GetTitle();
            progress.text = (status.GetCompletedCount() + "/" + status.GetQuest().GetNumberOfObjectives());
            this.status = status;

        }

        public QuestStatus GetQuestStatus()
        {
            return status;
        }
    }
}
