using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable
    {
        List<QuestStatus> statuses = new List<QuestStatus>();

        public event Action onQuestUpdated;


        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest))
            {
                return;
            }
            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);
            if (onQuestUpdated != null)
            {
                onQuestUpdated();
            }

        }

        public bool HasQuest(Quest quest)
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest && quest.HasObjective(objective))
                {
                    status.AddCompletedOjbective(objective);
                    onQuestUpdated();
                }
            }
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (QuestStatus status in statuses)
            {
                state.Add(status.CaptureState());
            }
            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if (stateList == null)
            {
                return;
            }

            statuses.Clear();

            foreach (object objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));
            }
        }
    }
}
