using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSpeed = 10f;
        [SerializeField] float projectileLiftTime = 10f;
        Health target = null;
        [SerializeField] GameObject explosionFX = null;
        [SerializeField] GameObject explosionFXmiss = null;
        [SerializeField] float endExplosionFX = 5f;
        [SerializeField] float endExplosionFXmiss = 5f;
        [SerializeField] float rotationSpeedx = 0f;
        [SerializeField] float rotationSpeedy = 0f;
        [SerializeField] float rotationSpeedz = 0f;
        
        float damage = 0f;
        float x =0;
        float y =0;
        float z =0;

        
        [SerializeField] bool isHomingProjectile = false;

        GameObject instigator;
        [SerializeField] AudioSource launchSound;
        [SerializeField] UnityEvent hitSound;

        private void Start() 
        {
            Destroy(gameObject, projectileLiftTime);
            if (target == null ) { return; }
            if (launchSound){
                launchSound.Play();
            }
            transform.LookAt(GetAimLocation());
            
        }

        void Update()
        {
            if (target == null) { return; }
            if(isHomingProjectile && target.IsAlive())
            {
                transform.LookAt(GetAimLocation());
            }
            
            
            transform.Translate(Vector3.forward * Time.deltaTime * projectileSpeed);
        }

        public void SetTarget(GameObject instigator, Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null) {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        
        private void OnTriggerEnter(Collider other) {
            
            if(other.gameObject == instigator.gameObject)
            {
                return;
            }

            if(other.gameObject.GetComponent<Character>())
            {
                Character character = other.gameObject.GetComponent<Character>();
                if(character.GetCharacterType() == instigator.GetComponent<Character>().GetCharacterType() && !character.GetFriendlyFireAllowed()){
                    return;
                }
            }

            if(other.gameObject.GetComponent<Health>())
            {
                hitSound.Invoke();
                other.gameObject.GetComponent<Health>().TakeDamage(instigator, damage);
                if(explosionFX){
                    GameObject explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);
                    Destroy(explosion, endExplosionFX);
                }
            }
            else
            {
                if (explosionFXmiss)
                {
                    GameObject explosion = Instantiate(explosionFXmiss, transform.position, Quaternion.identity);
                    Destroy(explosion, endExplosionFXmiss);
                }
            }


            Destroy(gameObject);
        }


        


        private void RotateProjectile()
        {
            z += rotationSpeedz * Time.deltaTime;
            y += rotationSpeedy * Time.deltaTime;
            x += rotationSpeedy * Time.deltaTime;
            transform.Rotate(x, y, z);
        }
    }
    


}


