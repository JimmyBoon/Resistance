using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foregroundTransform;
        Health health;
        Canvas canvas;

        float x = 1f;

        
        private void Awake() 
        {
            health = GetComponentInParent<Health>();
            canvas = GetComponentInChildren<Canvas>();
        }

        void Update()
        {

            if(health == null)
            { 
                canvas.enabled = false; 
                return;
            }

            x = health.GetHealthPoints() / health.GetMaxHealthPoints();

            if (Mathf.Approximately(x, 0) || Mathf.Approximately(x, 1))
            {
                canvas.enabled = false;
            }
            else
            {
                canvas.enabled = true;
            }


            foregroundTransform.localScale = new Vector3(x, 1f, 1f);
        }
    }
}