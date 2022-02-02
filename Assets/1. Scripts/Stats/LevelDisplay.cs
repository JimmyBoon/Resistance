using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class LevelDisplay : MonoBehaviour
    {
        Text text;
        BaseStats baseStats;

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            text = GetComponent<Text>();
        }

        private void Update() 
        {
            text.text = baseStats.GetLevel().ToString();
        }
    }
}
