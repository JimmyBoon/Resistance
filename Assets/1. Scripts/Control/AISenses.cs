using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Control
{
    public class AISenses : MonoBehaviour
    {
        [SerializeField] float seeingRange = 20f;
        [Tooltip("Sight angle will only be up to 180 degrees. With 0 begin directly forward and 90 begin able to see completely left or right")]
        [SerializeField(), Range(0, 180)] float sightAngle = 80f;
        [SerializeField] GameObject eyes;

        Vector3 heading;
        float angle;
        Vector3 lastSeenLocation;

        public List<Character> ScanForCharacters()
        {
            List<Character> identifiedTargets = new List<Character>();

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, seeingRange, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<Character>())
                {
                    if (InSight(hit.transform.gameObject))
                    {
                        Character characterBeingChecked = GetComponent<AISuspecion>().CheckThreatLevel(hit.transform.GetComponent<Character>());
                        if (characterBeingChecked != null)
                        {
                            identifiedTargets.Add(hit.transform.GetComponent<Character>());
                        }
                    }
                }
            }

            return identifiedTargets;
        }

        //Seeing

        private bool InSightAngle(GameObject target)
        {
            if(target == null){return false;}
            heading = target.transform.position - eyes.transform.position;
            angle = Vector3.Angle(heading, eyes.transform.forward);
            return angle < sightAngle;
        }

        public void SetLastSeenLocation(Vector3 target)
        {
            lastSeenLocation = target;
        }

        public bool InSight(GameObject target)
        {
            if (target == null) { return false; }
            RaycastHit hit;
            if (DistanceToTarget(target) > seeingRange) { return false; }
            bool canSee = Physics.Linecast(eyes.transform.position, target.transform.position, out hit) && InSightAngle(target) && hit.transform.gameObject.name == target.gameObject.name;
            if (canSee)
            {
                lastSeenLocation = target.transform.position;
            }
            return canSee;
        }

        public Vector3 GetLastSeenLocation()
        {
            return lastSeenLocation;
        }

        //Hearing

        public delegate void OnNoiseHeardDelegate(NoiseInstance noise);
        public OnNoiseHeardDelegate onNoiseHeard;

        public void SoundWasHeard(NoiseInstance noiseInstance)
        {
            if(noiseInstance.noiseSource == null)
            {
                print("something is null in the noise instance " + noiseInstance.noiseSource);
                print(gameObject);
                return;
            }
            
            onNoiseHeard(noiseInstance);
        }

        private float DistanceToTarget(GameObject target)
        {
            if(target == null) { return Mathf.Infinity; }
            return Vector3.Distance(transform.position, target.transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, seeingRange);
        }


    }
}


