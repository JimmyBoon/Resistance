using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class RunoverPickup : MonoBehaviour
    {
        Pickup pickup;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        // Re-enable this code, part of IRaycastable to allow for cursor affordance:
        
        // public CursorType GetCursorType()
        // {
        //     if (pickup.CanBePickedUp())
        //     {
        //         return CursorType.RunoverPickup;
        //     }
        //     else
        //     {
        //         return CursorType.FullPickup;
        //     }
        // }

        // public bool HandleRaycast(PlayerController callingController)
        // {
        //     return true;
        // }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject == GameObject.FindWithTag("Player"))
            {
                pickup.PickupItem();
            }
            
        }
    }
}