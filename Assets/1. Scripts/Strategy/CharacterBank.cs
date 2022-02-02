using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using RPG.Saving;

namespace RPG.Strategy
{
    public class CharacterBank : MonoBehaviour
    {
        
        [SerializeField] int index;
        [SerializeField] Character[] characterList;
        int restoredIndex = -2;



        
        private void Start()
        {
            characterList = GetComponentsInChildren<Character>();
            index = characterList.Length - 1;
        
        }

        public GameObject GetNextCharacterInBank()
        {
            characterList = GetComponentsInChildren<Character>();
            if(characterList.Length == 0)
            {
                print("No characters left in the bank");
                return null;
            }

            GameObject characterToReturn = characterList[0].gameObject;
            
            
            return characterToReturn;
        }

    }
}