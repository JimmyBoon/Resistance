using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [SerializeField] float ScatterDistance = 1f;
        [SerializeField] DropLibrary dropLibrary;


        const int attempts = 30;

        public void RandomDrop()
        {
            if(!dropLibrary){return;}
            var baseStats = GetComponent<BaseStats>();

            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.number);
            }
        }  

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < attempts; i++)
            {
                Vector3 randomDropLocation = transform.position + Random.insideUnitSphere * ScatterDistance;
                
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDropLocation, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }


            return transform.position;
        }
    }
}