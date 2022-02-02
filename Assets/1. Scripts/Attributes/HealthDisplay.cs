using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        Text text;

        private void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            text = GetComponent<Text>();
        }

        private void Update() 
        {
            text.text = (health.GetHealthPoints().ToString() + " / " + health.GetMaxHealthPoints().ToString()); //in lecture String.Format method was used to reduce the decimal places.
        }

    }
}