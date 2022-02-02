using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;


namespace RPG.Control
{
    public class AIControllerNonCombatant : AIController
    {   
        [SerializeField] Transform[] runawayLocatons;
        [SerializeField] float safetyDistance = 5f;
        int index = 0;

        private void Start() 
        {
            RunawayIndexRandomiser();
        }

        private void RunawayIndexRandomiser()
        {
            index = Random.Range(0,runawayLocatons.Length);
        }


        protected override void OnHearingBehaviour(Vector3 location)
        {
            print("That sounds scary");
            mover.StartMoveAction(runawayLocatons[index].position, CheckInjuredStatus(investigateSpeed));
        }

        protected override void AttackBehaviour()
        {
            print("Oh my there is danger");
            AlertNearByCounterparts(target);
            if (AtRunawayLocation() && DistanceToTarget() < safetyDistance)
            {
                index++;
                if (index == runawayLocatons.Length)
                {
                    index = 0;
                }
            }
            mover.StartMoveAction(runawayLocatons[index].position, CheckInjuredStatus(attackSpeed));
        }

        private float DistanceToTarget()
        {
            if(target == null) 
            {
                return Mathf.Infinity;
            }
            return Vector3.Distance(transform.position, target.transform.position);
        }

        protected override void OnTakeDameage(float damage, GameObject instigator)
        {
            print("I took damage");
            if(AtRunawayLocation()){
                index++;
                if (index == runawayLocatons.Length)
                {
                    index = 0;
                }
            }
            mover.StartMoveAction(runawayLocatons[index].position, CheckInjuredStatus(attackSpeed));
        }

        private bool AtRunawayLocation()
        {

            float distanceToWaypoint = Vector3.Distance(transform.position, runawayLocatons[index].position);

            if (distanceToWaypoint < waypointTollerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void ReacquireTarget()
        {
            print("Keep running!");
        }

        protected override bool AbleToEngageTarget()
        {
            return aISenses.InSight(target);
        }

        protected override bool ShouldReaquireTarget()
        {
            return attackAlertTime > timeSinceLastSawTarget && target == null;
        }

        protected override void AlertNearByCounterparts(GameObject target)
        {
            if (target == null)
            {
                print("Target is null");
            }
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, alertDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<Character>() == null || hit.transform.GetComponent<AIControllerNonCombatant>() == null)
                {
                    continue;
                }
                if (hit.transform.GetComponent<Character>().GetCharacterType() == character.GetCharacterType())
                {
                    hit.transform.GetComponent<AIControllerNonCombatant>().AlertedByCounterpart(target);
                }
            }
        }

        public override void AlertedByCounterpart(GameObject target)
        {
            timeSinceAlerted = 0;
            aISenses.SetLastSeenLocation(target.transform.position);
            aISuspecion.AddToTargetList(target);
        }

    }
}

