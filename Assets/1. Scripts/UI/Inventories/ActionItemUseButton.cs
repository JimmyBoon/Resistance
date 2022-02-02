using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    public class ActionItemUseButton : MonoBehaviour
    {   
        [SerializeField] int buttonIndex;

        GameObject player;

        private void Awake() 
        {
            player = GameObject.FindWithTag("Player");
            
        }

        private void Start() {
            buttonIndex = GetComponentInParent<ActionSlotUI>().GetIndex();
        }

        public void UseItem()
        {
            player.GetComponent<ActionStore>().Use(buttonIndex, player);
        }
    }
}