using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using UnityEngine;
using System;
using UnityEngine.AI;
using GameDevTV.Utils;
using RPG.Saving;

namespace RPG.Control
{
    public class AIController : MonoBehaviour, ISaveable
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspectionTime = 5f;
        [SerializeField] protected float attackAlertTime = 1f;
        [SerializeField] float hearingLoseInterestTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] protected float waypointTollerance = 1f;
        [SerializeField] float dwellTimeMin = 0.1f;
        [SerializeField] float dwellTimeMax = 2f;
        [Header("Speeds")]
        [SerializeField] float patrolSpeed = 3f;
        [SerializeField] protected float attackSpeed = 5f;
        [SerializeField] protected float investigateSpeed = 4f;
        [SerializeField] float injuredSpeed = 1.2f;
        [SerializeField] Transform hospital;
        [Tooltip("Alert distance will inform near by AI when engaged in an attack")]
        [SerializeField] protected float alertDistance = 7f;
        [SerializeField] string noiseHeard;

        LazyValue<Vector3> guardPosition;
        protected GameObject target = null;
        GameObject lastAttackedTarget = null;
        protected Mover mover;
        Fighter fighter;
        Health health;
        ActionScheduler actionScheduler;
        NavMeshAgent navMeshAgent;
        protected AISenses aISenses;
        protected AISuspecion aISuspecion;
        Animator animator;
        protected Character character;

        protected float timeSinceLastSawTarget = Mathf.Infinity;
        float timeSinceLastHeardNoiseToInvestigate = Mathf.Infinity;
        float timeSinceLastTookDamage = Mathf.Infinity;
        protected float timeSinceAlerted = Mathf.Infinity;

        NoiseInstance noiseToInvestigate;

        float timeSinceArrivingAtWaypoint = 0;
        float dwellTime = 1f;
        int currentWaypointIndex = 0;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            aISenses = GetComponent<AISenses>();
            aISuspecion = GetComponent<AISuspecion>();
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();
            guardPosition = new LazyValue<Vector3>(GetInitialGuardPosition);
        }

        private void OnEnable()
        {
            health.onTakeDamage += OnTakeDameage;
            aISenses.onNoiseHeard += OnNoiseHeard;
        }

        // private void OnDisable()
        // {
        //     health.onTakeDamage -= OnTakeDameage;
        //     aISenses.onNoiseHeard -= OnNoiseHeard;
        // }


        private Vector3 GetInitialGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {

            // am I dead?
            if (!health.IsAlive()) { return; }

            target = ScanAndSetTarget();

            // am I injured?
            if (InjuredBehaviour()) { return; }

            //is there something to attack? 
            //is it in range?
            //fighter.CanAttack
            //can I see it? aISenses
            if (AbleToEngageTarget())
            {
                timeSinceLastSawTarget = 0;
                lastAttackedTarget = target;
                AttackBehaviour();
            }
            else if (ShouldReaquireTarget())
            {
                ReacquireTarget();
            }
            else if (suspectionTime > timeSinceLastSawTarget || suspectionTime > timeSinceAlerted)
            {
                SuspicionBehaviour();
            }
            else if (timeSinceLastHeardNoiseToInvestigate < hearingLoseInterestTime)
            {
                OnHearingBehaviour(noiseToInvestigate.noiseLocation);
            }
            else
            {
                PatrolBehaviour();
                target = null;
                lastAttackedTarget = null;
            }

            UpdateTimers();



        }

        protected virtual bool ShouldReaquireTarget()
        {
            return attackAlertTime > timeSinceLastSawTarget && target == null && lastAttackedTarget != null || attackAlertTime > timeSinceLastTookDamage && target == null && lastAttackedTarget != null;
        }

        protected virtual bool AbleToEngageTarget()
        {
            return InAttackRange() && fighter.CanAttack(target.gameObject) && aISenses.InSight(target);
        }

        protected virtual void ReacquireTarget()
        {
            transform.LookAt(lastAttackedTarget.transform.position);
        }

        private void OnNoiseHeard(NoiseInstance noise)
        {
            if(aISuspecion.ShouldSoundBeInvestigated(noise))
            {
                noiseToInvestigate = noise;
                timeSinceLastHeardNoiseToInvestigate = 0;
            }
            
        }



        private void UpdateTimers()
        {
            timeSinceLastSawTarget += Time.deltaTime;
            timeSinceLastHeardNoiseToInvestigate += Time.deltaTime;
            timeSinceLastTookDamage += Time.deltaTime;
            timeSinceAlerted += Time.deltaTime;
        }



        private GameObject ScanAndSetTarget()
        {
            List<Character> targets = new List<Character>(aISenses.ScanForCharacters());
            float distanceToClosestTarget = Mathf.Infinity;
            if (targets.Count > 0)
            {
                print(targets.Count);
                GameObject targetToReturn = targets[0].gameObject;
                foreach(Character character in targets)
                {
                    float distanceToThisTarget = Vector3.Distance(character.transform.position, transform.position);
                    if(distanceToThisTarget < distanceToClosestTarget)
                    {
                        distanceToClosestTarget = distanceToThisTarget;
                        targetToReturn = character.gameObject;
                    }
                }
                return targetToReturn;
            }

            return null;

        }

        private bool InjuredBehaviour()
        {
            if(!health.IsInjured())
            {
                animator.SetBool("injured", false);
                return false;
            }
            if(health.IsInjured())
            {
                animator.SetBool("injured", true);
                if(hospital != null)
                {
                    mover.StartMoveAction(hospital.position, injuredSpeed);
                }
                
                return true;
            }
            return false;

        }

        protected float CheckInjuredStatus(float requestedSpeed)
        {
            if (health.IsInjured())
            {
                animator.SetBool("injured", true);
                return injuredSpeed;
            }
            else
            {
                animator.SetBool("injured", false);
                return requestedSpeed;
            }
        }

        protected virtual void OnHearingBehaviour(Vector3 location)
        {
            mover.StartMoveAction(location, CheckInjuredStatus(investigateSpeed));
        }

        protected virtual void AttackBehaviour()
        {
            AlertNearByCounterparts(target);
            fighter.Attack(target.gameObject, attackSpeed);
        }

        protected virtual void AlertNearByCounterparts(GameObject target)
        {
            if(target == null)
            {
                print("Target is null");
            }
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, alertDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                if(hit.transform.GetComponent<Character>() == null){
                    continue;
                }
                if(hit.transform.GetComponent<Character>().GetCharacterType() == character.GetCharacterType())
                {
                    hit.transform.GetComponent<AIController>().AlertedByCounterpart(target);
                }
            }
        }

        public virtual void AlertedByCounterpart(GameObject target)
        {
            timeSinceAlerted = 0;
            aISenses.SetLastSeenLocation(target.transform.position);
            aISuspecion.AddToTargetList(target);
        }

        private void SuspicionBehaviour()
        {
            //actionScheduler.CancelCurrentAction();
            mover.StartMoveAction(aISenses.GetLastSeenLocation(), CheckInjuredStatus(investigateSpeed));
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {

                    timeSinceArrivingAtWaypoint += Time.deltaTime;


                    if (timeSinceArrivingAtWaypoint > dwellTime)
                    {
                        CycleWaypoint();
                        timeSinceArrivingAtWaypoint = 0;
                    }
                }
                nextPosition = GetCurrentWaypoint();
            }

            mover.StartMoveAction(nextPosition, CheckInjuredStatus(patrolSpeed));
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());

            if (distanceToWaypoint < waypointTollerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CycleWaypoint()
        {
            dwellTime = GetDwellTime();
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        public void ResetWaypointIndex()
        {
            currentWaypointIndex = 0;
        }

        public void SetPatrolPath(PatrolPath newPatrolPath)
        {
            patrolPath = newPatrolPath;
        }

        private float GetDwellTime()
        {
            return UnityEngine.Random.Range(dwellTimeMin, dwellTimeMax);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypointPosition(currentWaypointIndex);
        }


        private bool InAttackRange()
        {
            if(target == null){return false;}
            return Vector3.Distance(target.transform.position, transform.position) < chaseDistance;
        }

        protected virtual void OnTakeDameage(float damage, GameObject instigator)
        {
            timeSinceLastTookDamage = 0;
        }

        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        //Needed for animation
        public void FootR()
        {

        }
        public void FootL()
        {
        }
        public void Land()
        {
        }
        public void WeaponSwitch()
        {
        }

        public object CaptureState()
        {
            return transform.parent.name;
        }

        public void RestoreState(object state)
        {
            string parentName = (string)state;
            GameObject parent = GameObject.Find(parentName);
            transform.SetParent(parent.transform);
            print (parentName);
        }
    }
}

