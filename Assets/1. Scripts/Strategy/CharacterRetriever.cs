using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Strategy
{
    public class CharacterRetriever : MonoBehaviour
    {
        [SerializeField] Transform characterBank;

        private void OnTriggerEnter(Collider other) 
        {
            if(other.GetComponent<AIController>())
            {
                other.transform.SetParent(characterBank);

                AIController aIController = other.GetComponent<AIController>();
                aIController.ResetWaypointIndex();
                aIController.enabled = false;
                other.GetComponent<NavMeshAgent>().Warp(characterBank.position);
            }
        }

    }

}

