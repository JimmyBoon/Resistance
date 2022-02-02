using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using UnityEngine;

namespace RPG.Control
{
    public class Character : MonoBehaviour
    {
        [SerializeField] CharacterType characterType;
        [SerializeField] bool friendlyFireAllowed;

        public CharacterType GetCharacterType()
        {
            return characterType;
        }

        public bool GetFriendlyFireAllowed(){
            return friendlyFireAllowed;
        }

        public bool IsCharacterArmed()
        {
            WeaponClass weaponClass = GetComponent<Fighter>().GetWeaponConfig().GetWeaponClass();

            if(weaponClass == WeaponClass.Unarmed || weaponClass == WeaponClass.Tools)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }

}
