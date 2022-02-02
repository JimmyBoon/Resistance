using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        [SerializeField] CursorType cursor;
        [SerializeField] bool active = true;
        
        public void SetCombatActive(bool combatActive)
        {
            active = combatActive;
        }

        public CursorType GetCursorType()
        {
            return cursor;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(!active) {return false;}

            Fighter fighter = callingController.GetComponent<Fighter>();

            if (!fighter.CanAttack(this.gameObject))
            {
                return false;

            }

            if (Input.GetMouseButton(0))
            {
                fighter.Attack(this.gameObject, callingController.GetCurrentSpeed());
            }

            return true;
        }
    }
}
