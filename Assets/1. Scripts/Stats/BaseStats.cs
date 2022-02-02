using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharaterClass charaterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffects;
        [SerializeField] bool shouldUseModifiers = false;
        LazyValue<int> currentLevel;

        Experience experience;
        
        private void Awake() 
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }
        private void Start()
        {
            currentLevel.ForceInit();
            UpdateLevel();
        }

        private void OnEnable() 
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel; //this adds UpdateLevel to the list of methods on which is called when GainExperience is called in Experience.cs
            }
        }

        private void OnDisable() 
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel; //this adds UpdateLevel to the list of methods on which is called when GainExperience is called in Experience.cs
            }
        }
        
        

        public event Action onLevelUp;

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                onLevelUp();
                LevelUpEffects();
            }
        }



        private void LevelUpEffects()
        {
            if (levelUpEffects)
            {
                GameObject effects = Instantiate(levelUpEffects, transform.position, Quaternion.identity);
                Destroy(effects, 5f);
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }



        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, charaterClass, GetLevel());
        }


        public int GetLevel()
        {
            return currentLevel.value;
        }

        public float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifer in provider.GetAdditiveModifiers(stat))
                {
                    total += modifer;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifer in provider.GetPercentageModifiers(stat))
                {
                    total += modifer;
                }
            }
            return total;
        }

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (!experience)
            {
                return startingLevel;
            }

            float currentXP = GetComponent<Experience>().GetExperiencePoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, charaterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                if (progression.GetStat(Stat.ExperienceToLevelUp, charaterClass, level) < currentXP)
                {
                    continue;
                }
                else
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }
    }
}


