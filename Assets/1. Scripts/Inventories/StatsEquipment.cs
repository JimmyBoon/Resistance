using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (EquipLocation slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null)
                {
                    continue;
                }

                foreach (float modifer in item.GetAdditiveModifiers(stat))
                {
                    yield return modifer;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (EquipLocation slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null)
                {
                    continue;
                }

                foreach (float modifer in item.GetPercentageModifiers(stat))
                {
                    yield return modifer;
                }
            }
        }
    }
}
