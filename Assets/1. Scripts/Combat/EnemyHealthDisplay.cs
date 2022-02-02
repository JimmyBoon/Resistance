using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        Text text;
        Health target;


        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            text = GetComponent<Text>();
        }

        private void Update()
        {

            target = fighter.GetTarget();

            if (target)
            {
                text.text = (target.GetHealthPoints().ToString() + " / " + target.GetMaxHealthPoints().ToString());
            }
            else
            {
                text.text = "No Target";
            }

        }
    }
}


