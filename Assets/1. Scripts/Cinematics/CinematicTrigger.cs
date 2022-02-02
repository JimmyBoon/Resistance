using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool hasPlayed = false;

        public object CaptureState()
        {
            return hasPlayed;
        }

        public void RestoreState(object state)
        {
            hasPlayed = (bool)state;
        }

        private void OnTriggerEnter(Collider collider) {
            
            if(hasPlayed){ return; }

            if(collider.gameObject.tag == "Player")
            {
                hasPlayed = true;
                GetComponent<PlayableDirector>().Play();
            }
            
        }

    }
}