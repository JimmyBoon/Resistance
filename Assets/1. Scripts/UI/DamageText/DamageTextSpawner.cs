using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;

namespace RPG.UI.DamageText
{

    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] GameObject damageTextPrefab;

        Health health;

        private void Awake() {
            health = GetComponentInParent<Health>();
        }
        
        private void OnEnable() 
        {
            health.onTakeDamage += Spawn;            
        }
        private void OnDisable() 
        {
            health.onTakeDamage -= Spawn;
        }

        public void Spawn(float damageText, GameObject instigtor)
        {
            GameObject spawnedText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            Text text = spawnedText.GetComponentInChildren<Text>();
            text.text = damageText.ToString(); //if decimal places are an issue use string.format
            Destroy(spawnedText, 1f);
        }
    }

}


