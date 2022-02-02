using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [Tooltip("The weapon scriptable object")]
        [SerializeField] WeaponConfig weapon;
        [SerializeField] float rotateSpeed = 1f;
        [SerializeField] float respawnTime = 5f;
        [SerializeField] float healthToRestore = 0f;
        [SerializeField] CursorType cursor;
        
        private void Update()
        {
            RotateItem();
        }

        private void RotateItem()
        {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == GameObject.FindWithTag("Player"))
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if(weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if(healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            HideItem();
            yield return new WaitForSeconds(seconds);
            ShowItem();
        }

        private void HideItem()
        {
            GetComponent<BoxCollider>().enabled = false;
            Transform[] transforms = GetComponentsInChildren<Transform>();
            foreach (Transform trans in transforms)
            {
                if(trans == this.transform){continue;}
                trans.gameObject.SetActive(false);
            }
        }

        private void ShowItem()
        {
            GetComponent<BoxCollider>().enabled = true;
            Transform[] transforms = GetComponentsInChildren<Transform>(true);
            foreach (Transform trans in transforms)
            {
                trans.gameObject.SetActive(true);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.gameObject);

            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return cursor;
        }
    }
}

