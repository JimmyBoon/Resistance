using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;
using RPG.Core;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier 
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float waitInFadeTime = 1f;
        
        Fader fader;
        SavingWrapper savingWrapper;

        private void Start() {
            fader = FindObjectOfType<Fader>();
            savingWrapper = FindObjectOfType<SavingWrapper>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == GameObject.FindWithTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0){
                Debug.LogError("Scene to load not set");
                yield break;
            }

            yield return fader.FadeOut(fadeOutTime);
            DontDestroyOnLoad(gameObject);

            PlayerControllerActivation(false);
            savingWrapper.Save();
            

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerControllerActivation(false);
            
            
            Portal otherPortal = GetOtherPortal();
            
            savingWrapper.Load();
            
            UpdatePlayer(otherPortal);
            savingWrapper.Save();

            yield return new WaitForSeconds(waitInFadeTime);
            fader.FadeIn(fadeInTime);
            PlayerControllerActivation(true);

            
            
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
            
        }

        private Portal GetOtherPortal(){
            
            foreach (Portal portal in FindObjectsOfType<Portal>()){
                if(portal != this && portal.destination == destination)
                {
                    return portal;
                }
                else
                {
                    continue;
                }
            }

            return null;
        }

        private void PlayerControllerActivation(bool setActive)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerController>().enabled = setActive;
            if(!setActive)
            {
                player.GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }
    }



}


