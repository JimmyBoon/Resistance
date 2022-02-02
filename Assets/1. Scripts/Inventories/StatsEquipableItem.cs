using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;

namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "StatsEquipeableItem", menuName = "Inventory/StatsEquipeableItem", order = 0)]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {

        [SerializeField] Modifer[] additiveModifier;
        [SerializeField] Modifer[] percentageModifier;

        [System.Serializable]
        struct Modifer
        {   
            public Stat stat;
            public float value;

        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach(Modifer modifer in additiveModifier)
            {
                if(stat == modifer.stat)
                {
                    yield return modifer.value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (Modifer modifer in percentageModifier)
            {
                if (stat == modifer.stat)
                {
                    yield return modifer.value;
                }
            }
        }
    }
}
