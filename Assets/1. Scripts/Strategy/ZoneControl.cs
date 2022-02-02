using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Strategy
{
    public class ZoneControl : MonoBehaviour
    {
        [SerializeField] bool inControl;
        [SerializeField] CharacterType[] enemyCharacters;
        [Range(1,10)]
        [SerializeField] int priortyForControl = 1;
        [SerializeField] PatrolPath[] patrolPaths;
        TroopsAssigned troopsAssignedTransform;
        [SerializeField] AIController[] troopsAssigned;
        [SerializeField] float zoneAssessmentArea = 5f;

        private void Awake() {
            
            troopsAssignedTransform = GetComponentInChildren<TroopsAssigned>();
            patrolPaths = GetComponentsInChildren<PatrolPath>();
        }

        private void Start() 
        {
            troopsAssigned = GetComponentsInChildren<AIController>();
        }

        private void Update() 
        {
            SetInControl(CheckForControl());
        }

        public void SetInControl(bool inControl)
        {
            this.inControl = inControl;
        }

        public bool GetInControl(){

            return CheckForControl();
        }

        public PatrolPath GetPatrolPath()
        {
            return patrolPaths[0];
        }

        public Transform GetTroopAssignmentParent(){
            return troopsAssignedTransform.gameObject.transform;
        }

        public AIController[] GetTroopsAssigned()
        {
            troopsAssigned = GetComponentsInChildren<AIController>();
            return troopsAssigned;
        }

        private bool CheckForControl()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, zoneAssessmentArea, Vector3.up, 0);
            foreach(RaycastHit hit in hits)
            {
                if(hit.transform.GetComponent<Character>())
                {
                    foreach(CharacterType type in enemyCharacters)
                    {
                        if (hit.transform.GetComponent<Character>().GetCharacterType() == type)
                        {
                            return false;
                            
                        }
                    }

                }

            }

            return true;

        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, zoneAssessmentArea);
        }
    }

}
