using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float> healthPoints;
        LazyValue<float> maxHealthPoints;
        [SerializeField] float regenerationHealthPercentage = 70f;
        [SerializeField] float injuredAtPercentageHealth = 25f;
        [SerializeField] UnityEvent onDeath;

        // Lecutre code
        [SerializeField] TakeDamageEvent takeDamage;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        //my code
        public delegate void OnTakeDamageDelegate(float damage, GameObject instigator);
        public OnTakeDamageDelegate onTakeDamage;


        bool isAlive = true;
        [SerializeField] bool isInjured = false;

        //Todo Make injured methods, change the bool to true and reduce the max

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealthPoints);
            maxHealthPoints = new LazyValue<float>(GetInitialHealthPoints);
        }

        private float GetInitialHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            healthPoints.ForceInit();
            maxHealthPoints.ForceInit();
            
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += UpdateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= UpdateHealth;
        }


        public bool IsAlive()
        {
            return isAlive;
        }

        private void UpdateHealth()
        {
            maxHealthPoints.value = GetComponent<BaseStats>().GetStat(Stat.Health);
            float regenHealthPoints = maxHealthPoints.value * (regenerationHealthPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
            CalculateInjured(injuredAtPercentageHealth);
            print(healthPoints.value);
        }


        public void TakeDamage(GameObject instigator, float damage)
        {
            

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            if (healthPoints.value == 0 && isAlive)
            {
                AwardExperience(instigator);
                Death();
            }
            else
            {
                CalculateInjured(injuredAtPercentageHealth);
                takeDamage.Invoke(damage);

                onTakeDamage(damage, instigator);
            }
        }

        public void Heal(float healthPointsToHeal)
        {
            healthPoints.value = Mathf.Clamp(healthPoints.value + healthPointsToHeal, 0, maxHealthPoints.value);
            CalculateInjured(injuredAtPercentageHealth);
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience)
            {
                experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
            }
        }

        private void Death()
        {
            isAlive = false;
            onDeath.Invoke();
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<CapsuleCollider>().enabled = false; //to not allow attacks anymore

            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<NavMeshAgent>().enabled = false; //to not impeed movement of other charaters
        }

        private void DeathOnRestore()
        {
            isAlive = false;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<CapsuleCollider>().enabled = false; //to not allow attacks anymore

            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<NavMeshAgent>().enabled = false; //to not impeed movement of other charaters
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool IsInjured()
        {
            return isInjured;
        }

        private void CalculateInjured(float injuredPercentage)
        {
            if(healthPoints.value/maxHealthPoints.value < injuredPercentage / 100)
            {
                isInjured = true;
            }
            else
            {
                isInjured = false;
            }
        }

        //ISaveable:
        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {

            healthPoints.value = (float)state;
            CalculateInjured(injuredAtPercentageHealth);
            if (healthPoints.value <= 0)
            {
                isAlive = false;
                DeathOnRestore();
            }
        }


    }
}


