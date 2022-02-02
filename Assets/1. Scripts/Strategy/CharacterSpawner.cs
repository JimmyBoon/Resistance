using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Strategy
{
    public class CharacterSpawner : MonoBehaviour
    {
        CharacterBank characterBank;
        [SerializeField] KeyCode keyCode;
        [SerializeField] Transform spawnLocation;
        [SerializeField] PatrolPath defaultPatrolPath = null;

        private void Awake() {
            characterBank = GetComponentInChildren<CharacterBank>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                ActivateCharacter(defaultPatrolPath, transform);
            }

        }


        public void ActivateCharacter(PatrolPath patrolPath, Transform spawnParent)
        {
            GameObject charaterInstance = characterBank.GetNextCharacterInBank();
            if (charaterInstance == null) { return; }
            
            charaterInstance.transform.SetParent(spawnParent);
            charaterInstance.GetComponent<AISenses>().enabled = true;
            AIController aIController = charaterInstance.GetComponent<AIController>();
            aIController.enabled = true;

            aIController.SetPatrolPath(patrolPath);
            charaterInstance.GetComponent<NavMeshAgent>().Warp(spawnLocation.position);
        }

    }
}