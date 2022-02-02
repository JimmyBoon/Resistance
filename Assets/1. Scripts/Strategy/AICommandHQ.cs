using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Strategy
{
    public class AICommandHQ : MonoBehaviour
    {
        [SerializeField] ZoneControl[] zones;
        [SerializeField] KeyCode deployKey;
        [SerializeField] CharacterSpawner characterSpawner;
        [SerializeField] PatrolPath retrievePatrolPath;
        [SerializeField] Transform underOrdersTransform;
        

        private void Start() 
        {
            underOrdersTransform = GetComponentInChildren<UnderOrders>().transform;
            zones = GetComponentsInChildren<ZoneControl>();
        }

        private void Update() 
        {
            if(Input.GetKeyDown(deployKey))
            {
                foreach(ZoneControl zone in zones)
                {
                    if(zone.GetInControl() == false)
                    {
                        characterSpawner.ActivateCharacter(zone.GetPatrolPath(), zone.GetTroopAssignmentParent());
                    }
                    else
                    {
                        AIController[] troops = zone.GetTroopsAssigned();
                        foreach(AIController troop in troops)
                        {
                            troop.SetPatrolPath(retrievePatrolPath);
                            troop.transform.SetParent(underOrdersTransform);
                        }
                    }
                }
            }
        }

    }
}
