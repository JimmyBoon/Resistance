using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Cinematics
{
    public class CameraZoneController : MonoBehaviour
    {
        [SerializeField] GameObject parentCameraController;
        [SerializeField] string triggerName;
        [SerializeField] bool selfDestructZone = false;
        [SerializeField] float destroyTime = 3f;
        

        Animator animator;


        void Awake()
        {
            animator = parentCameraController.GetComponent<Animator>();

        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == GameObject.FindWithTag("Player"))
            {
                animator.SetTrigger(triggerName);
                if (selfDestructZone)
                {
                    Destroy(gameObject, destroyTime);
                }
            }

        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject == GameObject.FindWithTag("Player"))
            {
                animator.ResetTrigger(triggerName);

            }
        }
    }

}


