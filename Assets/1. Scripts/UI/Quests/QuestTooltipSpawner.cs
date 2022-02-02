using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Utils.UI.Tooltips;
using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            return true;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus status = GetComponent<QuestItemUI>().GetQuestStatus();
            tooltip.GetComponent<QuestToolTipUI>().Setup(status);

        }

    }
}