using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicsControlRemover : MonoBehaviour
    {
        GameObject player;
        PlayableDirector playableDirector;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playableDirector = GetComponent<PlayableDirector>();
        }

        private void OnEnable()
        {
            if (!playableDirector) { return; }
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;
        }

        private void OnDisable()
        {
            if (!playableDirector) { return; }
            playableDirector.played -= DisableControl;
            playableDirector.stopped -= EnableControl;
        }



        private void DisableControl(PlayableDirector aDirector)
        {
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<ActionScheduler>().CancelCurrentAction();

        }

        private void EnableControl(PlayableDirector aDirector)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}


