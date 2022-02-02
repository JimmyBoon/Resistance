using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Inventories;

namespace RPG.Inventories
{
    

    [CreateAssetMenu(fileName = "HealthActionItem", menuName = "Inventory/ActionItem/HealthActionItem", order = 0)]
    public class HealthActionItem : ActionItem
    {

        [SerializeField] float healthPointsToRestore = 50f;

        public override void Use(GameObject user)
        {
            Health health = user.GetComponent<Health>();
            health.Heal(healthPointsToRestore);
            Debug.Log("Health restore worked: " + healthPointsToRestore);
        }
    }
}