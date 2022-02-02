using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Control;

namespace RPG.Movement
{

    public class Mover : MonoBehaviour, IAction, ISaveable, INoise
    {
        NavMeshAgent navMeshAgent;
        Animator animator;

        [SerializeField] float maxPathLength = 40f;

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }


        void Update()
        {
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speed)
        {

            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speed);
        }

        private void MoveToCursor(float speed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHit)
            {
                MoveTo(hit.point, speed);
            }
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) { return false; }
            if (path.status != NavMeshPathStatus.PathComplete) { return false; }
            if (GetPathLength(path) > maxPathLength) { return false; }
            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            Vector3[] corners = path.corners;
            float pathDistance = 0;
            if (corners.Length < 2) { return pathDistance; }

            for (int i = 0; i < corners.Length - 1; i++)
            {
                pathDistance += Vector3.Distance(corners[i], corners[i + 1]);
            }

            return pathDistance;
        }

        public void MoveTo(Vector3 destination, float speed)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            //this varies from the lecutre, see class 17 if needed to change later.
            float speed = GetSpeed();

            //Code from Lecture:
            // Vector3 velocity = navMeshAgent.velocity;
            // Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            // float speed = localVelocity.z;

            animator.SetFloat("forwardSpeed", speed);
        }

        private float GetSpeed()
        {
            return navMeshAgent.velocity.magnitude;
        }

        public NoiseInstance GetNoiseInstance()
        {
            NoiseInstance noise = new NoiseInstance();
            noise.noiseAmount = navMeshAgent.velocity.magnitude / 2;
            noise.noiseLocation = transform.position;
            noise.noiseSource = gameObject;
            noise.noiseType = NoiseType.Footsteps;
            return noise;
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
